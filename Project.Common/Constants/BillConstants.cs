using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common.Constants
{
    public static class BillConstants
    {
        public const string BillNotFound = "Không tìm thấy hóa đơn";
        public const string BillDetailNotFound = "Không tìm thấy chi tiết hóa đơn";

        // Trạng thái đơn hàng
        public const string PendingConfirmation = "PendingConfirmation"; // Chờ xác nhận
        public const string Confirmed = "Confirmed";               // Đã xác nhận
        public const string Rejected = "Rejected";                      // Bị từ chối
        public const string OutOfStock = "OutOfStock";            //Hết hàng
        public const string Paid = "Paid";                                      // Đã thanh toán
        public const string Packed = "Packed";                            // Đã đóng gói
        public const string Shipping = "Shipping";                      // Đang vận chuyển
        public const string Delivered = "Delivered";                   // Đã giao hàng
        public const string Completed = "Completed";              // Hoàn thành
        public const string Cancelled = "Cancelled";                  // Đã hủy
        public const string DeliveryFailed = "DeliveryFailed";           // Giao hàng thất bại
        public const string ReturnProcessing = "ReturnProcessing";       // Đang xử lý hoàn trả
        public const string Returned = "Returned";                       // Đã hoàn trả

     


        // Trạng thái thanh toán
        public const string PaymentStatusUnpaid = "PaymentStatusUnpaid";
        public const string PaymentStatusPaid = "PaymentStatusPaid";
        public const string PaymentStatusRefunded = "PaymentStatusRefunded";
    }
}
