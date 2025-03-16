namespace Project.Common;

public static class BillConstant
{
    public const string BillNotFound = "Không tìm thấy hóa đơn";
    public const string BillDetailNotFound = "Không tìm thấy chi tiết hóa đơn";
    
    // Trạng thái đơn hàng
    public const int StatusPending = 0;
    public const int StatusConfirmed = 1;
    public const int StatusProcessing = 2;
    public const int StatusShipping = 3;
    public const int StatusDelivered = 4;
    public const int StatusCancelled = 5;
    
    // Trạng thái thanh toán
    public const int PaymentStatusUnpaid = 0;
    public const int PaymentStatusPaid = 1;
    public const int PaymentStatusRefunded = 2;
}