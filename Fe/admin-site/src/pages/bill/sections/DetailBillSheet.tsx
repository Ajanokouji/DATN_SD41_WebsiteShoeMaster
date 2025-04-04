import React, { useEffect, useState } from "react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
} from "@/components/ui/sheet";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectBill } from "@/redux/apps/bill/billSelector";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { fetchBillById, patchBill, updateBill } from "@/redux/apps/bill/billSlice";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Button } from "@/components/ui/button";
import { Printer, Download, FileText } from "lucide-react";
import { formatVietnamTime } from "@/utils/format";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ORDER_STATUS, ORDER_STATUS_LABELS } from "@/constants/orderStatus.constants";

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

  useEffect(() => {
    if (bill) {
      setSelectedStatus(bill.status);
    }
  }, [bill]);

  useEffect(() => {
    dispatch(fetchBillById(billId));
  }, [dispatch, billId]);

  const handleUpdateStatus = async (newStatus: string) => {
    if (bill && newStatus) {
      try {
        await dispatch(patchBill({
          id: bill.id,
          data: {
            id: bill.id,
            status: newStatus
          }
        }));
        // Refresh lại dữ liệu
        dispatch(fetchBillById(billId));
      } catch (error) {
        console.error("Failed to update status:", error);
      }
    }
  };

  const getStatusBadgeVariant = (status: string) => {
    switch (status) {
      case ORDER_STATUS.Completed:
        return "success";
      case ORDER_STATUS.Cancelled:
      case ORDER_STATUS.Rejected:
      case ORDER_STATUS.DeliveryFailed:
        return "destructive";
      case ORDER_STATUS.Shipping:
      case ORDER_STATUS.Delivered:
        return "info";
      case ORDER_STATUS.ReturnProcessing:
      case ORDER_STATUS.Returned:
        return "warning";
      default:
        return "secondary";
    }
  };

  const formatCurrency = (amount: number | null) => {
    if (amount === null) return "N/A";
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
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
                  <Badge 
                    variant={getStatusBadgeVariant(bill.status)} 
                    className="text-white border-white"
                  >
                    Trạng thái: {ORDER_STATUS_LABELS[bill.status] || bill.status}
                  </Badge>
                  <Badge variant="outline" className="text-white border-white">
                    Thanh toán: {bill.paymentStatus === "0" ? "Chưa thanh toán" : bill.paymentStatus === "1" ? "Đã thanh toán" : "Thất bại"}
                  </Badge>
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
                >
                  <SelectTrigger className="w-[200px]">
                    <SelectValue placeholder="Chọn trạng thái mới" />
                  </SelectTrigger>
                  <SelectContent>
                    {Object.entries(ORDER_STATUS_LABELS).map(([value, label]) => (
                      <SelectItem key={value} value={value}>
                        {label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <Button
                onClick={() => handleUpdateStatus(selectedStatus)}
                disabled={!selectedStatus || selectedStatus === bill.status}
              >
                Cập nhật trạng thái
              </Button>
            </div>
          </div>

          {/* Company & Invoice Info */}
          <div className="p-6 bg-white">
            <div className="grid grid-cols-2 gap-6 mb-8">
              <div>
                <h2 className="text-lg font-semibold mb-1">Từ</h2>
                <div className="text-gray-800">
                  <p className="font-medium">Your Company Name</p>
                  <p className="text-sm text-gray-600">
                    123 Business Street<br />
                    Business City, 12345<br />
                    contact@yourcompany.com<br />
                    +1 (555) 123-4567
                  </p>
                </div>
              </div>
              
              <div>
                <h2 className="text-lg font-semibold mb-1">Bill To</h2>
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
                <p className="text-sm text-gray-500">Invoice Number</p>
                <p className="font-medium">{bill.billCode}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Issue Date</p>
                <p className="font-medium">{formatVietnamTime(bill.createdOnDate)?.split(' ')[0]}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Payment Method</p>
                <p className="font-medium">{bill.paymentMethod || "Not specified"}</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Order ID</p>
                <p className="font-medium text-primary">{bill.orderId?.substring(0, 8)}</p>
              </div>
            </div>

            {/* Bill Items */}
            <div className="mb-8">
              <h2 className="text-lg font-semibold mb-3">Order Details</h2>
              <div className="overflow-x-auto">
                <table className="w-full border-collapse">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="py-3 px-4 text-left text-sm font-semibold text-gray-700 border-b">Product</th>
                      <th className="py-3 px-4 text-left text-sm font-semibold text-gray-700 border-b">Size</th>
                      <th className="py-3 px-4 text-left text-sm font-semibold text-gray-700 border-b">Color</th>
                      <th className="py-3 px-4 text-right text-sm font-semibold text-gray-700 border-b">Quantity</th>
                      <th className="py-3 px-4 text-right text-sm font-semibold text-gray-700 border-b">Unit Price</th>
                      <th className="py-3 px-4 text-right text-sm font-semibold text-gray-700 border-b">Total</th>
                    </tr>
                  </thead>
                  <tbody>
                    {bill.billDetails?.map((detail, index) => (
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
                        <td className="py-3 px-4 text-left border-b text-gray-800">{detail.size}</td>
                        <td className="py-3 px-4 text-left border-b text-gray-800">{detail.color}</td>
                        <td className="py-3 px-4 text-right border-b text-gray-800">{detail.quantity}</td>
                        <td className="py-3 px-4 text-right border-b text-gray-800">{formatCurrency(detail.price)}</td>
                        <td className="py-3 px-4 text-right border-b text-gray-800">{formatCurrency(detail.totalPrice)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Payment Summary */}
            <div className="mb-8 bg-gray-50 p-4 rounded-lg">
              <h2 className="text-lg font-semibold mb-3">Payment Summary</h2>
              <div className="space-y-2">
                <div className="flex justify-between">
                  <span className="text-gray-600">Subtotal:</span>
                  <span>{formatCurrency(bill.totalAmount)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Discount:</span>
                  <span>-{formatCurrency(bill.discountAmount || 0)}</span>
                </div>
                {bill.voucherCode && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Voucher ({bill.voucherCode}):</span>
                    <span>Applied</span>
                  </div>
                )}
                <Separator className="my-2" />
                <div className="flex justify-between font-semibold text-lg">
                  <span>Total</span>
                  <span className="text-primary">{formatCurrency(bill.amountToPay)}</span>
                </div>
                {bill.finalAmount !== null && bill.finalAmount !== bill.amountToPay && (
                  <div className="flex justify-between font-bold text-lg">
                    <span>Final Amount</span>
                    <span className="text-primary">{formatCurrency(bill.finalAmount)}</span>
                  </div>
                )}
              </div>
            </div>

            {/* Notes */}
            {bill.notes && (
              <div className="mb-8">
                <h2 className="text-lg font-semibold mb-2">Notes</h2>
                <div className="p-3 bg-gray-50 rounded-lg text-gray-700">
                  {bill.notes}
                </div>
              </div>
            )}

            {/* Thank You Note */}
            <div className="text-center mb-6">
              <div className="inline-block p-4 border-t border-gray-200">
                <FileText className="h-8 w-8 text-primary mx-auto mb-2" />
                <p className="font-medium text-gray-800">Thank you for your business!</p>
                <p className="text-sm text-gray-600 mt-1">If you have any questions, please contact our support team.</p>
              </div>
            </div>

            {/* Action Buttons */}
            <div className="mt-6 flex gap-3 justify-end border-t pt-4">
              <Button variant="outline" onClick={onClose}>
                Close
              </Button>
              {bill.status === "0" && (
                <>
                  <Button variant="destructive">Cancel Bill</Button>
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