import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import {
  fetchContentBaseById,
  updateContentBase,
} from "@/redux/apps/contentBase/contentBaseSlice";
import { zodResolver } from "@hookform/resolvers/zod";
import { ContentBaseFormSchema, contentBaseFormSchema } from "./FormSchema";

export const useContentBaseForm = (
  contentBaseId: string,
  contentBase: any,
  onClose: () => void
) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const methods = useForm<ContentBaseFormSchema>({
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
      createdByUserId: "",
      lastModifiedByUserId: "",
      createdOnDate: new Date().toISOString(),
      lastModifiedOnDate: new Date().toISOString(),
      content: "",
    },
  });

  useEffect(() => {
    if (contentBase) {
      methods.reset({
        title: contentBase.title || "",
        seoUri: contentBase.seoUri || "",
        seoTitle: contentBase.seoTitle || "",
        seoDescription: contentBase.seoDescription || "",
        seoKeywords: contentBase.seoKeywords || "",
        publishStartDate: contentBase.publishStartDate || "",
        publishEndDate: contentBase.publishEndDate || "",
        isPublish: contentBase.isPublish || false,
        createdByUserId: contentBase.createdByUserId || "",
        lastModifiedByUserId: contentBase.lastModifiedByUserId || "",
        createdOnDate: contentBase.createdOnDate || new Date().toISOString(),
        lastModifiedOnDate: contentBase.lastModifiedOnDate || new Date().toISOString(),
        content: contentBase.content || "",
      });
    }
  }, [contentBase, methods]);

  useEffect(() => {
    if (contentBaseId) {
      setIsLoading(true);
      dispatch(fetchContentBaseById(contentBaseId)).finally(() =>
        setIsLoading(false)
      );
    }
  }, [dispatch, contentBaseId]);

  const handleSubmit = async (value: ContentBaseFormSchema) => {
    try {
      value.title = value.title.trim();
      value.seoUri = value.seoUri.trim();
      value.seoTitle = value.seoTitle.trim();
      value.seoDescription = value.seoDescription?.trim() || undefined;
      value.seoKeywords = value.seoKeywords.trim();
      value.content = value.content.trim();

      value.publishStartDate = new Date(value.publishStartDate).toISOString();
      value.publishEndDate = new Date(value.publishEndDate).toISOString();

      const updatedContentBase = {
        ...contentBase,
        ...value,
      };

      const editRs = await dispatch(
        updateContentBase({ id: contentBaseId, data: updatedContentBase })
      );
      if (updateContentBase.fulfilled.match(editRs)) {
        onClose();
      }
    } catch (error) {
      console.error("Lỗi trong quá trình sửa Content Base:", error);
    }
  };

  return {
    isEditing,
    setIsEditing,
    isLoading,
    methods,
    handleSubmit,
  };
};