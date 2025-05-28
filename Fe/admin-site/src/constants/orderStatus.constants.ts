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
  Returned: "Returned",                      // Đã hoàn trả
  OutOfStock: "OutOfStock"                   // Không đủ hàng
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
  Returned: "Đã hoàn trả",
  OutOfStock: "Không đủ hàng"
};

// Ma trận chuyển đổi trạng thái
export const STATUS_TRANSITIONS: Record<string, string[]> = {
  PendingConfirmation: ["Confirmed", "Rejected", "Cancelled", "OutOfStock"],
  Confirmed: ["Paid", "Cancelled"],
  Paid: ["Packed", "Cancelled"],
  Packed: ["Shipping", "Cancelled"],
  Shipping: ["Delivered", "DeliveryFailed"],
  DeliveryFailed: ["Shipping", "Cancelled"],
  Delivered: ["Completed", "ReturnProcessing"],
  ReturnProcessing: ["Returned"],
  Returned: [], // Trạng thái kết thúc
  Completed: [], // Trạng thái kết thúc
  Cancelled: [], // Trạng thái kết thúc
  Rejected: [], // Trạng thái kết thúc
  OutOfStock: ["Cancelled"] // Có thể hủy đơn khi hết hàng
}; 