using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Doitsu.Ecommerce.Core.ViewModels;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Services.EmailService;
using Doitsu.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OfficeOpenXml;

using Optional;
using Optional.Async;
using Optional.Collections;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IOrderService : IBaseService<Orders>
    {
        /// <summary>
        /// Version on Bach Moc Website
        /// Should be update
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Option<string, string>> CheckoutCartAsync(CheckoutCartViewModel data, EcommerceIdentityUser user);

        Task<ImmutableList<OrderDetailViewModel>> GetAllOrdersByUserIdAsync(int userId);

        Task<OrderDetailViewModel> GetOrderDetailByCodeAsync(string orderCode);

        Task<ImmutableList<OrderDetailViewModel>> GetOrderDetailByParams(OrderStatusEnum? orderStatus,
            DateTime? fromDate,
            DateTime? toDate,
            string userPhone,
            string orderCode,
            ProductFilterParamViewModel[] productFilter,
            OrderTypeEnum? orderType,
            int? summaryOrderId);

        /// <summary>
        /// Create a sale order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateSaleOrderWithOptionAsync(CreateOrderWithOptionViewModel request);

        /// <summary>
        /// Create Withdrawl order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateWithdrawalOrderWithOptionAsync(CreateOrderWithOptionViewModel request);

        /// <summary>
        /// Create Deposit order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateDepositOrderAsync(OrderViewModel request);

        /// <summary>
        /// This feature just use for Orders with New Status, and Sale Type
        /// Inverse Orders will be overrided to Processing Status if feature done.
        /// </summary>
        /// <param name="inverseOrders"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CreateSummaryOrderAsync(CreateSummaryOrderViewModel inverseOrders, int userId);

        /// <summary>
        /// Get bytes array of Export Summary Excel (detail style)
        /// </summary>
        /// <param name="summaryOrderId"></param>
        /// <returns></returns>
        Task<Option<ExportOrderToExcel, string>> GetSummaryOrderDetailStyleAsExcelBytesAsync(int summaryOrderId);

        /// <summary>
        /// Get bytes array of Export Summary Excel (phonecard website style)
        /// </summary>
        /// <param name="summaryOrderId"></param>
        /// <returns></returns>
        Task<Option<ExportOrderToExcel, string>> GetSummaryOrderPhoneCardWebsiteStyleAsExcelBytesAsync(int summaryOrderId);

        /// <summary>
        /// Complete the summary order and included different cancelled orders.
        /// </summary>
        /// <param name="summaryOrderId">Summary Order Id to Query</param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> CompleteSummaryOrderAsync(int summaryOrderId);

        Task<Option<OrderViewModel, string>> CancelSummaryOrderAsync(int summaryOrderId, int auditUserId, string cancelNote = "");

        Task<Option<OrderViewModel, string>> CancelOrderAsync(string orderCode, int userId, string cancelNote = "");


        Task<Option<OrderViewModel, string>> CompleteOrderAsync(string orderCode, int userId);

        /// <summary>
        /// Superpower function, to work correctly we have to complete
        /// TODO: Wrap all logic change order status
        /// CURRENT: just as a controller pass cancel or done status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="statusEnum"></param>
        /// <returns></returns>
        Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "");

        Task<Option<OrderViewModel, string>> ChangeOrderNote(int orderId, string note = "");
        Task<Option<OrderViewModel, string>> ChangeOrderCancelNote(int orderId, string note = "");

    }

    public class OrderService : BaseService<Orders>, IOrderService
    {
        private readonly IEmailService emailService;
        private readonly IProductService productService;
        private readonly IUserTransactionService userTransactionService;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userManager;

        public OrderService(EcommerceDbContext dbContext,
            IMapper mapper,
            ILogger<BaseService<Orders, EcommerceDbContext>> logger,
            IEmailService emailService,
            IProductService productService,
            EcommerceIdentityUserManager<EcommerceIdentityUser> userManager,
            IUserTransactionService userTransactionService) : base(dbContext, mapper, logger)
        {
            this.emailService = emailService;
            this.productService = productService;
            this.userManager = userManager;
            this.userTransactionService = userTransactionService;
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

        public async Task<Option<OrderViewModel, string>> CreateSaleOrderWithOptionAsync(CreateOrderWithOptionViewModel request)
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await request.SomeNotNull()
                    .WithException("Dữ liệu truyền lên rỗng.")
                    .Filter(d => d.UserId > 0, "Không tìm thấy thông tin người đặt hàng.")
                    .MapAsync(async d => (optionOrder: await MappingFromOrderWithOptionToSaleOrder(d), productVariants: d.OrderItems.Select(oi => oi.ProductVariant).ToImmutableList()))
                    .FlatMapAsync(async d =>
                    {
                        return await d.optionOrder.MapAsync(async o =>
                        {
                            var user = await userManager.FindByIdAsync(o.UserId.ToString());
                            var userTransaction = this.userTransactionService.PrepareUserTransaction(o, d.productVariants, user);
                            var result = await userTransactionService.UpdateUserBalanceAsync(userTransaction, user);
                            return result.Map(u =>
                            {
                                o.User = user;
                                return o;
                            });
                        }).FlattenAsync();
                    })
                    .MapAsync(async o =>
                    {
                        try
                        {
                            var user = await userManager.FindByIdAsync(o.UserId.ToString());
                            var messagePayloads = new List<MessagePayload>()
                            {
                                await emailService.PrepareLeaderOrderMailConfirmAsync(o.User, o),
                                await emailService.PrepareCustomerOrderMailConfirm(o.User, o)
                            };

                            var emailResult = await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"Process email for order {o.Code} failure.");
                        }

                        await this.CreateAsync(o);
                        await this.CommitAsync();
                        await transaction.CommitAsync();
                        return this.Mapper.Map<OrderViewModel>(o);
                    });
            }

        }

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
                    // prepare transaction
                    var user = await userManager.FindByIdAsync(order.UserId.ToString());
                    var userTransaction = this.userTransactionService.PrepareUserTransaction(order, ImmutableList<ProductVariantViewModel>.Empty, user, UserTransactionTypeEnum.Withdrawal);
                    return (order, userTransaction, user);
                })
                .FlatMapAsync(async d =>
                {
                    return (await userTransactionService.UpdateUserBalanceAsync(d.userTransaction, d.user))
                        .Map(updateBalanceRes =>
                        {
                            d.order.User = updateBalanceRes;
                            return d.order;
                        });
                })
                .MapAsync(async o =>
                {
                    try
                    {
                        var user = await userManager.FindByIdAsync(o.UserId.ToString());
                        var messagePayloads = new List<MessagePayload>()
                        {
                            await emailService.PrepareLeaderOrderMailConfirmAsync(o.User, o),
                            await emailService.PrepareCustomerOrderMailConfirm(o.User, o)
                        };

                        var emailResult = await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Process email for order {o.Code} failure.");
                    }

                    await this.CreateAsync(o);
                    await this.CommitAsync();
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

        private async Task<Option<Orders, string>> MappingFromOrderWithOptionToSaleOrder(CreateOrderWithOptionViewModel request)
        {
            return (await request.SomeNotNull()
                .WithException("Thông tin đơn hàng rỗng")
                .FlatMapAsync(async d =>
                {
                    var resultOrderItems = (await Task.WhenAll(d.OrderItems.Select(async orderItem =>
                    {
                        var productVariant = await productService.FindProductVariantFromOptionsAsync(orderItem.ProductOptionValues);
                        if (productVariant == null)
                        {
                            return Option.None<CreateOrderItemWithOptionViewModel, string>("Không tìm thấy sản phẩm mà bạn muốn đặt hàng.");
                        }
                        else if (productVariant.Status == ProductVariantStatusEnum.Unavailable)
                        {
                            return Option.None<CreateOrderItemWithOptionViewModel, string>("Sản phẩm mà bạn muốn đặt hàng đang bị khóa. Vui lòng thử lại vào khoản thời gian tiếp theo.");
                        }
                        else
                        {
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
                        }
                        return Option.Some<CreateOrderItemWithOptionViewModel, string>(orderItem);
                    }))).ToImmutableList();
                    if (resultOrderItems.Exceptions().Any()) return Option.None<CreateOrderWithOptionViewModel, string>(resultOrderItems.Exceptions().Aggregate((a, b) => $"{a}\n{b}"));
                    else d.OrderItems = resultOrderItems.Values().ToImmutableList();
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

                    var order = this.Mapper.Map<Orders>(request);
                    order.Code = DataUtils.GenerateCode(Constants.OrderInformation.ORDER_CODE_LENGTH);
                    order.CreatedDate = DateTime.UtcNow.ToVietnamDateTime();
                    order.Type = OrderTypeEnum.Sale;
                    order.Status = (int)OrderStatusEnum.New;
                    return order;
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

                var productVariantIds = await productService.GetProductVariantIdsFromProductFilterParamsAsync(productFilter);
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

        public async Task<Option<OrderViewModel, string>> CreateSummaryOrderAsync(CreateSummaryOrderViewModel summaryOrder, int userId)
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await (summaryOrder, userId).SomeNotNull()
                    .WithException(string.Empty)
                    .Filter(req => req.userId != 0, "Không tìm thấy thông tin người sử dụng tính năng tạo Đơn Tổng")
                    .Filter(req => req.summaryOrder != null && req.summaryOrder.Orders != null && req.summaryOrder.Orders.Count > 0, "Không tìm thấy dữ liệu để tạo Đơn Tổng")
                    .FlatMapAsync(async req =>
                    {
                        var data = req.summaryOrder.Orders;
                        // Get New Order Ids From Client
                        var newOIds = data.Where(o => o.Status == (int)OrderStatusEnum.New).Select(o => o.Id).ToImmutableList();
                        // Filter on Db
                        var newOEntities = (await this.GetAsTracking(dbO => newOIds.Contains(dbO.Id)
                            && dbO.Status == (int)OrderStatusEnum.New
                            && dbO.SummaryOrderId == null).ToListAsync()).Select(o =>
                        {
                            o.Status = (int)OrderStatusEnum.Processing;
                            return o;
                        }).ToImmutableList();
                        if (newOEntities.Count == 0) return Option.None<(ImmutableList<Orders> listNewDbOrders, int uId, string note), string>("Các đơn hàng bạn chọn, không có đơn hàng nào phù hợp để tạo Đơn Tổng.");
                        return Option.Some<(ImmutableList<Orders> listNewDbOrders, int uId, string note), string>((listNewDbOrders: newOEntities, uId: req.userId, req.summaryOrder.Note));
                    })
                    .MapAsync(async transformedData =>
                    {
                        var summaryOrder = new Orders()
                        {
                            UserId = transformedData.uId,
                            Code = DataUtils.GenerateCode(Constants.OrderInformation.ORDER_CODE_LENGTH),
                            CreatedDate = DateTime.UtcNow.ToVietnamDateTime(),
                            FinalPrice = transformedData.listNewDbOrders.Sum(newDbO => newDbO.FinalPrice),
                            TotalPrice = transformedData.listNewDbOrders.Sum(newDbO => newDbO.TotalPrice),
                            TotalQuantity = transformedData.listNewDbOrders.Sum(newDbO => newDbO.TotalQuantity),
                            Status = (int)OrderStatusEnum.Processing,
                            Note = transformedData.note,
                            Type = OrderTypeEnum.Summary,
                            InverseSummaryOrders = transformedData.listNewDbOrders
                        };
                        await this.CreateAsync(summaryOrder);
                        await this.CommitAsync();
                        await transaction.CommitAsync();
                        return this.Mapper.Map<OrderViewModel>(summaryOrder);
                    });
            }
        }

        public Task<Option<ExportOrderToExcel, string>> GetSummaryOrderDetailStyleAsExcelBytesAsync(int summaryOrderId)
        {
            Expression<Func<Orders, bool>> filterSummaryOrderWithId = (o) => o.Id == summaryOrderId && o.Type == OrderTypeEnum.Summary && o.Status == (int)OrderStatusEnum.Processing;
            return summaryOrderId.SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .Filter(req => req != 0, "Không tìm thấy mã của Đơn Tổng")
                .FilterAsync(async req => await this.AnyAsync(filterSummaryOrderWithId), "Không tìm thấy bất kỳ Đơn Hàng Tổng nào với mã này.")
                .MapAsync(async id =>
                {
                    var summaryOrder = await this.GetAsNoTracking(filterSummaryOrderWithId)
                        .Include(o => o.InverseSummaryOrders).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant).ThenInclude(pv => pv.ProductVariantOptionValues).ThenInclude(pvov => pvov.ProductOptionValue).ThenInclude(pov => pov.ProductOption)
                        .Include(o => o.InverseSummaryOrders).ThenInclude(o => o.User)
                        .Include(o => o.InverseSummaryOrders).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    using (var package = new ExcelPackage())
                    {
                        var sheet = package.Workbook.Worksheets.Add(GetNormalizedOfType(OrderTypeEnum.Summary));
                        var currentRowIndex = 1;
                        sheet.Cells[currentRowIndex, 1].Value = $"Mã {GetNormalizedOfType(OrderTypeEnum.Summary)}";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.Code;
                        sheet.Cells[currentRowIndex, 1].Value = "Tổng tiền chưa chiết khấu";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.TotalPrice.GetVietnamDong();
                        sheet.Cells[currentRowIndex, 1].Value = "Tổng tiền đã chiết khẩu";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.FinalPrice.GetVietnamDong();
                        sheet.Cells[currentRowIndex, 1].Value = $"Ngày tạo";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.CreatedDate.ToString(Constants.DateTimeFormat.Default);
                        sheet.Cells[currentRowIndex, 1].Value = $"Người tạo đơn tổng";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.User.UserName;

                        var excelRangeTitle = sheet.Cells[1, 1, 3, 2];
                        excelRangeTitle.Style.Font.Bold = true;
                        excelRangeTitle.Style.Font.Size = 14;
                        excelRangeTitle.AutoFitColumns();

                        if (summaryOrder != null && summaryOrder.InverseSummaryOrders != null && summaryOrder.InverseSummaryOrders.Count > 0)
                        {
                            foreach (var o in summaryOrder.InverseSummaryOrders)
                            {
                                // draw current summary order
                                var firstRowIndex = currentRowIndex;
                                var headerColumnIndex = 1;
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Mã Đơn";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Loại đơn hàng";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Trạng thái";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Số tiền tạm tính";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Giảm giá trên đơn hàng";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Thông tin thêm";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Số tiền thực tế";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Số điện thoại";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Người đặt hàng";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Ngày tạo";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Ghi chú";
                                sheet.Cells[currentRowIndex, headerColumnIndex].Value = "Lý do hủy";
                                var excelRangeHeader = sheet.Cells[currentRowIndex, 1, currentRowIndex, headerColumnIndex];
                                excelRangeHeader.Style.Font.Bold = true;
                                excelRangeHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                                ++currentRowIndex;
                                var dataColumnIndex = 1;
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.Code;
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = GetNormalizedOfType(o.Type);
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = GetNormalizedOfStatus((OrderStatusEnum)o.Status);
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.TotalPrice.GetVietnamDong();
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = $"{o.Discount}%";
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = GetNormalizedDynamicDescriptionOrder(o);
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.FinalPrice.GetVietnamDong();
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.DeliveryPhone;
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.User.Fullname;
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.CreatedDate.ToString(Constants.DateTimeFormat.Default);
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.Note;
                                sheet.Cells[currentRowIndex, dataColumnIndex++].Value = o.CancelNote;

                                var excelRangeAll = sheet.Cells[firstRowIndex, 1, currentRowIndex, headerColumnIndex];
                                excelRangeAll.AutoFitColumns();
                                excelRangeAll.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                excelRangeAll.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                excelRangeAll.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                excelRangeAll.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                                // draw inverse summary orders
                                ++currentRowIndex;
                                firstRowIndex = currentRowIndex;
                                headerColumnIndex = 1;
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Tên sản phẩm";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Sku";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Đặc tính sản phẩm";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Giá trị sản phẩm";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Giảm giá trên sản phẩm";
                                sheet.Cells[currentRowIndex, headerColumnIndex++].Value = "Số lượng";
                                sheet.Cells[currentRowIndex, headerColumnIndex].Value = "Số tiền thực tế";
                                excelRangeHeader = sheet.Cells[currentRowIndex, 1, currentRowIndex, headerColumnIndex];
                                excelRangeHeader.Style.Font.Bold = true;
                                excelRangeHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                foreach (var item in o.OrderItems)
                                {
                                    ++currentRowIndex;
                                    dataColumnIndex = 1;
                                    sheet.Cells[currentRowIndex, dataColumnIndex++].Value = item.Product.Name;
                                    sheet.Cells[currentRowIndex, dataColumnIndex++].Value = item.ProductVariant?.Sku;
                                    sheet.Cells[currentRowIndex, dataColumnIndex++].Value = GetNormalizedOptionValueDescriptionOrder(item);
                                    sheet.Cells[currentRowIndex, dataColumnIndex++].Value = item.SubTotalPrice.GetVietnamDong();
                                    sheet.Cells[currentRowIndex, dataColumnIndex++].Value = $"{item.Discount}%";
                                    sheet.Cells[currentRowIndex, dataColumnIndex++].Value = item.SubTotalQuantity;
                                    sheet.Cells[currentRowIndex, dataColumnIndex].Value = item.SubTotalFinalPrice.GetVietnamDong();
                                }
                                excelRangeAll = sheet.Cells[firstRowIndex, 1, currentRowIndex, headerColumnIndex];
                                excelRangeAll.AutoFitColumns();
                                excelRangeAll.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                excelRangeAll.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                excelRangeAll.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                excelRangeAll.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                currentRowIndex += 2;
                            }
                        }
                        return ExportOrderToExcel.CreateInstance(package.GetAsByteArray(), $"Phone-{summaryOrder.CreatedDate.ToString("dd_MM_yyyy")}-{summaryOrder.Code}.xlsx");
                    }
                });
        }

        public Task<Option<ExportOrderToExcel, string>> GetSummaryOrderPhoneCardWebsiteStyleAsExcelBytesAsync(int summaryOrderId)
        {
            Expression<Func<Orders, bool>> filterSummaryOrderWithId = (o) => o.Id == summaryOrderId && o.Type == OrderTypeEnum.Summary;
            return summaryOrderId.SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .Filter(req => req != 0, "Không tìm thấy mã của Đơn Tổng")
                .FilterAsync(async req => await this.AnyAsync(filterSummaryOrderWithId), "Không tìm thấy bất kỳ Đơn Hàng Tổng nào với mã này.")
                .MapAsync(async id =>
                {
                    var summaryOrder = await this.GetAsNoTracking(filterSummaryOrderWithId)
                        .Include(o => o.User)
                        .Include(o => o.InverseSummaryOrders).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant).ThenInclude(pv => pv.ProductVariantOptionValues).ThenInclude(pvov => pvov.ProductOptionValue).ThenInclude(pov => pov.ProductOption)
                        .Include(o => o.InverseSummaryOrders).ThenInclude(o => o.User)
                        .Include(o => o.InverseSummaryOrders).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    using (var package = new ExcelPackage())
                    {
                        var sheet = package.Workbook.Worksheets.Add(GetNormalizedOfType(OrderTypeEnum.Summary));
                        var currentRowIndex = 1;
                        sheet.Cells[currentRowIndex, 1].Value = $"Mã {GetNormalizedOfType(OrderTypeEnum.Summary)}";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.Code;
                        sheet.Cells[currentRowIndex, 1].Value = "Tổng tiền chưa chiết khấu";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.TotalPrice.GetVietnamDong();
                        sheet.Cells[currentRowIndex, 1].Value = "Tổng tiền đã chiết khẩu";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.FinalPrice.GetVietnamDong();
                        sheet.Cells[currentRowIndex, 1].Value = $"Ngày tạo";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.CreatedDate.ToString(Constants.DateTimeFormat.Default);
                        sheet.Cells[currentRowIndex, 1].Value = $"Người tạo đơn tổng";
                        sheet.Cells[currentRowIndex++, 2].Value = summaryOrder.User.UserName;

                        // Beautify order summary titles
                        var excelRangeOrderSummaryTitles = sheet.Cells[1, 1, 5, 2];
                        excelRangeOrderSummaryTitles.Style.Font.Bold = true;
                        excelRangeOrderSummaryTitles.Style.Font.Size = 14;

                        if (summaryOrder != null && summaryOrder.InverseSummaryOrders != null && summaryOrder.InverseSummaryOrders.Count > 0)
                        {
                            // draw current summary order
                            var firstRowIndex = currentRowIndex;
                            var lastInverseOrderTblColumnIndex = 1;
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "STT";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Mã Đơn";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Trạng thái";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Tên tài khoản";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Loại tài khoản";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Thông tin nạp tiền";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Số tiền cần nạp";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Số lượng";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Thành tiền";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Tiền thực trả";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Ngày tạo";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex++].Value = "Ghi chú";
                            sheet.Cells[currentRowIndex, lastInverseOrderTblColumnIndex].Value = "Mã app";

                            var excelRangeHeader = sheet.Cells[currentRowIndex, 1, currentRowIndex, lastInverseOrderTblColumnIndex];
                            excelRangeHeader.Style.Font.Bold = true;
                            excelRangeHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                            var count = 0;
                            foreach (var o in summaryOrder.InverseSummaryOrders)
                            {
                                ++currentRowIndex;
                                var lastCurrentRowColumnIndex = 1;
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = (++count);
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.Code;
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = GetNormalizedOfStatus((OrderStatusEnum)o.Status);
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.User.UserName;
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = GetSkusAsString(o.OrderItems.ToList());
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.DeliveryAddress;
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = (o.TotalPrice / o.TotalQuantity).GetVietnamDong();
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.TotalQuantity;
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.TotalPrice.GetVietnamDong();
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.FinalPrice.GetVietnamDong();
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.CreatedDate.ToString(Constants.DateTimeFormat.Default);
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.Note;
                                sheet.Cells[currentRowIndex, lastCurrentRowColumnIndex++].Value = o.Dynamic03;
                            }

                            var excelRangeAll = sheet.Cells[firstRowIndex, 1, currentRowIndex, lastInverseOrderTblColumnIndex];
                            excelRangeAll.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            excelRangeAll.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            excelRangeAll.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            excelRangeAll.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            excelRangeAll = sheet.Cells[firstRowIndex, 1, currentRowIndex, lastInverseOrderTblColumnIndex];

                            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
                        }
                        return ExportOrderToExcel.CreateInstance(package.GetAsByteArray(), $"Phone-{summaryOrder.CreatedDate.ToString("dd_MM_yyyy")}-{summaryOrder.Code}.xlsx");
                    }
                });
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
                    var user = await userManager.FindByIdAsync(o.UserId.ToString());
                    var userTransaction = this.userTransactionService.PrepareUserTransaction(o, ImmutableList<ProductVariantViewModel>.Empty, user, UserTransactionTypeEnum.Income);
                    var updateBalanceResult = await userTransactionService.UpdateUserBalanceAsync(userTransaction, user);
                    return updateBalanceResult.Match<Option<Orders, string>>(
                        res =>
                        {
                            o.User = user;
                            return Option.Some<Orders, string>(o);
                        },
                        error => { return Option.None<Orders, string>(error); }
                    );
                })
                .MapAsync(async o =>
                {
                    try
                    {
                        var user = await userManager.FindByIdAsync(o.UserId.ToString());
                        var messagePayloads = new List<MessagePayload>()
                        {
                            await emailService.PrepareLeaderOrderMailConfirmAsync(o.User, o),
                            await emailService.PrepareCustomerOrderMailConfirm(o.User, o)
                        };

                        var emailResult = await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Process email for order {o.Code} failure.");
                    }

                    await this.CreateAsync(o);
                    await this.CommitAsync();
                    return this.Mapper.Map<OrderViewModel>(o);
                });
        }

        #region Status Order
        public async Task<Option<OrderViewModel, string>> CompleteSummaryOrderAsync(int summaryOrderId)
        {
            return await (summaryOrderId).SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .MapAsync(async req =>
                {
                    var summaryOrder = await this.GetAsTracking(o => o.Id == req && OrderTypeEnum.Summary == o.Type)
                        .Include(o => o.InverseSummaryOrders)
                        .FirstOrDefaultAsync();
                    summaryOrder.Status = (int)OrderStatusEnum.Done;
                    this.Update(summaryOrder);

                    var updatedInverseOrders = summaryOrder.InverseSummaryOrders
                        .Where(inverseOrder => inverseOrder.Status != (int)OrderStatusEnum.Cancel)
                        .Select(io => { io.Status = (int)OrderStatusEnum.Done; return io; })
                        .ToImmutableList();
                    this.UpdateRange(updatedInverseOrders);

                    await this.CommitAsync();
                    return this.Mapper.Map<OrderViewModel>(summaryOrder);
                });
        }

        public async Task<Option<OrderViewModel, string>> CancelSummaryOrderAsync(int summaryOrderId, int auditUserId, string cancelNote = "")
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await (summaryOrderId).SomeNotNull()
                    .WithException("Mã của Đơn Tổng không hợp lệ.")
                    .MapAsync(async req =>
                    {
                        var summaryOrder = await this.GetAsTracking(o => o.Id == req && o.Type == OrderTypeEnum.Summary)
                            .FirstOrDefaultAsync();
                        summaryOrder.Status = (int)OrderStatusEnum.Cancel;
                        summaryOrder.CancelNote = $"{cancelNote}";
                        this.Update(summaryOrder);

                        var listUpdatingInverseOrderCode = (await this.GetAsNoTracking(o => o.SummaryOrderId == summaryOrder.Id && o.Type == OrderTypeEnum.Sale)
                            .Where(inverseOrder => inverseOrder.Status != (int)OrderStatusEnum.Done)
                            .Select(io => io.Code)
                            .ToListAsync())
                            .ToImmutableList();

                        foreach (var updatingOrderCode in listUpdatingInverseOrderCode)
                        {
                            await CancelOrderInternalAsync(updatingOrderCode, auditUserId, cancelNote);
                        }

                        await this.CommitAsync();
                        await transaction.CommitAsync();
                        return this.Mapper.Map<OrderViewModel>(summaryOrder);
                    });
            }
        }

        public async Task<Option<OrderViewModel, string>> CancelOrderAsync(string orderCode, int auditUserId, string cancelNote = "")
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                return await CancelOrderInternalAsync(orderCode, auditUserId, cancelNote)
                    .MapAsync(async res =>
                    {
                        await this.CommitAsync();
                        await transaction.CommitAsync();
                        return res;
                    });
            }

        }

        /// <summary>
        /// You should commit when using this method.
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="auditUserId"></param>
        /// <param name="cancelNote"></param>
        /// <returns></returns>
        private async Task<Option<OrderViewModel, string>> CancelOrderInternalAsync(string orderCode, int auditUserId, string cancelNote = "")
        {
            return await (orderCode, auditUserId).SomeNotNull()
                .WithException(string.Empty)
                .Filter(d => !d.orderCode.IsNullOrEmpty(), "Mã đơn hàng gửi lên rỗng.")
                .FlatMapAsync(async d =>
                {
                    var order = await Get(o => o.Code == d.orderCode && OrderTypeEnum.Summary != o.Type).FirstOrDefaultAsync();
                    var auditUser = await userManager.FindByIdAsync(auditUserId.ToString());
                    if (auditUser == null)
                    {
                        return Option.None<OrderViewModel, string>("Tài khoản đang thao tác hủy đơn hàng không tồn tại hoặc bị xóa.");
                    }
                    var isInRoleAdmin = await userManager.IsInRoleAsync(auditUser, Constants.UserRoles.ADMIN);
                    if (order == null)
                    {
                        return Option.None<OrderViewModel, string>("Không tìm thấy đơn hàng phù hợp để hủy đơn.");
                    }
                    else if (!isInRoleAdmin && (OrderStatusEnum)order.Status != OrderStatusEnum.New)
                    {
                        return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} không phải là đơn hàng MỚI nên không thể hủy.");
                    }
                    else if ((OrderStatusEnum)order.Status == OrderStatusEnum.Done)
                    {
                        return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} đã HOÀN THÀNH nên không thể hủy.");
                    }
                    else if ((OrderStatusEnum)order.Status == OrderStatusEnum.Cancel)
                    {
                        return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} đã được HỦY nên không thể hủy.");
                    }
                    else
                    {
                        order.Status = (int)OrderStatusEnum.Cancel;
                        order.CancelNote = $"{cancelNote}.";
                        this.Update(order);

                        var orderOwner = await userManager.FindByIdAsync(order.UserId.ToString());
                        var userTransaction = this.userTransactionService.PrepareUserTransaction(order, ImmutableList<ProductVariantViewModel>.Empty, orderOwner, UserTransactionTypeEnum.Rollback);
                        await this.userTransactionService.UpdateUserBalanceAsync(userTransaction, orderOwner);
                        await userTransactionService.CreateAsync(userTransaction);
                        return Option.Some<OrderViewModel, string>(Mapper.Map<OrderViewModel>(order));
                    }
                });
        }

        public async Task<Option<OrderViewModel, string>> CompleteOrderAsync(string orderCode, int userId)
        {
            return await (orderCode).SomeNotNull()
                .WithException("Mã của Đơn Tổng không hợp lệ.")
                .FlatMapAsync(async req =>
                {
                    var order = await this.GetAsTracking(o => o.Code == orderCode && OrderTypeEnum.Summary != o.Type).Where(o => o.Status != (int)OrderStatusEnum.Cancel).FirstOrDefaultAsync();
                    var auditUser = await userManager.FindByIdAsync(userId.ToString());
                    if (auditUser == null)
                    {
                        return Option.None<OrderViewModel, string>("Tài khoản đang thao tác hủy đơn hàng không tồn tại hoặc bị xóa.");
                    }
                    var isInRoleAdmin = await userManager.IsInRoleAsync(auditUser, Constants.UserRoles.ADMIN);
                    if (order == null) return Option.None<OrderViewModel, string>("Không tìm thấy đơn hàng phù hợp");
                    else if (!isInRoleAdmin) return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} không thể hoàn thành do người thao tác không phải là Admin.");
                    else if (order.Status == (int)OrderStatusEnum.Cancel) return Option.None<OrderViewModel, string>($"Đơn hàng {orderCode} đã HỦY nên không thể hoàn thành.");
                    else
                    {
                        order.Status = (int)OrderStatusEnum.Done;
                        this.Update(order);
                        await this.CommitAsync();
                        return Option.Some<OrderViewModel, string>(this.Mapper.Map<OrderViewModel>(order));
                    }
                });
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderStatus(int orderId, OrderStatusEnum statusEnum, int auditUserId, string note = "")
        {
            return await (orderId, statusEnum).SomeNotNull()
                .WithException(string.Empty)
                .FilterAsync(async d => await this.SelfRepository.AnyAsync(x => x.Id == d.orderId), "Không thể tim thấy đơn hàng cần đổi trạng thái")
                .FlatMapAsync(async d =>
                {
                    var orderCode = await this.Get(o => o.Id == orderId).Select(o => o.Code).FirstOrDefaultAsync();
                    switch (statusEnum)
                    {
                        case OrderStatusEnum.Cancel:
                            return await CancelOrderAsync(orderCode, auditUserId, note);
                        case OrderStatusEnum.Done:
                            return await CompleteOrderAsync(orderCode, auditUserId);
                        default:
                            return Option.None<OrderViewModel, string>("Không thể chuyển đơn hàng sang trạng thái này.");
                    }
                });
        }
        #endregion

        #region Utilization 

        private string GetSkusAsString(IList<OrderItems> listOi)
        {
            if (listOi == null || listOi.Count == 0) return "";
            return listOi.Select(oi => oi.ProductVariant.Sku).Aggregate((x, y) => $"{x}\n{y}");
        }

        private string GetNormalizedOfType(OrderTypeEnum typeEnum)
        {
            return typeEnum
            switch
            {
                OrderTypeEnum.Summary => "Hóa đơn tổng",
                OrderTypeEnum.Sale => "Hóa đơn hàng bán",
                OrderTypeEnum.Desposit => "Hóa đơn nạp tiền",
                _ => "Hóa đơn rút tiền"
            };
        }

        private string GetNormalizedOfStatus(OrderStatusEnum statusEnum)
        {
            return statusEnum
            switch
            {
                OrderStatusEnum.New => "Đang chờ nạp",
                OrderStatusEnum.Processing => "Đang xử lý",
                OrderStatusEnum.Cancel => "Đã hủy",
                OrderStatusEnum.Done => "Hoàn thành",
                _ => "Thất bại"
            };
        }

        private string GetNormalizedDynamicDescriptionOrder(Orders order)
        {
            if (order == null) return string.Empty;
            else
            {
                var messages = new List<string>();
                if (!order.Dynamic01.IsNullOrEmpty()) messages.Add($"{order.Dynamic01}");
                if (!order.Dynamic02.IsNullOrEmpty()) messages.Add($"{order.Dynamic02}");
                if (!order.Dynamic03.IsNullOrEmpty()) messages.Add($"{order.Dynamic03}");
                if (!order.Dynamic04.IsNullOrEmpty()) messages.Add($"{order.Dynamic04}");
                if (!order.Dynamic05.IsNullOrEmpty()) messages.Add($"{order.Dynamic05}");
                return messages.Count > 0 ? messages.Aggregate((x, y) => $"{x}, {y}") : string.Empty;
            }
        }

        private string GetNormalizedOptionValueDescriptionOrder(OrderItems item)
        {
            if (item.ProductVariant?.ProductVariantOptionValues != null
                    && item.ProductVariant?.ProductVariantOptionValues?.Count > 0)
            {
                var optionValues = new List<string>();
                foreach (var ov in item.ProductVariant.ProductVariantOptionValues)
                {
                    optionValues.Add($"{ov.ProductOptionValue.ProductOption.Name}: {ov.ProductOptionValue.Value}");
                }
                return optionValues.Count > 0 ? optionValues.Aggregate((x, y) => $"{x}\n {y}") : string.Empty;
            }
            return string.Empty;
        }


        #endregion
        public async Task<Option<OrderViewModel, string>> ChangeOrderNote(int orderId, string note = "")
        {
            return await (orderId, note)
                .SomeNotNull()
                .WithException(string.Empty)
                .MapAsync(async data =>
                {
                    var order = await this.FindByKeysAsync(orderId);
                    order.Note = note;
                    return this.Mapper.Map<OrderViewModel>(order);
                });
        }

        public async Task<Option<OrderViewModel, string>> ChangeOrderCancelNote(int orderId, string note = "")
        {
            return await (orderId, note)
                .SomeNotNull()
                .WithException(string.Empty)
                .MapAsync(async data =>
                {
                    var order = await this.FindByKeysAsync(orderId);
                    order.CancelNote = note;
                    return this.Mapper.Map<OrderViewModel>(order);
                });
        }
    }
}