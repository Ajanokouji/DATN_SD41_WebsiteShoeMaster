import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { FieldSelectionValue, FormData } from "@/types/product/form";
import {
  fetchProductById,
  updateProduct,
} from "@/redux/apps/product/productSlice";
import { MetadataObj, VariantObjs } from "@/types/product/product";

export const useProductForm = (
  productId: string,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  product: any,
  onClose: () => void
) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [fieldSelectionValues, setFieldSelectionValues] = useState<
    FieldSelectionValue[]
  >([]);
  const [variantObjs, setVariantObjs] = useState<VariantObjs[]>([]);
  const [mediaObjs, setMediaObjs] = useState<string[]>([]);

  const methods = useForm<FormData>();
  const { setValue } = methods;

  useEffect(() => {
    if (productId) {
      dispatch(fetchProductById(productId));
    }
  }, [dispatch, productId]);

  useEffect(() => {
    if (product) {
      setValue("name", product.name || "");
      setValue("description", product.description || "");
      setValue("imageUrl", product.imageUrl || "");
      setValue("parentId", product.id || "");
      setValue("sortOrder", product.sortOrder || "");
      setValue("type", product.type || "");
      setValue("code", product.code || "");
      setValue("completeCode", product.completeCode || "");
      setValue("completeName", product.completeName || "");
      setValue("completePath", product.completePath || "");

      // Map variant data to CombinationData structure
      const mappedVariants = (product.variantObjs || []).map(variant => ({
        group1: variant.group1 || "",
        group2: variant.group2 || "",
        price: variant.price || "",
        stock: variant.stock || 0,
        sku: variant.sku || "",
        imgUrl: variant.imgUrl || ""
      }));
      setVariantObjs(mappedVariants);
      setMediaObjs(product.mediaObjs || []);

      product.metadataObj?.forEach((meta: MetadataObj) => {
        setValue(meta.fieldName, meta.fieldValues || "");
      });

      // Extract fieldSelectionValues but don't set it in the form
      const extractedFieldValues: FieldSelectionValue[] =
        product.metadataObj?.flatMap(
          (meta: MetadataObj) =>
            meta.fieldSelectionValues?.map((fv: FieldSelectionValue) => ({
              ...fv,
              fieldName: meta.fieldName,
            })) || []
        ) || [];

      setFieldSelectionValues(extractedFieldValues);
    }
  }, [product, setValue]);

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

  const handleVariantChange = (variants: VariantObjs[]) => {
    setVariantObjs(variants);
    methods.setValue("variantObjs", variants);
  };

  const handleAddVariant = () => {
    setVariantObjs([
      ...variantObjs,
      {
        group1: "",
        group2: "",
        price: "",
        stock: 0,
        sku: "",
        imgUrl:"",
      },
    ]);
  };
  const handleRemoveVariant = (index: number) => {
    const newVariants = [...variantObjs];
    newVariants.splice(index, 1);
    setVariantObjs(newVariants);
  };

  // Media Objects Handlers
  const handleAddMedia = (mediaUrl: string) => {
    setMediaObjs([...mediaObjs, mediaUrl]);
  };

  const handleRemoveMedia = (index: number) => {
    const newMedia = [...mediaObjs];
    newMedia.splice(index, 1);
    setMediaObjs(newMedia);
  };

  const handleSubmit = (value: FormData) => {
    const standardFields = [
      "name",
      "description",
      "imageUrl",
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
      product?.metadataObj
        ?.filter(
          (meta: MetadataObj) =>
            !updatedMetadata.some(
              (newMeta) => newMeta.fieldName === meta.fieldName
            )
        )
        .concat(updatedMetadata) || updatedMetadata;

    const updatedProduct = {
      ...product,
      ...value,
      metadataObj: mergedMetadata,
      variantObjs: variantObjs,
      mediaObjs: mediaObjs,
    };

    console.log("Updated Product:", updatedProduct);

    dispatch(updateProduct({ id: productId, data: updatedProduct }));
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
    variantObjs,
    handleVariantChange,
    handleAddVariant,
    handleRemoveVariant,
    mediaObjs,
    handleAddMedia,
    handleRemoveMedia,
    handleSubmit,
  };
};
