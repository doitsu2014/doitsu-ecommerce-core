using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.DeliveryIntegration;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Doitsu.Ecommerce.Core.Services.Interface;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Services.EmailService;
using Doitsu.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.Core.Services
{
    public partial class OrderService : EcommerceBaseService<Orders>, IOrderService
    {
        private readonly IEmailService emailService;
        private readonly IProductService productService;
        private readonly IProductVariantService productVariantService;
        private readonly IUserTransactionService userTransactionService;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userManager;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDeliveryIntegrator deliveryIntegrator;

        public OrderService(EcommerceDbContext dbContext,
            IMapper mapper,
            ILogger<EcommerceBaseService<Orders>> logger,
            IEmailService emailService,
            IProductService productService,
            EcommerceIdentityUserManager<EcommerceIdentityUser> userManager,
            IUserTransactionService userTransactionService,
            IServiceScopeFactory serviceScopeFactory,
            IProductVariantService productVariantService,
            IDeliveryIntegrator deliveryIntegrator) : base(dbContext, mapper, logger)
        {
            this.emailService = emailService;
            this.productService = productService;
            this.userManager = userManager;
            this.userTransactionService = userTransactionService;
            this.serviceScopeFactory = serviceScopeFactory;
            this.deliveryIntegrator = deliveryIntegrator;
            this.productVariantService = productVariantService;
        }

        public async Task<Option<string, string>> CheckoutCartAsync(CheckoutCartViewModel data, EcommerceIdentityUser user)
        {
            return await new { data, user }
                .SomeNotNull()
                .WithException(string.Empty)
                .FlatMapAsync(async d =>
                {
                    using (var trans = await CreateTransactionAsync())
                    {
                        var order = new Orders();
                        order.Status = (int)OrderStatusEnum.New;
                        order.Discount = 0;
                        order.Code = DataUtils.GenerateCode(Constants.OrderInformation.ORDER_CODE_LENGTH).ToUpper();
                        order.TotalPrice = data.TotalPrice;
                        order.FinalPrice = data.TotalPrice;
                        order.TotalQuantity = data.TotalQuantity;
                        order.UserId = user.Id;

                        foreach (var item in data.CartItems)
                        {
                            var orderItem = new OrderItems
                            {
                                Discount = 0,
                                ProductId = item.Id,
                                OrderId = order.Id,
                                SubTotalPrice = item.SubTotal,
                                SubTotalQuantity = item.SubQuantity,
                                SubTotalFinalPrice = item.SubTotal
                            };
                            if (order.OrderItems == null) order.OrderItems = new List<OrderItems>();
                            order.OrderItems.Add(orderItem);
                        }
                        await CreateAsync(order);
                        await CommitAsync();
                        trans.Commit();
                        try
                        {
                            var messagePayloads = new List<MessagePayload>()
                            {
                                await emailService.PrepareLeaderOrderMailConfirmAsync(user, order),
                                await emailService.PrepareCustomerOrderMailConfirm(user, order)
                            };

                            var emailResult = await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
                            emailResult.MatchNone(error =>
                            {
                                Logger.LogInformation("Send mails to confirm order code {Code} on {CreatedDate} failure: {error}", user.Id, order.Code, order.CreatedDate, error);
                            });

                            Logger.LogInformation("User {Id} completed order code {Code} on {CreatedDate}", user.Id, order.Code, order.CreatedDate);
                            return Option.Some<string, string>(order.Code);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Process user order fail");
                            return Option.None<string, string>($"Quá trình xử lý đơn hàng bị lỗi.");
                        }
                    }
                });
        }

        public async Task<ImmutableList<OrderDetailViewModel>> GetAllOrdersByUserIdAsync(int userId)
        {
            var result = await this.GetAsNoTracking(x => x.UserId.Equals(userId))
                .ProjectTo<OrderDetailViewModel>(Mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return result.ToImmutableList();
        }

        public async Task<OrderDetailViewModel> GetOrderDetailByCodeAsync(string orderCode)
        {
            var result = await this.FirstOrDefaultAsync<OrderDetailViewModel>(x => x.Code.Equals(orderCode));
            return result;
        }

        public async Task<Option<OrderDetailViewModel, string>> CreateOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            return await request.SomeNotNull()
                    .WithException("Dữ liệu truyền lên rỗng.")
                    .Filter(d => d.UserId > 0, "Thông tin người đặt hàng không hợp lệ.")
                    .FlatMapAsync(async d => await MapOrderWithOptionToSaleOrderAsync(d))
                    .MapAsync(async d => {
                        await this.CreateAsync<CreateOrderWithOptionViewModel>(d);
                        await this.CommitAsync();
                        return this.Mapper.Map<OrderDetailViewModel>(d);
                    });
        }

        public async Task<Option<OrderViewModel, string>> CreateNormalOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await CreateOrderWithOptionAsync(request) 
                    .MapAsync(async o =>
                    {
                        await SendEmailToUsersAsync(this.Mapper.Map<Orders>(o));
                        await transaction.CommitAsync();
                        return this.Mapper.Map<OrderViewModel>(o);
                    });
            }
        }

        public async Task<Option<OrderViewModel, string>> CreateSaleOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await CreateOrderWithOptionAsync(request)  
                    .MapAsync(async data => await Task.FromResult((createdOrder: data, productVariants: data.OrderItems.Select(oi => oi.ProductVariant).ToImmutableList())))
                    .MapAsync(async data =>
                    {
                        var userTransaction = this.userTransactionService.PrepareUserTransaction(data.createdOrder, data.productVariants, data.createdOrder.UserId);
                        var result = await userTransactionService.UpdateUserBalanceAsync(userTransaction, data.createdOrder.UserId);
                        return data.createdOrder;
                    })
                    .MapAsync(async orderDetail =>
                    {
                        var entityOrder = this.Mapper.Map<Orders>(orderDetail);
                        await SendEmailToUsersAsync(entityOrder);
                        await transaction.CommitAsync();
                        return this.Mapper.Map<OrderViewModel>(orderDetail);
                    });
            }

        }

        /// <summary>
        /// This function is serving on the creating order action and rewrite user's balance from 1 transaction scope
        /// Obselete and why?
        /// Because using transaction should using on Controller or another higher Layer
        /// So this function will be deleted on the some next version
        /// And you should use CreateNormalOrderWithOptionAsync instead of this function.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<Option<OrderViewModel, string>> CreateWithdrawalOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await request.SomeNotNull()
                .WithException("Dữ liệu truyền lên rỗng.")
                .Filter(d => d.UserId > 0, "Không tìm thấy thông tin tài khoản của bạn khi thực hiện Rút Tiền.")
                .Filter(d => d.TotalPrice > 0, "Số tiền để rút tiền phải lớn hơn 0.")
                .Filter(d => d.FinalPrice > 0, "Số tiền để rút tiền phải lớn hơn 0.")
                // Prepare data
                .MapAsync(async d =>
                {
                    // map to orders
                    var order = this.Mapper.Map<Orders>(d);
                    order.Code = DataUtils.GenerateCode(Constants.OrderInformation.ORDER_CODE_LENGTH);
                    order.CreatedDate = DateTime.UtcNow.ToVietnamDateTime();
                    order.Type = OrderTypeEnum.Withdrawal;
                    order.Status = (int)OrderStatusEnum.Processing;
                    await this.CreateAsync(order);
                    await this.CommitAsync();
                    return (order);
                })
                .FlatMapAsync(async d =>
                {
                    // prepare transaction
                    var userTransaction = this.userTransactionService.PrepareUserTransaction(this.Mapper.Map<OrderDetailViewModel>(d),
                                                                                             ImmutableList<ProductVariantViewModel>.Empty,
                                                                                             d.UserId,
                                                                                             UserTransactionTypeEnum.Withdrawal);
                    return (await userTransactionService.UpdateUserBalanceAsync(userTransaction, d.UserId))
                        .Map(res =>
                        {
                            return d;
                        });
                })
                .MapAsync(async o =>
                {
                    await SendEmailToUsersAsync(o);
                    await transaction.CommitAsync();
                    return this.Mapper.Map<OrderViewModel>(o);
                });
            }

        }

        /// <summary>
        /// If system can not detect or detect on 0 Price, system will using the client price @orderItemSubTotalPrice.
        /// </summary>
        /// <param name="productVariant">The product variant values to detect product</param>
        /// <param name="orderItemSubTotalPrice">The sub total price, what price is typed from client side</param>
        /// <returns></returns>
        private decimal DetectSubTotalPrice(ProductVariantDetailViewModel productVariant, decimal orderItemSubTotalPrice)
        {
            if (productVariant.AnotherPrice > 0)
            {
                return productVariant.AnotherPrice;
            }
            else if (productVariant.ProductPrice > 0)
            {
                return productVariant.ProductPrice;
            }
            else
            {
                return orderItemSubTotalPrice > 0 ? orderItemSubTotalPrice : 0;
            }
        }

        private async Task<Option<CreateOrderWithOptionViewModel, string>> MapOrderWithOptionToSaleOrderAsync(CreateOrderWithOptionViewModel request)
        {
            return (await request.SomeNotNull()
                .WithException("Thông tin đơn hàng rỗng")
                .FlatMapAsync(async d =>
                {
                    var calcShippingFeesReqData = this.Mapper.Map<CalculateDeliveryFeesRequestModel>(d);
                    calcShippingFeesReqData.Weight = 0;

                    foreach (var orderItem in d.OrderItems)
                    {
                        var productVariant = await productVariantService.FindProductVariantFromOptionsAsync(orderItem.ProductOptionValues);
                        if (productVariant == null)
                        {
                            return Option.None<CreateOrderWithOptionViewModel, string>("Không tìm thấy 1 sản phẩm mà bạn muốn đặt hàng.");
                        }
                        else if (productVariant.Status == ProductVariantStatusEnum.Unavailable)
                        {
                            return Option.None<CreateOrderWithOptionViewModel, string>("Có 1 sản phẩm mà bạn muốn đặt hàng đang bị khóa. Vui lòng thử lại vào khoản thời gian tiếp theo.");
                        }
                        else
                        {
                            // Mapping data to order items
                            orderItem.ProductId = productVariant.ProductId;
                            orderItem.Discount = productVariant.AnotherDiscount;

                            var subTotalPrice = DetectSubTotalPrice(productVariant, orderItem.SubTotalPrice);
                            orderItem.SubTotalPrice = subTotalPrice;

                            var discount = orderItem.Discount ?? 0;
                            var price = subTotalPrice * orderItem.SubTotalQuantity;
                            orderItem.SubTotalFinalPrice = price - (price * ((decimal)discount) / 100);
                            orderItem.ProductId = productVariant.ProductId;
                            orderItem.ProductVariant = productVariant;
                            orderItem.ProductVariantId = productVariant.Id;
                            d.TotalPrice += price;
                            d.TotalQuantity += orderItem.SubTotalQuantity;
                            d.FinalPrice += orderItem.SubTotalFinalPrice;

                            // Mapping data to delivery information
                            calcShippingFeesReqData.Weight += productVariant.ProductWeight;
                        }
                    }
                    if (d.DeliveryProvider != null)
                    {
                        var deliver = d.DeliveryProvider.Value;
                        var defaultWareHouse = await this.DbContext.WareHouses.FirstOrDefaultAsync();
                        calcShippingFeesReqData.PickAddress = defaultWareHouse.Address;
                        calcShippingFeesReqData.PickProvince = defaultWareHouse.City;
                        calcShippingFeesReqData.PickWard = defaultWareHouse.Ward;
                        calcShippingFeesReqData.PickDistrict = defaultWareHouse.District;
                        (await this.deliveryIntegrator.CalculateShipFeeAsync(deliver, calcShippingFeesReqData))
                            .Map(fees => d.DeliveryFees = fees);
                    }
                    return Option.Some<CreateOrderWithOptionViewModel, string>(d);
                }))
                .Map(d =>
                {
                    if (d.Priority.HasValue)
                    {
                        var originPrice = d.TotalPrice * d.TotalQuantity;
                        var priorityValue = ((decimal)d.Priority.Value / 100);
                        d.FinalPrice += (originPrice * priorityValue);
                    }

                    d.Code = DataUtils.GenerateCode(Constants.OrderInformation.ORDER_CODE_LENGTH);
                    d.CreatedDate = DateTime.UtcNow.ToVietnamDateTime();
                    d.Type = OrderTypeEnum.Sale;
                    d.Status = (int)OrderStatusEnum.New;
                    return d;
                });
        }

        public async Task<ImmutableList<OrderDetailViewModel>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
                                                                                      DateTime? fromDate,
                                                                                      DateTime? toDate,
                                                                                      string userPhone,
                                                                                      string orderCode,
                                                                                      ProductFilterParamViewModel[] productFilter,
                                                                                      OrderTypeEnum? orderType,
                                                                                      int? summaryOrderId)
        {
            var query = this.GetAllAsNoTracking()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (orderStatus.HasValue) query = query.Where(x => x.Status == (int)orderStatus.Value);
            if (!userPhone.IsNullOrEmpty()) query = query.Where(x => x.User.PhoneNumber.Contains(userPhone));
            if (!orderCode.IsNullOrEmpty()) query = query.Where(x => x.Code.Contains(orderCode));
            if (fromDate.HasValue && fromDate != DateTime.MinValue)
            {
                query = query.Where(x => x.CreatedDate >= fromDate.Value.StartOfDay());
                if (toDate.HasValue && toDate != DateTime.MinValue)
                {
                    query = query.Where(x => x.CreatedDate <= toDate.Value.EndOfDay());
                }
            }
            if (orderType != null) query = query.Where(x => x.Type == orderType);
            if (productFilter != null && productFilter.Length > 0)
            {
                var productIds = productFilter.Select(pf => pf.Id);
                query = query.Where(o => o.OrderItems.Select(oi => oi.ProductId).Any(pi => productIds.Contains(pi)));

                var productVariantIds = await productVariantService.GetProductVariantIdsFromProductFilterParamsAsync(productFilter);
                if (productVariantIds != null && productVariantIds.Length > 0)
                {
                    query = query.Where(o => o.OrderItems.Select(oi => oi.ProductVariantId).Any(pvId => productVariantIds.Contains(pvId)));
                }
            }
            if (summaryOrderId != null) query = query.Where(o => o.SummaryOrderId == summaryOrderId);

            var result = await query
                .ProjectTo<OrderDetailViewModel>(Mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return result.ToImmutableList();
        }

        public async Task<Option<OrderViewModel, string>> CreateDepositOrderAsync(OrderViewModel request)
        {
            return await request.SomeNotNull()
                .WithException("Dữ liệu truyền lên rỗng.")
                .Filter(d => d.UserId > 0, "Không tìm thấy thông tin người được nạp tiền.")
                .Map(d =>
                {
                    var order = this.Mapper.Map<Orders>(request);
                    order.Code = DataUtils.GenerateCode(Constants.OrderInformation.ORDER_CODE_LENGTH);
                    order.CreatedDate = DateTime.UtcNow.ToVietnamDateTime();
                    order.Type = OrderTypeEnum.Desposit;
                    order.Status = (int)OrderStatusEnum.Done;
                    var price = d.TotalPrice * d.TotalQuantity;
                    order.FinalPrice = price - (price * (decimal)d.Discount);
                    return order;
                })
                .FlatMapAsync(async o =>
                {
                    var userTransaction = this.userTransactionService.PrepareUserTransaction(this.Mapper.Map<OrderDetailViewModel>(o),
                                                                                             ImmutableList<ProductVariantViewModel>.Empty,
                                                                                             o.UserId,
                                                                                             UserTransactionTypeEnum.Income);
                    var updateBalanceResult = await userTransactionService.UpdateUserBalanceAsync(userTransaction, o.UserId);
                    return updateBalanceResult.Match<Option<Orders, string>>(
                        res =>
                        {
                            return Option.Some<Orders, string>(o);
                        },
                        error => { return Option.None<Orders, string>(error); }
                    );
                })
                .MapAsync(async o =>
                {
                    await SendEmailToUsersAsync(o);
                    return this.Mapper.Map<OrderViewModel>(o);
                });
        }

    }
}