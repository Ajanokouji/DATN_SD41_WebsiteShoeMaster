import * as z from "zod";

export const voucherFormSchema = z.object({
  voucherName: z.string().min(1, "Voucher names is required"),
  voucherType: z.number(),
  createdByUserId: z.string(),
  lastModifiedByUserId: z.string(),
  lastModifiedOnDate: z.string(),
  createdOnDate: z.string(),
  startDate: z.string(),
  endDate: z.string(),
  status: z.number(),
  isdeleted: z.boolean(),

});

export type VoucherFormSchema = z.infer<typeof voucherFormSchema>;
