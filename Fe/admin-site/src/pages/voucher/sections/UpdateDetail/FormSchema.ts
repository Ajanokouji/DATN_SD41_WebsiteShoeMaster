import * as z from "zod";

const discountAmountSchema = z
  .preprocess(
    (val) => (val === "" || val === undefined ? undefined : Number(val)),
    z
      .number({
        invalid_type_error: "Số tiền giảm phải là số",
        required_error: "Giá trị giảm bắt buộc nhập",
      })
      .int("Số tiền giảm phải là số nguyên")
      .gt(0, "Số tiền giảm phải lớn hơn 0")
  );

const discountPercentageSchema = z
  .preprocess(
    (val) => (val === "" || val === undefined ? undefined : Number(val)),
    z
      .number({
        invalid_type_error: "Phần trăm giảm phải là số",
        required_error: "Giá trị giảm bắt buộc nhập",
      })
      .gt(0, "Phần trăm giảm phải lớn hơn 0")
      .lte(100, "Phần trăm giảm phải nhỏ hơn hoặc bằng 100")
  );

const baseSchema = z.object({
  voucherName: z.string().min(1, "Tên voucher bắt buộc nhập"),
  code: z.string().min(1, "Mã voucher bắt buộc nhập"),

  startDate: z.string().min(1, "Ngày bắt đầu bắt buộc nhập"),
  endDate: z.string().min(1, "Ngày kết thúc bắt buộc nhập"),

  minimumOrderAmount: z
    .preprocess(
      (val) => (val === "" || val === undefined ? undefined : Number(val)),
      z
        .number({
          required_error: "Giá trị đơn hàng tối thiểu bắt buộc nhập",
          invalid_type_error: "Giá trị đơn hàng tối thiểu phải là số",
        })
        .int("Giá trị đơn hàng tối thiểu phải là số nguyên")
        .min(0, "Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0")
    ),

  description: z.string().optional(),

  status: z.number(),
  voucherType: z.number(),
  isdeleted: z.boolean(),
  createdByUserId: z.string(),
  lastModifiedByUserId: z.string(),
  createdOnDate: z.string(),
  lastModifiedOnDate: z.string(),
});

export const voucherFormSchema = z
  .discriminatedUnion("discountType", [
    z.object({
      discountType: z.literal("$"),
      discountAmount: discountAmountSchema,
      discountPercentage: z.any(),
    }),
    z.object({
      discountType: z.literal("%"),
      discountPercentage: discountPercentageSchema,
      discountAmount: z.any(),
    }),
  ])
  .and(baseSchema)
  .refine((data) => {
    const start = new Date(data.startDate);
    const end = new Date(data.endDate);
    return start < end;
  }, {
    message: "Ngày bắt đầu phải nhỏ hơn ngày kết thúc",
    path: ["startDate"],
  })
  .refine((data) => {
    const start = new Date(data.startDate);
    const end = new Date(data.endDate);
    return start < end;
  }, {
    message: "Ngày kết thúc phải lớn hơn ngày bắt đầu",
    path: ["endDate"],
  });

export type VoucherFormSchema = z.infer<typeof voucherFormSchema>;