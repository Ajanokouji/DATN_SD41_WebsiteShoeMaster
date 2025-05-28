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

export interface BillDetailDto {
  id: string;
  billId: string;
  productId: string;
  productName: string;
  productImage: string;
  size: number;
  color: string;
  quantity: number;
  price: number;
  totalPrice: number;
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
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  customerAddress: string;
  finalAmount: number;
  voucherCode: string;
  paymentMethod: string;
  updatedDate: string;
  billDetails: BillDetailDto[];
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
  productName: string;
  productImage: string;
  size: number;
  color: string;
  quantity: number;
  sku:string;
  price: number;
  totalPrice: number;
}

export interface BillFilterParams extends PaginationParams {
  id_hoa_don?: string;
  id_nhan_vien?: string;
  CustomerId?: string;
  id_don_hang?: string;
  id_phuong_thuc_thanh_toan?: string;
  BillCode?: string;
  ten_khach_nhan?: string;
  email_khach_nhan?: string;
  so_dien_thoai_khach_nhan?: string;
  dia_chi_nhan?: string;
  tong_tien?: number;
  tong_tien_khuyen_mai?: number;
  tong_tien_sau_khuyen_mai?: number;
  tong_tien_phai_thanh_toan?: number;
  Status?: string;
  PaymentStatus?: string;
  create_on_date?: string;
  last_modifi_on_date?: string;
  update_by?: string;
  ghi_chu?: string;
  ngay_tao?: string;
  ngay_thanh_toan?: string;
  tong_tien_decimal?: number;
  StartDate?: string;
  EndDate?: string;
}

export interface InventoryCheckDetail {
  productId: string;
  productName: string;
  requestedQuantity: number;
  availableQuantity: number;
  isAvailable: boolean;
  sku: string;
}

export interface InventoryCheckResponse {
  data: InventoryCheckDetail[];
  code: number;
  message: string;
  licenseInfo: string;
  totalTime: number;
}
