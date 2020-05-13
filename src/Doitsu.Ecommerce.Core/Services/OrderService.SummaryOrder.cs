using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Service.Core;
using Doitsu.Utils;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.Core.Services
{
    public partial class OrderService
    {
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

    }
}