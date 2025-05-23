import * as z from "zod";

const discountAmountSchema = z.preprocess(
  (val) => (val === "" || val === undefined ? undefined : Number(val)),
  z
    .number({
      invalid_type_error: "Số tiền giảm phải là số",
      required_error: "Giá trị giảm bắt buộc nhập",
    })
    .gt(0, "Số tiền giảm phải lớn hơn 0")
);

const discountPercentageSchema = z.preprocess(
  (val) => (val === "" || val === undefined ? undefined : Number(val)),
  z
    .number({
      invalid_type_error: "Phần trăm giảm phải là số",
      required_error: "Giá trị giảm bắt buộc nhập",
    })
    .gt(0, "Phần trăm giảm phải lớn hơn 0")
    .lte(100, "Phần trăm giảm phải nhỏ hơn hoặc bằng 100")
);

const maxDiscountAmountSchema = z.preprocess(
  (val) => (val === "" || val === undefined ? undefined : Number(val)),
  z
    .number({
      required_error: "Số tiền giảm tối đa bắt buộc nhập",
      invalid_type_error: "Số tiền giảm tối đa phải là số",
    })
    .gt(0, "Số tiền giảm tối đa phải lớn hơn 0")
);

const baseSchema = z.object({
  voucherName: z
    .string()
    .min(1, "Tên voucher bắt buộc nhập")
    .max(100, "Tên voucher không được vượt quá 100 ký tự"),

  code: z
    .string()
    .min(1, "Mã voucher bắt buộc nhập")
    .max(10, "Mã voucher không được vượt quá 10 ký tự")
    .regex(
      /^[A-Za-z0-9]+$/,
      "Mã voucher chỉ được chứa chữ cái không dấu và số, không có khoảng trắng hay ký tự đặc biệt"
    ),

  startDate: z.string().min(1, "Ngày bắt đầu bắt buộc nhập"),
  endDate: z.string().min(1, "Ngày kết thúc bắt buộc nhập"),

  minimumOrderAmount: z.preprocess(
    (val) => (val === "" || val === undefined ? undefined : Number(val)),
    z
      .number({
        required_error: "Giá trị đơn hàng tối thiểu bắt buộc nhập",
        invalid_type_error: "Giá trị đơn hàng tối thiểu phải là số",
      })
      .min(0, "Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0")
  ),

  description: z.string().max(256, "Mô tả không được quá 256 ký tự").optional(),

  status: z.number(),
  voucherType: z.number(),
  isdeleted: z.boolean(),
  createdByUserId: z.string(),
  lastModifiedByUserId: z.string(),
  createdOnDate: z.string(),
  lastModifiedOnDate: z.string(),
  imageUrl: z.string().nullable(),

  totalMaxUsage: z.preprocess(
    (val) => (val === "" || val === undefined ? undefined : Number(val)),
    z
      .number({
        required_error: "Tổng lượt sử dụng tối đa bắt buộc nhập",
        invalid_type_error: "Tổng lượt sử dụng tối đa phải là số",
      })
      .int("Tổng lượt sử dụng tối đa phải là số nguyên")
      .min(1, "Tổng lượt sử dụng tối đa phải lớn hơn 0")
  ),

  maxUsagePerCustomer: z.preprocess(
    (val) => (val === "" || val === undefined ? undefined : Number(val)),
    z
      .number({
        required_error: "Lượt sử dụng tối đa/người mua bắt buộc nhập",
        invalid_type_error: "Lượt sử dụng tối đa/người mua phải là số",
      })
      .int("Lượt sử dụng tối đa/người mua phải là số nguyên")
      .min(1, "Lượt sử dụng tối đa/người mua phải lớn hơn 0")
  ),
});

export const voucherFormSchema = z
  .discriminatedUnion("discountType", [
    z.object({
      discountType: z.literal("VNĐ"),
      discountAmount: discountAmountSchema,
      discountPercentage: z.any(),
      maxDiscountAmount: z.any(),
    }),
    z.object({
      discountType: z.literal("%"),
      discountPercentage: discountPercentageSchema,
      maxDiscountAmount: maxDiscountAmountSchema,
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
  })
  .refine((data) => data.maxUsagePerCustomer <= data.totalMaxUsage, {
    message: "Lượt sử dụng tối đa/người mua phải nhỏ hơn hoặc bằng Tổng lượt sử dụng tối đa",
    path: ["maxUsagePerCustomer"],
  });

export type VoucherFormSchema = z.infer<typeof voucherFormSchema>;