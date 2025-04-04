export const ORDER_STATUS = {
  PendingConfirmation: "PendingConfirmation", // Chờ xác nhận
  Confirmed: "Confirmed",                     // Đã xác nhận
  Rejected: "Rejected",                       // Bị từ chối
  Paid: "Paid",                              // Đã thanh toán
  Packed: "Packed",                          // Đã đóng gói
  Shipping: "Shipping",                      // Đang vận chuyển
  Delivered: "Delivered",                    // Đã giao hàng
  Completed: "Completed",                    // Hoàn thành
  Cancelled: "Cancelled",                    // Đã hủy
  DeliveryFailed: "DeliveryFailed",         // Giao hàng thất bại
  ReturnProcessing: "ReturnProcessing",      // Đang xử lý hoàn trả
  Returned: "Returned"                       // Đã hoàn trả
} as const;

export const ORDER_STATUS_LABELS: Record<string, string> = {
  PendingConfirmation: "Chờ xác nhận",
  Confirmed: "Đã xác nhận",
  Rejected: "Bị từ chối",
  Paid: "Đã thanh toán",
  Packed: "Đã đóng gói",
  Shipping: "Đang vận chuyển",
  Delivered: "Đã giao hàng",
  Completed: "Hoàn thành",
  Cancelled: "Đã hủy",
  DeliveryFailed: "Giao hàng thất bại",
  ReturnProcessing: "Đang xử lý hoàn trả",
  Returned: "Đã hoàn trả"
}; 