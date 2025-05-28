import * as z from "zod";

// Regex cho mã nhân viên: 3-20 ký tự, không dấu cách, không dấu tiếng Việt, không ký tự đặc biệt
const employeeCodeRegex = /^[A-Za-z0-9]{3,20}$/;

// Regex cho số điện thoại: bắt đầu bằng 0, 10 số
const phoneRegex = /^0\d{9}$/;

export const contactFormSchema = z.object({
  name: z
    .string()
    .min(1, "Mã nhân viên bắt buộc nhập")
    .regex(employeeCodeRegex, "Mã nhân viên phải từ 3-20 ký tự, không dấu cách, không dấu tiếng Việt, không ký tự đặc biệt"),
  fullName: z.string().min(1, "Họ tên bắt buộc nhập"),
  address: z.string().optional(),
  dateOfBirth: z.string().min(1, "Năm sinh bắt buộc nhập"),
  email: z
    .string()
    .optional()
    .refine(
      (val) => !val || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val),
      { message: "Email không đúng định dạng" }
    ),
  phoneNumber: z
    .string()
    .optional()
    .refine(
      (val) => !val || phoneRegex.test(val),
      { message: "Số điện thoại phải bắt đầu bằng số 0 và có 10 số" }
    ),
  isdeleted: z.boolean(),
  createdByUserId: z.string(),
  lastModifiedByUserId: z.string(),
  lastModifiedOnDate: z.string(),
  createdOnDate: z.string(),
});

export type ContactFormSchema = z.infer<typeof contactFormSchema>;