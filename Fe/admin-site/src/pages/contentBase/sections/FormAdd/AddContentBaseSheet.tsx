import React, { useState } from "react";
import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { createContentBase } from "@/redux/apps/contentBase/contentBaseSlice";
import { contentBaseFormSchema, ContentBaseFormSchema } from "./FormSchema";
import { ContentBaseReqDto } from "@/types/contentBase/contentBase";
import { BasicInfoFields } from "./BasicFields";

interface AddContentBaseSheetProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddContentBaseSheet: React.FC<AddContentBaseSheetProps> = ({
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<ContentBaseFormSchema>({
    resolver: zodResolver(contentBaseFormSchema),
    mode: "onChange",
    reValidateMode: "onChange",
    defaultValues: {
      title: "",
      seoUri: "",
      seoTitle: "",
      seoDescription: "",
      seoKeywords: "",
      publishStartDate: "",
      publishEndDate: "",
      isPublish: false,
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedOnDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
      isdeleted: false,
      content: "",
    },
  });

  const handleSubmit = async (values: ContentBaseFormSchema) => {
    setIsSubmitting(true);
    try {
      const contentBaseData: ContentBaseReqDto = {
        ...values,
        seoUri: values.seoUri.trim(),
        seoTitle: values.seoTitle.trim(),
        seoDescription: values.seoDescription?.trim() || "",
        seoKeywords: values.seoKeywords.trim(),
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedOnDate: values.lastModifiedOnDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
        content: values.content || "",
      };

      const createRs = await dispatch(createContentBase(contentBaseData));
      if (createContentBase.fulfilled.match(createRs)) {
        onClose();
      }
    } catch (error) {
      console.error("Lỗi trong quá trình thêm content base:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Tạo Content Base
          </SheetTitle>
          <SheetDescription>
            Điền thông tin để tạo một Content Base mới.
          </SheetDescription>
        </SheetHeader>
        <br />
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-6 overflow-auto"
          >
            {/* <div className="mb-4">
              <label htmlFor="title" className="block text-sm font-medium mb-1">
                Tiêu đề <span className="text-red-500">*</span>
              </label>
              <input
                id="title"
                type="text"
                value={form.watch("title")}
                onChange={handleTitleChange}
                className="border p-2 rounded w-full"
                placeholder="Nhập tiêu đề bài viết"
              />
            </div>

            <div className="mb-4">
              <label htmlFor="seoUri" className="block text-sm font-medium mb-1">
                SEO URI (Tự động tạo)
              </label>
              <input
                id="seoUri"
                type="text"
                value={form.watch("seoUri")}
                readOnly
                className="border p-2 rounded w-full bg-gray-100"
                placeholder="SEO URI sẽ tự động tạo từ tiêu đề"
              />
            </div> */}

            <BasicInfoFields control={form.control} />

            <div className="flex justify-end gap-2 pt-4">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Đang tạo..." : "Tạo"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Thoát
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
};

export default AddContentBaseSheet;