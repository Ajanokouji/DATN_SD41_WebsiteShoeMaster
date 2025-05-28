import React, { useEffect, useState } from "react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectProduct } from "@/redux/apps/product/productSelector";
import { fetchProductById } from "@/redux/apps/product/productSlice";
import { formatVietnamTime } from "@/utils/format";
import { Badge } from "@/components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Edit, ChevronLeft, ChevronRight } from "lucide-react";

interface DetailProductSheetProps {
  productId: string;
  isOpen: boolean;
  onClose: () => void;
  onEdit?: () => void;
}

const DetailProductSheet: React.FC<DetailProductSheetProps> = ({
  productId,
  isOpen,
  onClose,
  onEdit,
}) => {
  const dispatch = useAppDispatch();
  const product = useAppSelector(selectProduct);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  useEffect(() => {
    if (productId) {
      dispatch(fetchProductById(productId));
    }
  }, [dispatch, productId]);

  const handleNextImage = () => {
    if (product?.mediaObjs) {
      setCurrentImageIndex((prev) => 
        prev === product.mediaObjs.length - 1 ? 0 : prev + 1
      );
    }
  };

  const handlePrevImage = () => {
    if (product?.mediaObjs) {
      setCurrentImageIndex((prev) => 
        prev === 0 ? product.mediaObjs.length - 1 : prev - 1
      );
    }
  };

  if (!product) {
    return (
      <Sheet open={isOpen} onOpenChange={onClose}>
        <SheetContent className="w-full sm:max-w-lg">
          <div className="flex items-center justify-center h-full">
            <p>Đang tải chi tiết sản phẩm...</p>
          </div>
        </SheetContent>
      </Sheet>
    );
  }

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader className="space-y-4">
          <div className="flex justify-between items-center">
            <SheetTitle className="text-2xl font-bold">
              Chi tiết sản phẩm
            </SheetTitle>
            {onEdit && (
              <Button 
                variant="outline" 
                size="sm" 
                onClick={onEdit}
                className="flex items-center gap-2"
              >
                <Edit className="h-4 w-4" />
                Chỉnh sửa
              </Button>
            )}
          </div>
          <Badge variant="outline" className="w-fit">
            {product.status}
          </Badge>
        </SheetHeader>

        <Tabs defaultValue="info" className="mt-6">
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="info">Thông tin cơ bản</TabsTrigger>
            <TabsTrigger value="media">Hình ảnh</TabsTrigger>
            <TabsTrigger value="variants">Biến thể</TabsTrigger>
            <TabsTrigger value="metadata">Thuộc tính</TabsTrigger>
          </TabsList>

          <TabsContent value="info" className="mt-6">
            <Card>
              <CardContent className="p-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <img
                      src={product.imageUrl}
                      alt={product.name}
                      className="rounded-lg w-full object-cover max-h-[400px]"
                    />
                  </div>
                  <div className="space-y-4">
                    <div>
                      <h2 className="text-2xl font-bold">{product.name}</h2>
                      <p className="text-gray-600 mt-2">{product.description}</p>
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <p className="text-sm text-gray-500">Mã sản phẩm</p>
                        <p className="font-medium">{product.code}</p>
                      </div>
                      <div>
                        <p className="text-sm text-gray-500">Thứ tự hiển thị</p>
                        <p className="font-medium">{product.sortOrder}</p>
                      </div>
                      <div>
                        <p className="text-sm text-gray-500">Tên đầy đủ</p>
                        <p className="font-medium">{product.completeName}</p>
                      </div>
                      <div>
                        <p className="text-sm text-gray-500">Mã đầy đủ</p>
                        <p className="font-medium">{product.completeCode}</p>
                      </div>
                    </div>
                    <div className="pt-4 border-t">
                      <h3 className="font-semibold mb-2">Nhãn sản phẩm</h3>
                      <div className="flex flex-wrap gap-2">
                        {product.labelsObjs?.map((label, index) => (
                          <Badge 
                            key={index}
                            style={{ backgroundColor: label.color }}
                          >
                            {label.objectName}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="media" className="mt-6">
            <Card>
              <CardContent className="p-6">
                <div className="relative">
                  {product.mediaObjs && product.mediaObjs.length > 0 ? (
                    <>
                      <div className="relative h-[500px] w-full">
                        <img
                          src={product.mediaObjs[currentImageIndex]}
                          alt={`Product media ${currentImageIndex + 1}`}
                          className="w-full h-full object-contain"
                        />
                        <div className="absolute inset-0 flex items-center justify-between">
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={handlePrevImage}
                            className="hover:bg-black/10"
                          >
                            <ChevronLeft className="h-8 w-8" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={handleNextImage}
                            className="hover:bg-black/10"
                          >
                            <ChevronRight className="h-8 w-8" />
                          </Button>
                        </div>
                      </div>
                      <div className="mt-4 flex gap-2 overflow-x-auto pb-2">
                        {product.mediaObjs.map((media, index) => (
                          <img
                            key={index}
                            src={media}
                            alt={`Thumbnail ${index + 1}`}
                            className={`w-20 h-20 object-cover cursor-pointer rounded-md
                              ${currentImageIndex === index ? 'ring-2 ring-primary' : ''}`}
                            onClick={() => setCurrentImageIndex(index)}
                          />
                        ))}
                      </div>
                    </>
                  ) : (
                    <p className="text-center text-gray-500">Không có hình ảnh bổ sung</p>
                  )}
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="variants" className="mt-6">
            <Card>
              <CardContent className="p-6">
                {product.variantObjs && product.variantObjs.length > 0 ? (
                  <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                    {product.variantObjs.map((variant, index) => (
                      <div 
                        key={index}
                        className="p-4 border rounded-lg hover:shadow-md transition-shadow"
                      >
                        <div className="space-y-2">
                          {variant.imgUrl && (
                            <div className="mb-3 flex justify-center">
                              <img 
                                src={variant.imgUrl} 
                                alt={`Variant ${index + 1}`}
                                className="max-h-32 max-w-32 object-contain rounded-md"
                              />
                            </div>
                          )}
                          <div className="flex justify-between items-center">
                            <span className="text-sm text-gray-500">Màu sắc:</span>
                            <span className="font-medium">{variant.group1}</span>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-sm text-gray-500">Loại size:</span>
                            <span className="font-medium">{variant.group2}</span>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-sm text-gray-500">Số lượng tồn:</span>
                            <span className="font-medium">{variant.stock}</span>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-sm text-gray-500">Giá :</span>
                            <span className="font-medium text-primary">
                              ${variant.price}
                            </span>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-center text-gray-500">Không có biến thể</p>
                )}
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="metadata" className="mt-6">
            <Card>
              <CardContent className="p-6">
                {product.metadataObj && product.metadataObj.length > 0 ? (
                  <div className="space-y-6">
                    {product.metadataObj.map((meta, index) => (
                      <div key={index} className="border-b pb-4 last:border-0">
                        <h3 className="font-semibold text-lg mb-2">
                          {meta.fieldDisplayName || meta.fieldName}
                        </h3>
                        <p className="text-gray-700">{meta.fieldValues}</p>
                        {meta.fieldSelectionValues && meta.fieldSelectionValues.length > 0 && (
                          <div className="mt-2 pl-4">
                            <p className="text-sm text-gray-500 mb-1">Giá trị chi tiết:</p>
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                              {meta.fieldSelectionValues.map((selection, idx) => (
                                <div 
                                  key={idx}
                                  className="flex justify-between items-center p-2 bg-gray-50 rounded"
                                >
                                  <span className="text-sm font-medium">
                                    {selection.code}:
                                  </span>
                                  <span className="text-sm">{selection.value}</span>
                                </div>
                              ))}
                            </div>
                          </div>
                        )}
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-center text-gray-500">Không có thuộc tính bổ sung</p>
                )}
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>

        <div className="mt-6 text-sm text-gray-500 space-y-1">
          <p>Ngày tạo: {formatVietnamTime(product.createdOnDate)}</p>
          <p>Cập nhật lần cuối: {formatVietnamTime(product.lastModifiedOnDate)}</p>
          <p>Ngày công khai: {formatVietnamTime(product.publicOnDate)}</p>
        </div>
      </SheetContent>
    </Sheet>
  );
};

export default DetailProductSheet;
