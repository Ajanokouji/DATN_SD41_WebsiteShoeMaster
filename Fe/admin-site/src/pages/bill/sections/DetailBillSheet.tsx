import React, { useEffect, useState } from "react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
} from "@/components/ui/sheet";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectBill } from "@/redux/apps/bill/billSelector";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { fetchBillById, updateBill, checkInventory } from "@/redux/apps/bill/billSlice";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Button } from "@/components/ui/button";
import { Printer, Download } from "lucide-react";
import { formatVietnamTime } from "@/utils/format";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ORDER_STATUS, ORDER_STATUS_LABELS, STATUS_TRANSITIONS } from "@/constants/orderStatus.constants";
import { toast } from "sonner";
import { InventoryCheckDetail, BillDetailItem } from "@/types/bill/bill";

interface DetailBillSheetProps {
  billId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailBillSheet: React.FC<DetailBillSheetProps> = ({
  billId,
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const bill = useAppSelector(selectBill);
  const [selectedStatus, setSelectedStatus] = useState<string>("");
  const [inventoryCheck, setInventoryCheck] = useState<InventoryCheckDetail[]>([]);

  useEffect(() => {
    if (bill?.status) {
      setSelectedStatus(bill.status);
    }
  }, [bill]);

  // Xử lý khi load chi tiết hóa đơn
  useEffect(() => {
    const loadBillDetails = async () => {
      try {
        const result = await dispatch(fetchBillById(billId)).unwrap();

        // Nếu trạng thái là PendingConfirmation HOẶC OutOfStock, kiểm tra kho
        if (result.status === ORDER_STATUS.PendingConfirmation || result.status === ORDER_STATUS.OutOfStock) {
          // inventoryResult là mảng InventoryCheckDetail[] trực tiếp
          const inventoryResult: InventoryCheckDetail[] = await dispatch(checkInventory(billId)).unwrap();

          // Gán trực tiếp mảng vào state
          setInventoryCheck(inventoryResult || []);

          // Kiểm tra nếu có sản phẩm không đủ hàng (dựa vào mảng inventoryResult)
          const outOfStockProducts = inventoryResult?.filter((detail: InventoryCheckDetail) => !detail.isAvailable) || [];

          // Chỉ hiển thị toast nếu đang ở PendingConfirmation ban đầu VÀ có sản phẩm thiếu hàng
          if (result.status === ORDER_STATUS.PendingConfirmation && outOfStockProducts.length > 0) {
              // Hiển thị chi tiết sản phẩm không đủ hàng
              const outOfStockMessage = outOfStockProducts
                .map((detail: InventoryCheckDetail) => `${detail.productName} (SKU: ${detail.sku}, Cần: ${detail.requestedQuantity}, Có: ${detail.availableQuantity})`)
                .join('\n');

              toast.error(`Không đủ số lượng trong kho:\n${outOfStockMessage}\n\nVui lòng cập nhật trạng thái sang "Không đủ hàng"`);
          }
        } else {
          // Nếu không ở trạng thái cần kiểm tra tồn kho, reset state kiểm tra tồn kho
          setInventoryCheck([]);
        }
      } catch (error) {
        console.error('Error loading bill details:', error);
        toast.error('Có lỗi xảy ra khi tải chi tiết hóa đơn');
        // Reset state kiểm tra tồn kho khi có lỗi
        setInventoryCheck([]);
      }
    };

    if (isOpen) {
      loadBillDetails();
    }
  }, [dispatch, billId, isOpen]);

  // Cập nhật trạng thái hóa đơn
  const updateBillStatus = async (newStatus: string) => {
    try {
      await dispatch(updateBill({
        id: bill!.id,
        data: {
          id: bill!.id,
          status: newStatus
        }
      }));
      // Refresh lại dữ liệu
      dispatch(fetchBillById(billId));
      toast.success('Cập nhật trạng thái thành công');
    } catch (error) {
      console.error("Failed to update status:", error);
      toast.error('Cập nhật trạng thái thất bại');
    }
  };

  // Lấy danh sách trạng thái có thể chuyển đổi
  const getAvailableStatuses = (currentStatus: string) => {
    // Kiểm tra xem có sản phẩm nào thiếu hàng trong state inventoryCheck không
    // Nếu inventoryCheck là null hoặc undefined, coi như không có sản phẩm thiếu hàng
    const hasOutOfStockItems = inventoryCheck?.some(item => !item.isAvailable) ?? false;

    // Nếu trạng thái hiện tại là OutOfStock, luôn cho phép chuyển về Đã xác nhận (Confirmed)
    if (currentStatus === ORDER_STATUS.OutOfStock) {
      return [ORDER_STATUS.Confirmed];
    }

    // Nếu có sản phẩm không đủ hàng VÀ trạng thái hiện tại không phải OutOfStock,
    // chỉ cho phép chuyển sang OutOfStock.
    // Điều này xử lý trường hợp đang ở PendingConfirmation mà kiểm tra kho thấy thiếu hàng.
    if (hasOutOfStockItems && currentStatus !== ORDER_STATUS.OutOfStock) {
        return [ORDER_STATUS.OutOfStock];
    }

    // Đối với các trường hợp khác, sử dụng các trạng thái chuyển đổi thông thường.
    return STATUS_TRANSITIONS[currentStatus] || [];
  };

  const handleUpdateStatus = async (newStatus: string) => {
    if (bill && newStatus) {
      try {
        await updateBillStatus(newStatus);
      } catch (error) {
        console.error("Failed to update status:", error);
      }
    }
  };

  const formatCurrency = (amount: number | null) => {
    if (amount === null) return "N/A";
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };

  if (!bill) {
    return (
      <Sheet open={isOpen} onOpenChange={onClose}>
        <SheetContent className="w-full sm:max-w-lg">
          <div className="flex items-center justify-center h-full">
            <p>Đang tải chi tiết hóa đơn...</p>
          </div>
        </SheetContent>
      </Sheet>
    );
  }

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[60vw] max-w-none h-screen overflow-y-auto p-0">
        <div className="bg-white min-h-screen flex flex-col">
          {/* Invoice Header */}
          <div className="bg-primary text-white p-6">
            <SheetHeader className="mb-4 flex justify-between items-start">
              <div>
                <h1 className="text-2xl font-bold mb-2">Hóa đơn</h1>
                <p className="text-sm opacity-90">Mã hóa đơn: {bill.billCode}</p>
                <div className="flex gap-2 mt-1">
                  
                </div>
              </div>
              <div className="flex gap-2">
                <Button
                  variant="secondary"
                  size="sm"
                  className="flex items-center gap-1"
                >
                  <Printer className="h-4 w-4" />
                  <span>In</span>
                </Button>
                <Button
                  variant="secondary"
                  size="sm"
                  className="flex items-center gap-1"
                >
                  <Download className="h-4 w-4" />
                  <span>Tải xuống</span>
                </Button>
              </div>
            </SheetHeader>
          </div>

          {/* Status Update Section */}
          <div className="px-6 py-4 border-t border-b">
            <div className="flex items-center gap-4">
              <div className="flex-1">
                <Select
                  value={selectedStatus}
                  onValueChange={(value) => setSelectedStatus(value)}
                  disabled={getAvailableStatuses(bill?.status || "").length === 0}
                >
                  <SelectTrigger className="w-[200px] text-gray-900 bg-white">
                    <SelectValue>
                      {selectedStatus ? ORDER_STATUS_LABELS[selectedStatus] : "Chọn trạng thái mới"}
                    </SelectValue>
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem 
                      key={bill?.status} 
                      value={bill?.status || ""}
                      className="text-gray-900 hover:bg-gray-100"
                    >
                      {ORDER_STATUS_LABELS[bill?.status || ""]} (Hiện tại)
                    </SelectItem>
                    
                    {getAvailableStatuses(bill?.status || "")
                      .filter(status => status !== bill?.status)
                      .map((status) => (
                        <SelectItem 
                          key={status} 
                          value={status}
                          className="text-gray-900 hover:bg-gray-100"
                        >
                          {ORDER_STATUS_LABELS[status]}
                        </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <Button
                onClick={() => handleUpdateStatus(selectedStatus)}
                disabled={!selectedStatus || selectedStatus === bill?.status || 
                         !getAvailableStatuses(bill?.status || "").includes(selectedStatus)}
              >
                Cập nhật trạng thái
              </Button>
            </div>
            {getAvailableStatuses(bill?.status || "").length === 0 && (
              <p className="text-sm text-muted-foreground mt-2">
                Đơn hàng đã ở trạng thái kết thúc, không thể thay đổi trạng thái.
              </p>
            )}
          </div>

          {/* Company & Invoice Info */}
          <div className="p-6 bg-white">
            <div className="grid grid-cols-2 gap-6 mb-8">
              <div>
                <h2 className="text-lg font-semibold mb-1">Người Nhận</h2>
                <div className="text-gray-800">
                  <p className="font-medium">{bill.recipientName}</p>
                  <p className="text-sm text-gray-600">
                    {bill.recipientAddress}<br />
                    {bill.recipientEmail}<br />
                    {bill.recipientPhone}
                  </p>
                </div>
              </div>
            </div>
            
            <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-8 bg-gray-50 p-4 rounded-lg">
              <div>
                <p className="text-sm text-gray-500">Mã Hóa Đơn</p>
                <p className="font-medium">{bill.billCode}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Ngày Tạo</p>
                <p className="font-medium">{formatVietnamTime(bill.createdOnDate)?.split(' ')[0]}</p>
              </div>
             
            </div>

            {/* Bill Items */}
            <div className="mb-8">
              <h2 className="text-lg font-semibold mb-3">Chi tiết hóa đơn</h2>
              <div className="overflow-x-auto">
                <table className="w-full border-collapse">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="py-3 px-4 text-left text-sm font-semibold text-gray-700 border-b">Sản Phẩm</th>
                      <th className="py-3 px-4 text-left text-sm font-semibold text-gray-700 border-b">SKU</th>
                      <th className="py-3 px-4 text-left text-sm font-semibold text-gray-700 border-b">Kích cỡ</th>
                      <th className="py-3 px-4 text-left text-sm font-semibold text-gray-700 border-b">Màu sắc</th>
                      <th className="py-3 px-4 text-right text-sm font-semibold text-gray-700 border-b">Số lượng</th>
                      <th className="py-3 px-4 text-right text-sm font-semibold text-gray-700 border-b">Đơn giá</th>
                      <th className="py-3 px-4 text-right text-sm font-semibold text-gray-700 border-b">Tổng</th>
                      <th className="py-3 px-4 text-center text-sm font-semibold text-gray-700 border-b">Trạng thái</th>
                    </tr>
                  </thead>
                  <tbody>
                    {bill.billDetails?.map((detail: BillDetailItem, index: number) => {
                      const inventoryItem = inventoryCheck.find(item => item.productId === detail.productId && item.sku === detail.sku);
              
                      return (
                        <tr key={index}>
                          <td className="py-3 px-4 text-left border-b text-gray-800">
                            <div className="flex items-center gap-2">
                              <img 
                                src={detail.productImage} 
                                alt={detail.productName}
                                className="w-10 h-10 object-cover rounded"
                              />
                              <span>{detail.productName}</span>
                            </div>
                          </td>
                          <td className="py-3 px-4 text-left border-b text-gray-800">
                            {detail?.sku || '-'}
                          </td>
                          <td className="py-3 px-4 text-left border-b text-gray-800">{detail.size}</td>
                          <td className="py-3 px-4 text-left border-b text-gray-800">{detail.color}</td>
                          <td className="py-3 px-4 text-right border-b text-gray-800">{detail.quantity}</td>
                          <td className="py-3 px-4 text-right border-b text-gray-800">{formatCurrency(detail.price)}</td>
                          <td className="py-3 px-4 text-right border-b text-gray-800">{formatCurrency(detail.totalPrice)}</td>
                          <td className="py-3 px-4 text-center border-b">
                            {inventoryItem ? (
                              <Badge
                                variant={inventoryItem.isAvailable ? "default" : "destructive"}
                              >
                                {inventoryItem.isAvailable ? "Đủ hàng" : "Không đủ hàng"}
                              </Badge>
                            ) : (
                              '-'
                            )}
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Payment Summary */}
            <div className="mb-8 bg-gray-50 p-4 rounded-lg">
              <h2 className="text-lg font-semibold mb-3">Thanh Toán</h2>
              <div className="space-y-2">
                <div className="flex justify-between">
                  <span className="text-gray-600">Tổng Tiền:</span>
                  <span>{formatCurrency(bill.totalAmount)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Giảm trừ:</span>
                  <span>-{formatCurrency(bill.discountAmount || 0)}</span>
                </div>
                {bill.voucherCode && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Mã giảm giá: </span>
                    <span>{bill.voucherCode}</span>
                  </div>
                )}
                <Separator className="my-2" />
                <div className="flex justify-between font-semibold text-lg">
                  <span>Phải Trả</span>
                  <span className="text-primary">{formatCurrency(bill.amountToPay)}</span>
                </div>
                {bill.finalAmount !== null && bill.finalAmount !== bill.amountToPay && (
                  <div className="flex justify-between font-bold text-lg">
                    <span>Phải Trả</span>
                    <span className="text-primary">{formatCurrency(bill.finalAmount)}</span>
                  </div>
                )}
              </div>
            </div>

            {/* Notes */}
            {bill.notes && (
              <div className="mb-8">
                <h2 className="text-lg font-semibold mb-2">Ghi chú</h2>
                <div className="p-3 bg-gray-50 rounded-lg text-gray-700">
                  {bill.notes}
                </div>
              </div>
            )}


            {/* Action Buttons */}
            <div className="mt-6 flex gap-3 justify-end border-t pt-4">
              <Button variant="outline" onClick={onClose}>
                Đóng
              </Button>
              {bill.status === "0" && (
                <>
                  <Button variant="destructive">Hủy</Button>
                  <Button>Mark as Completed</Button>
                </>
              )}
              {bill.paymentStatus === "0" && bill.status !== "2" && (
                <Button variant="default">Mark as Paid</Button>
              )}
            </div>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
};

export default DetailBillSheet;