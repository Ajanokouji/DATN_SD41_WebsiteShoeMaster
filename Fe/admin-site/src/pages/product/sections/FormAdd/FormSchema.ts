import * as z from "zod";

export const productFormSchema = z.object({
  code: z.string().min(3, "Mã sản phẩm phải có ít nhất 3 ký tự"),
  name: z.string().min(2, "Tên sản phẩm phải có ít nhất 2 ký tự"),
  description: z
    .string()
    .min(10, "Mô tả phải có ít nhất 10 ký tự"),
  mainCategoryId: z.string().min(1, 'Không được để trống danh mục chính'),
  status: z.string(),
  imageUrl: z.string().url("URL hình ảnh không hợp lệ"),
  mediaObjs: z.array(z.string()).default([]),
  completeCode: z.string(),
  completeName: z.string(),
  completePath: z.string(),
  variantObjs: z.array(
    z.object({
      group1: z.string(),
      group2: z.string(),
      price: z.string(),
      stock: z.number(),
      sku: z.string(),
      imgUrl : z.string().optional()
    })
  ),
  variantJson: z.string().optional(),
  metadataObj: z.array(
    z.object({
      fieldName: z.string().min(1, "Field name is required"),
      fieldDisplayName: z.string().min(1, "Display name is required"),
      fieldType: z.number(),
      fieldValues: z.string(),
      fieldValueTexts: z.string(),
      fieldValueType: z.string(),
      fieldSelectionValues: z.array(
        z.object({
          key: z.string(),
          code: z.string(),
          value: z.string(),
          order: z.number(),
        })
      ),
    })
  ),
  labelsObjs: z.array(
    z.object({
      objectId: z.string(),
      objectCode: z.string(),
      objectName: z.string(),
      color: z.string(),
    })
  ),
  sortOrder: z.string(),
  createdByUserId: z.string(),
  lastModifiedByUserId: z.string(),
  lastModifiedDate: z.string(),
  createdOnDate: z.string(),
  publicOnDate: z.string(),
  workFlowStates: z.string(),
});

export type ProductFormSchema = z.infer<typeof productFormSchema>;
