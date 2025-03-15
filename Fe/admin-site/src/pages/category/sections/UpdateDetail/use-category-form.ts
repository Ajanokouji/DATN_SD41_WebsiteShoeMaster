import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import {
  fetchCategoryById,
  updateCategory,
} from "@/redux/apps/category/categorySlice";
import { FormData, FieldSelectionValue } from "@/types/category/form";
import { MetadataObj } from "@/types/category/category";

export const useCategoryForm = (
  categoryId: string,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  category: any,
  onClose: () => void
) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [fieldSelectionValues, setFieldSelectionValues] = useState<
    FieldSelectionValue[]
  >([]);

  const methods = useForm<FormData>();
  const { setValue } = methods;

  useEffect(() => {
    if (categoryId) {
      dispatch(fetchCategoryById(categoryId));
    }
  }, [dispatch, categoryId]);

  useEffect(() => {
    if (category) {
      setValue("name", category.name || "");
      setValue("description", category.description || "");
      setValue("parentId", category.id || "");
      setValue("sortOrder", category.sortOrder || 0);
      setValue("type", category.type || "");
      setValue("code", category.code || "");
      setValue("completeCode", category.completeCode || "");
      setValue("completeName", category.completeName || "");
      setValue("completePath", category.completePath || "");

      category.metadataObj?.forEach((meta: MetadataObj) => {
        setValue(meta.fieldName, meta.fieldValues || "");
      });

      // Extract fieldSelectionValues but don't set it in the form
      const extractedFieldValues: FieldSelectionValue[] =
        category.metadataObj?.flatMap(
          (meta: MetadataObj) =>
            meta.fieldSelectionValues?.map((fv: FieldSelectionValue) => ({
              ...fv,
              fieldName: meta.fieldName,
            })) || []
        ) || [];

      setFieldSelectionValues(extractedFieldValues);
    }
  }, [category, setValue]);

  const handleFieldSelectionValueChange = (
    index: number,
    field: keyof FieldSelectionValue,
    value: string | number
  ) => {
    const newValues = [...fieldSelectionValues];
    newValues[index] = {
      ...newValues[index],
      [field]: value,
    };
    setFieldSelectionValues(newValues);
  };

  const handleAddFieldSelectionValue = () => {
    setFieldSelectionValues([
      ...fieldSelectionValues,
      { key: "", code: "", value: "", order: 0, fieldName: "" },
    ]);
  };

  const handleRemoveFieldSelectionValue = (index: number) => {
    const newValues = [...fieldSelectionValues];
    newValues.splice(index, 1);
    setFieldSelectionValues(newValues);
  };

  const handleSubmit = (value: FormData) => {
    const standardFields = [
      "name",
      "description",
      "parentId",
      "sortOrder",
      "type",
      "code",
      "completeCode",
      "completeName",
      "completePath",
    ];

    // Nhóm fieldSelectionValues theo fieldName
    const groupedFieldSelectionValues: Record<string, FieldSelectionValue[]> =
      {};
    fieldSelectionValues.forEach((item) => {
      if (item.fieldName) {
        if (!groupedFieldSelectionValues[item.fieldName]) {
          groupedFieldSelectionValues[item.fieldName] = [];
        }
        groupedFieldSelectionValues[item.fieldName].push({
          key: item.key,
          code: item.code,
          value: item.value,
          order: item.order,
        });
      }
    });

    const updatedMetadata = Object.keys(value)
      .filter((key) => !standardFields.includes(key))
      .map((key) => ({
        fieldName: key,
        fieldValues: String(value[key]),
        fieldDisplayName: key.charAt(0).toUpperCase() + key.slice(1),
        fieldType: 0,
        fieldValueTexts: String(value[key]),
        fieldValueType: typeof value[key] === "string" ? "string" : "",
        fieldSelectionValues: groupedFieldSelectionValues[key] || [],
      }));

    const mergedMetadata =
      category?.metadataObj
        ?.filter(
          (meta: MetadataObj) =>
            !updatedMetadata.some(
              (newMeta) => newMeta.fieldName === meta.fieldName
            )
        )
        .concat(updatedMetadata) || updatedMetadata;

    const updatedCategory = {
      ...category,
      ...value,
      metadataObj: mergedMetadata,
    };

    dispatch(updateCategory({ id: categoryId, data: updatedCategory }));
    setIsEditing(false);
    onClose();
  };

  return {
    isEditing,
    setIsEditing,
    methods,
    fieldSelectionValues,
    handleFieldSelectionValueChange,
    handleAddFieldSelectionValue,
    handleRemoveFieldSelectionValue,
    handleSubmit,
  };
};
