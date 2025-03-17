export interface BillFormValues {
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
    notes: string;
  }
  