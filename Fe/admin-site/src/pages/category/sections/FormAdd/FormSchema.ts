import * as z from "zod";

export const categoryFormSchema = z.object({
  code: z.string().min(1, "Code is required"),
  name: z.string().min(1, "Name is required"),
  description: z.string(),
  parentId: z.string().nullable(),
  type: z.string(),
  completeCode: z.string(),
  completeName: z.string(),
  completePath: z.string(),
  parentPath: z.string(),
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
          order: z.number()
        })
      )
    })
  ),
  sortOrder: z.number(),
  createdByUserId: z.string(),
  lastModifiedByUserId: z.string(),
  lastModifiedOnDate: z.string(),
  createdOnDate: z.string(),
});

export type CategoryFormSchema = z.infer<typeof categoryFormSchema>;