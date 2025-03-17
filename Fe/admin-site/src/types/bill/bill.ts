import { PaginationParams } from "../common/pagination";

export interface BillReqDto {
    employeeId: string;
    customerId: string;
    orderId: string;
    paymentMethodId: string;
    voucherId?: string; // Có thể không bắt buộc
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
  voucherId: string;
  billCode: string;
  recipientName: string;
  recipientEmail: string;
  recipientPhone: string;
  recipientAddress: string;
  totalAmount: number;
  discountAmount: number;
  amountAfterDiscount: number;
  amountToPay: number;
  status: number;
  paymentStatus: number;
  updateBy: string;
  notes: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isDeleted: boolean;
}

export interface BillDetailResDto {
    id: string;
    employeeId: string;
    customerId: string;
    orderId: string;
    paymentMethodId: string;
    voucherId: string;
    billCode: string;
    recipientName: string;
    recipientEmail: string;
    recipientPhone: string;
    recipientAddress: string;
    totalAmount: number;
    discountAmount: number;
    amountAfterDiscount: number;
    amountToPay: number;
    status: number;
    paymentStatus: number;
    updateBy: string;
    notes: string;
    createdByUserId: string;
    lastModifiedByUserId: string;
    lastModifiedOnDate: string; // ISO string (UTC datetime)
    createdOnDate: string; // ISO string (UTC datetime)
    isDeleted: boolean;
  }
  

export interface BillFilterParams extends PaginationParams {
    Name?: string;
    Code?: string;
  }