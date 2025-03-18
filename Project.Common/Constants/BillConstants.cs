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
        public const string StatusPending = "StatusPending";
        public const string StatusConfirmed = "StatusConfirmed";
        public const string StatusProcessing = "StatusProcessing";
        public const string StatusShipping = "StatusShipping";
        public const string StatusDelivered = "StatusDelivered";
        public const string StatusCancelled = "StatusCancelled";

        // Trạng thái thanh toán
        public const string PaymentStatusUnpaid = "PaymentStatusUnpaid";
        public const string PaymentStatusPaid = "PaymentStatusPaid";
        public const string PaymentStatusRefunded = "PaymentStatusRefunded";
    }
}
