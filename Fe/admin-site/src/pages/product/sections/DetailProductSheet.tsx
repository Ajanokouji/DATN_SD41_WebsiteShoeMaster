import React, { useEffect } from "react";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectProduct } from "@/redux/apps/product/productSelector";
import { fetchProductById } from "@/redux/apps/product/productSlice";
import { formatVietnamTime } from "@/utils/format";

interface DetailProductSheetProps {
  productId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailProductSheet: React.FC<DetailProductSheetProps> = ({
  productId,
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const product = useAppSelector(selectProduct);

  useEffect(() => {
    if (productId) {
      dispatch(fetchProductById(productId));
    }
  }, [dispatch, productId]);

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700 mb-4">
            Product Details
          </SheetTitle>
          <SheetDescription />
        </SheetHeader>

        {product && (
          <div className="p-4 space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Image Section */}
              <div className="flex-1">
                <img
                  src={product.imageUrl}
                  alt={product.name}
                  className="rounded-lg w-full max-w-sm mx-auto"
                />
              </div>
              {/* Media Section */}
              <div className="mt-6">
                {product.mediaObjs && product.mediaObjs.length > 0 ? (
                  <div className="grid grid-cols-2 gap-4">
                    {product.mediaObjs.map((mediaUrl, index) => (
                      <div key={index} className="flex justify-center">
                        <img
                          src={mediaUrl}
                          alt={`Product Media ${index + 1}`}
                          className="rounded-lg w-full max-w-48"
                        />
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-gray-700">No media available.</p>
                )}
              </div>
            </div>
            {/* Product Info Section */}
            <div className="flex-1 space-y-4">
              <h2 className="text-2xl font-bold text-gray-800">
                {product.name}
              </h2>
              <p className="text-gray-600">{product.description}</p>
              <p className="font-medium text-gray-700">
                Status: <span className="text-green-500">{product.status}</span>
              </p>
              <p className="font-medium text-gray-700">
                Code: <span className="text-gray-500">{product.code}</span>
              </p>
              <p className="font-medium text-gray-700">
                Sort Order:{" "}
                <span className="text-gray-500">{product.sortOrder}</span>
              </p>
              <p className="font-medium text-gray-700">
                Complete Name:{" "}
                <span className="text-gray-500">{product.completeName}</span>
              </p>
              <p className="font-medium text-gray-700">
                Complete Code:{" "}
                <span className="text-gray-500">{product.completeCode}</span>
              </p>

              {/* Metadata Section */}
              <div className="mt-4">
                <h3 className="font-semibold text-lg text-gray-800">
                  Attributes
                </h3>
                <ul className="list-disc list-inside text-gray-700">
                  {product.metadataObj && product.metadataObj.length > 0 ? (
                    product.metadataObj.map((meta) => (
                      <li key={meta.fieldName} className="mb-2">
                        <span className="font-medium">
                          {meta.fieldDisplayName || meta.fieldName}:
                        </span>{" "}
                        {meta.fieldValues}
                        {/* Hiển thị fieldSelectionValues nếu có */}
                        {meta.fieldSelectionValues &&
                          meta.fieldSelectionValues.length > 0 && (
                            <ul className="list-disc list-inside pl-6 text-gray-600">
                              {meta.fieldSelectionValues.map((selection) => (
                                <li key={selection.key}>
                                  <span className="font-medium">
                                    {selection.code}:
                                  </span>{" "}
                                  {selection.value}
                                </li>
                              ))}
                            </ul>
                          )}
                      </li>
                    ))
                  ) : (
                    <li>No attributes available.</li>
                  )}
                </ul>
              </div>
            </div>

            {/* Variants Section */}
            <div>
              <h3 className="font-semibold text-lg text-gray-800">Variants</h3>
              {product.variantObjs && product.variantObjs.length > 0 ? (
                <div className="grid grid-cols-2 gap-4">
                  {product.variantObjs.map((variant, index) => (
                    <div
                      key={index}
                      className="p-4 border rounded-lg shadow-sm bg-gray-50"
                    >
                      <p>
                        Size: <strong>{variant.size}</strong>
                      </p>
                      <p>
                        Size Type: <strong>{variant.sizeType}</strong>
                      </p>
                      <p>
                        Lowest Ask: <strong>${variant.lowestAsk}</strong>
                      </p>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-gray-700">No variants available.</p>
              )}
            </div>

            {/* Labels Section */}
            <div>
              <h3 className="font-semibold text-lg text-gray-800">Labels</h3>
              {product.labelsObjs && product.labelsObjs.length > 0 ? (
                <div className="flex flex-wrap gap-2">
                  {product.labelsObjs.map((label, index) => (
                    <span
                      key={index}
                      className="px-2 py-1 rounded text-white"
                      style={{ backgroundColor: label.color }}
                    >
                      {label.objectName}
                    </span>
                  ))}
                </div>
              ) : (
                <p className="text-gray-700">No labels available.</p>
              )}
            </div>

            {/* Workflow States and Dates */}
            <div className="mt-6">
              <h3 className="font-semibold text-lg text-gray-800">
                Additional Info
              </h3>
              <p className="text-gray-700">
                Workflow State: {product.workFlowStates}
              </p>
              <p className="text-gray-700">
                Public On Date: {formatVietnamTime(product.publicOnDate)}
              </p>
              <p className="text-gray-700">
                Last Modified On:{" "}
                {formatVietnamTime(product.lastModifiedOnDate)}
              </p>
              <p className="text-gray-700">
                Created On: {formatVietnamTime(product.createdOnDate)}
              </p>
            </div>
          </div>
        )}
      </SheetContent>
    </Sheet>
  );
};

export default DetailProductSheet;
