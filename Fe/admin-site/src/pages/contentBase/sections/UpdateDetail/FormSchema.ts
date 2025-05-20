import * as z from "zod";

export const contentBaseFormSchema = z
  .object({
    title: z.string().min(1, "Tiêu đề bắt buộc nhập"),
    seoUri: z.string().min(1, "Đường dẫn SEO bắt buộc nhập"),
    seoTitle: z.string().min(1, "Tiêu đề SEO bắt buộc nhập"),
    seoDescription: z.string().optional(),
    seoKeywords: z.string().min(1, "Từ khóa SEO bắt buộc nhập"),
    publishStartDate: z.string().min(1, "Ngày bắt đầu bắt buộc nhập"),
    publishEndDate: z.string().min(1, "Ngày kết thúc bắt buộc nhập"),
    isPublish: z.boolean(),
    createdByUserId: z.string(),
    lastModifiedByUserId: z.string(),
    createdOnDate: z.string(),
    lastModifiedOnDate: z.string(),
    content: z.string().min(1, "Nội dung bắt buộc nhập"),
  })
  .refine((data) => {
    const start = new Date(data.publishStartDate);
    const end = new Date(data.publishEndDate);
    return start < end;
  }, {
    message: "Ngày bắt đầu phải nhỏ hơn ngày kết thúc",
    path: ["publishStartDate"],
  });

export type ContentBaseFormSchema = z.infer<typeof contentBaseFormSchema>;