import { PaginationParams } from "../common/pagination";
import { CustomerResDto } from "../customer/customer";
import { ProductResDto } from "../product/product";
import { VoucherResDto } from "../voucher/voucher";

export interface BillReqDto {
  employeeId: string;
  customerId: string;
  orderId: string;
  paymentMethodId: string;
  voucherId?: string;
  billCode: string;
  recipientName: string;
  recipientEmail: string;
  recipientPhone: string;
  recipientAddress: string;
  totalAmount: number;
  discountAmount: number;
  amountAfterDiscount: number;
  amountToPay: number;
  status: number; // 0: Pending, 1: Completed, 2: Canceled...
  paymentStatus: number; // 0: Unpaid, 1: Paid
  updateBy?: string;
  notes?: string;
}

export interface BillResDto {
  id: string;
  employeeId: string;
  customerId: string;
  orderId: string;
  paymentMethodId: string;
  voucherId: string | null;
  billCode: string;
  recipientName: string;
  recipientEmail: string;
  recipientPhone: string;
  recipientAddress: string;
  totalAmount: number;
  discountAmount: number;
  amountAfterDiscount: number;
  amountToPay: number;
  status: string;
  paymentStatus: string;
  updateBy: string;
  notes: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface BillDetailResDto extends BillResDto {
  voucherCode: string | null;
  finalAmount: number | null;
  paymentMethod: string | null;
  billDetails?: BillDetailItem[];
  customer?: CustomerResDto;
  employee?: "Employee";
  order?: "Order";
  voucher?: VoucherResDto;
}

export interface BillDetailItem {
  id: string;
  billId: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  totalAmount: number;
  note: string | null;
  product?: ProductResDto;
}

export interface BillFilterParams extends PaginationParams {
  Name?: string;
  Code?: string;
}
