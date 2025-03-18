import * as z from "zod";

export const contactFormSchema = z.object({
  name: z.string().min(1, "Code is required"),
  fullName: z.string().min(1, "Name is required"),
  address: z.string(),
  dateOfBirth: z.string(),
  imageUrl: z.string(),
  email: z.string(),
  phoneNumber: z.string(),
  content: z.string(),
  status: z.number(),
  isdeleted: z.boolean(),
  createdByUserId: z.string(),
  lastModifiedByUserId: z.string(),
  lastModifiedOnDate: z.string(),
  createdOnDate: z.string(),
});

export type ContactFormSchema = z.infer<typeof contactFormSchema>;