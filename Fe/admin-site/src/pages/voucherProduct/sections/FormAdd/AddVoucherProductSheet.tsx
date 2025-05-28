import { Button } from "@/components/ui/button";
import React, { useEffect, useRef, useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { fetchVoucherById } from "@/redux/apps/voucher/voucherSlice";
import VoucherProductReqDto, { VoucherProductResDto } from "@/types/voucherProduct/voucherProduct";
import { clearProducts, createVoucherProduct, findVoucherProductsByVcidPidVaid, searchProduct, setProductPage, setProductPageSize } from "@/redux/apps/voucherProduct/voucherProductSlice"
import { ProductDetailResDto, VariantObjs } from "@/types/product/product";
import { Input } from "@/components/ui/input";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
  selectProductPagination,
  selectProducts,
} from "@/redux/apps/voucherProduct/voucherProductSelector";
import Pagination from "@/components/Pagination";
import DetailProductSheet from "@/pages/product/sections/DetailProductSheet";

interface AddVoucherProductSheetProps {
  isOpen: boolean;
  onClose: () => void;
  voucherId: string;
  setValue: (name: string, value: any) => void;
}

const AddVoucherProductSheet: React.FC<AddVoucherProductSheetProps> = ({
  isOpen,
  onClose,
  voucherId,
  setValue,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errMs, setErrMs] = useState("");
  useEffect(() => {
    if (isOpen) {
      dispatch(clearProducts()); // Xóa danh sách products khi form được mở
    }
  }, [isOpen, dispatch]);

  const errorRef = useRef<HTMLHeadingElement>(null); // Tạo ref cho phần tử hiển thị lỗi
  
  useEffect(() => {
    if (errMs && errorRef.current) {
      // Nếu có lỗi, cuộn đến phần tử hiển thị lỗi
      errorRef.current.scrollIntoView({ behavior: "smooth", block: "center" });
    }
  }, [errMs]); // Theo dõi thay đổi của errMs

  const handleFormSubmit = async (values: ProductDetailResDto[] | null) => {
    setIsSubmitting(true);
    try {
      if(values === null || values.length <= 0)
      {
        setErrMs(""); // Đặt lại về rỗng để kích hoạt useEffect
        setTimeout(() => {
          setErrMs("Vui lòng không để trống sản phẩm cần thêm");
        }, 0); // Đặt lỗi mới sau một khoảng thời gian ngắn
        return;
      }
      setErrMs("");

      //Check voucher tồn tại
      const voucherResponse = await dispatch(fetchVoucherById(voucherId)).unwrap();
      if ((!voucherResponse) || (voucherResponse && voucherResponse.isDeleted)) {
        setErrMs(""); // Đặt lại về rỗng để kích hoạt useEffect
        setTimeout(() => {
          setErrMs("Không tìm thấy Voucher! Vui lòng kiểm tra lại!");
        }, 0); // Đặt lỗi mới sau một khoảng thời gian ngắn
        return;
      }
      setErrMs("");

      const voucherProducts: VoucherProductReqDto[] = values.flatMap((product) =>
        product.variantObjs.map((variant: VariantObjs) => ({
          voucherId: voucherId,
          productId: product.id,
          varientProductId: variant.sku,
          createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          lastModifiedOnDate: new Date().toISOString(),
          createdOnDate: new Date().toISOString(),
        }))
      );
      
      //Check xem các voucher Product cần thêm có trùng với voucher Product đã có trong db không
      const voucherProductsFound = await dispatch(findVoucherProductsByVcidPidVaid(voucherProducts)).unwrap();
      if(voucherProductsFound !== null && voucherProductsFound.length > 0)
      {
        setErrMs(""); // Đặt lại về rỗng để kích hoạt useEffect
        setTimeout(() => {
          setErrMs("Phát hiện trong danh sách sản phẩm đã chọn có biến thể của sản phẩm đã được thêm vào từ trước đó, vui lòng kiểm tra lại!");
        }, 0); // Đặt lỗi mới sau một khoảng thời gian ngắn
        return;
      }
      setErrMs("");

      var rs = await dispatch(createVoucherProduct(voucherProducts));
      if(createVoucherProduct.fulfilled.match(rs)){
        onClose();
      }
    } catch (error) {
      console.error("Lỗi trong quá trình thêm voucher:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  //search product
  const [searchString, setSearchString] = useState<string>("");
  const [inputSearchString, setInputSearchString] = useState<string>("");
  const products = useAppSelector(selectProducts);
  const pagination = useAppSelector(selectProductPagination);
  const [isSearching, setIsSearching] = useState<boolean>(false);
  const [variantProduct, setVariantProduct] = useState<ProductDetailResDto | null>(null);
  const [productsIsSelected, setProductsIsSelected] = useState<ProductDetailResDto[] | null>(null);
  
  const handleSubmitSearch = async () => {
    setIsSearching(true); // Bắt đầu tìm kiếm
    try {
      await dispatch(
        searchProduct({
          tenSanPham: inputSearchString,
          CurrentPage: 1,
          PageSize: pagination.pageSize,
        })
      ).unwrap(); // Chờ kết quả từ Redux Thunk
  
      setVariantProduct(null);
    } catch (error) {
      console.error("Search failed:", error); // Xử lý lỗi nếu có
    } finally {
      setIsSearching(false); // Kết thúc tìm kiếm (thành công hoặc thất bại)
    }
  };

  const handlePageChange = async (newPage: number) => {
    dispatch(setProductPage(newPage));
    try {
      await dispatch(
        searchProduct({
          tenSanPham: inputSearchString,
          CurrentPage: newPage,
          PageSize: pagination.pageSize,
        })
      ).unwrap(); // Chờ kết quả từ Redux Thunk
    } catch (error) {
      console.error("Search failed:", error);
    }
  };
  
  const handlePageSizeChange = async (newSize: number) => {
    dispatch(setProductPageSize(newSize));
    try {
      await dispatch(
        searchProduct({
          tenSanPham: inputSearchString,
          CurrentPage: 1,
          PageSize: newSize,
        })
      ).unwrap(); // Chờ kết quả từ Redux Thunk
    } catch (error) {
      console.error("Search failed:", error); // Xử lý lỗi nếu có
    }
  };

  //Thêm sản phẩm(biến thể) áp dụng mã voucher
  const AddProductIsSelected = (product: ProductDetailResDto, variant: VariantObjs) => {
    setProductsIsSelected((prev) => {
      if (!prev) {
        // Nếu danh sách ban đầu là null, thêm sản phẩm mới
        const newProduct = { ...product, variantObjs: [variant] };
        setValue("productsIsSelected", [newProduct]); // Lưu vào form state
        return [newProduct];
      }
  
      // Kiểm tra xem sản phẩm đã tồn tại trong danh sách chưa
      const existingProductIndex = prev.findIndex((p) => p.id === product.id);
  
      if (existingProductIndex === -1) {
        // Nếu sản phẩm chưa tồn tại, thêm sản phẩm mới vào đầu danh sách
        const newProduct = { ...product, variantObjs: [variant] };
        const updatedProducts = [newProduct, ...prev];
        setValue("productsIsSelected", updatedProducts); // Lưu vào form state
        return updatedProducts;
      } else {
        // Nếu sản phẩm đã tồn tại, thêm biến thể vào danh sách variantObjs
        const updatedProducts = [...prev];
        const existingProduct = updatedProducts[existingProductIndex];
  
        // Kiểm tra xem biến thể đã tồn tại trong variantObjs chưa
        const isVariantExists = existingProduct.variantObjs.some((v) => v.sku === variant.sku);
  
        if (!isVariantExists) {
          // Nếu biến thể chưa tồn tại, thêm biến thể vào variantObjs
          existingProduct.variantObjs = [...existingProduct.variantObjs, variant];
        }
  
        setValue("productsIsSelected", updatedProducts); // Lưu vào form state
        return updatedProducts;
      }
    });
  };

  const AddAllVariants = async (product: ProductDetailResDto) => {
    try {
      // Tạo payload để kiểm tra các biến thể trong DB
      const payload: VoucherProductReqDto[] = product.variantObjs.map((variant) => ({
        voucherId: voucherId,
        productId: product.id,
        varientProductId: variant.sku,
        createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // GUID hợp lệ
        lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // GUID hợp lệ
        lastModifiedOnDate: new Date().toISOString(), // Giá trị hợp lệ
        createdOnDate: new Date().toISOString(), // Giá trị hợp lệ
      }));

      // Gọi API để kiểm tra các biến thể đã tồn tại trong DB
      const existingVariants = await dispatch(
        findVoucherProductsByVcidPidVaid(payload)
      ).unwrap();

      // Lọc các biến thể chưa tồn tại trong DB và chưa tồn tại trong danh sách `productsIsSelected`
      const newVariants = product.variantObjs.filter(
        (variant) =>
          !existingVariants.some(
            (existing) =>
              existing.productId === product.id &&
              existing.varientProductId === variant.sku
          ) &&
          !productsIsSelected?.some(
            (selectedProduct) =>
              selectedProduct.id === product.id &&
              selectedProduct.variantObjs.some((v) => v.sku === variant.sku)
          )
      );

      // Nếu không có biến thể mới nào để thêm, hiển thị thông báo và dừng xử lý
      if (newVariants.length === 0) {
        setErrMs(""); // Đặt lại về rỗng để kích hoạt useEffect
        setTimeout(() => {
          setErrMs("Tất cả các biến thể của sản phẩm này đã được thêm trước đó hoặc đang trong danh sách đã chọn.");
        }, 0); // Đặt lỗi mới sau một khoảng thời gian ngắn
        return;
      }
      setErrMs("");

      // Thêm sản phẩm và các biến thể chưa tồn tại vào danh sách
      setProductsIsSelected((prev) => {
        if (!prev) {
          const newProduct = { ...product, variantObjs: newVariants };
          setValue("productsIsSelected", [newProduct]); // Lưu vào form state
          return [newProduct];
        }

        // Kiểm tra xem sản phẩm đã tồn tại trong danh sách chưa
        const existingProductIndex = prev.findIndex((p) => p.id === product.id);

        if (existingProductIndex === -1) {
          // Nếu sản phẩm chưa tồn tại, thêm sản phẩm mới với các biến thể chưa tồn tại
          const newProduct = { ...product, variantObjs: newVariants };
          const updatedProducts = [newProduct, ...prev];
          setValue("productsIsSelected", updatedProducts); // Lưu vào form state
          return updatedProducts;
        } else {
          // Nếu sản phẩm đã tồn tại, thêm các biến thể chưa có vào danh sách variantObjs
          const updatedProducts = [...prev];
          const existingProduct = updatedProducts[existingProductIndex];

          // Thêm các biến thể mới vào danh sách variantObjs
          existingProduct.variantObjs = [...existingProduct.variantObjs, ...newVariants];

          setValue("productsIsSelected", updatedProducts); // Lưu vào form state
          return updatedProducts;
        }
      });
    } catch (error) {
      console.error("Error checking existing variants:", error);
      setErrMs("Đã xảy ra lỗi khi kiểm tra biến thể trong cơ sở dữ liệu.");
    }
  };

  const removeAllProducts = () => {
    setProductsIsSelected(null); // Đặt danh sách về null
    setValue("productsIsSelected", []); // Cập nhật giá trị trong form state
  };

  const removeProductInProductsIsSelected = (index: number) => {
    setProductsIsSelected((prev) => {
      if (!prev) return null; // Nếu mảng ban đầu là null, không làm gì
      return prev.filter((_, i) => i !== index); // Loại bỏ phần tử tại vị trí index
    });
  };

  const removeVariantInProductsIsSelected = (productId: string, variantIndex: number) => {
    setProductsIsSelected((prev) => {
      if (!prev) return null; // Nếu danh sách ban đầu là null, không làm gì

      // Tìm sản phẩm có id === productId
      const productIndex = prev.findIndex((product) => product.id === productId);
      if (productIndex === -1) return prev; // Nếu không tìm thấy sản phẩm, trả về danh sách cũ

      // Tạo bản sao của danh sách sản phẩm
      const updatedProducts = [...prev];
      const product = updatedProducts[productIndex];

      // Xóa variant tại vị trí variantIndex
      product.variantObjs = product.variantObjs.filter((_, index) => index !== variantIndex);

      // Nếu variantObjs rỗng, xóa luôn sản phẩm
      if (product.variantObjs.length === 0) {
        updatedProducts.splice(productIndex, 1); // Xóa sản phẩm khỏi danh sách
      }

      return updatedProducts;
    });
  };

  //ren variant
  const [visibleVariants, setVisibleVariants] = useState<VariantObjs[]>([]); // Biến thể đang hiển thị
  const [fetchedVariants, setFetchedVariants] = useState<VoucherProductResDto[]>([]); // Biến thể từ API
  const [variantPage, setVariantPage] = useState<number>(1); // Trang hiện tại của biến thể
  const VARIANTS_PER_PAGE = 10; // Số lượng biến thể hiển thị mỗi lần

  const showVariantProducts = async (product: ProductDetailResDto) => {
    setVariantProduct(product); // Lưu sản phẩm hiện tại
    setVariantPage(1); // Đặt lại trang về 1
  
    // Lấy 10 biến thể đầu tiên
    const initialVariants = product.variantObjs.slice(0, VARIANTS_PER_PAGE);
  
    // Map 10 biến thể đầu tiên thành mảng VoucherProductReqDto[]
    const requestPayload: VoucherProductReqDto[] = initialVariants.map((variant) => ({
      voucherId: voucherId,
      productId: product.id,
      varientProductId: variant.sku,
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // GUID hợp lệ
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // GUID hợp lệ
      lastModifiedOnDate: new Date().toISOString(), // Giá trị hợp lệ
      createdOnDate: new Date().toISOString(), // Giá trị hợp lệ
    }));
  
    try {
      // Gọi API để kiểm tra 10 biến thể đầu tiên
      const response = await dispatch(findVoucherProductsByVcidPidVaid(requestPayload)).unwrap();

      setFetchedVariants(response); // Lưu danh sách biến thể đã tồn tại
      setVisibleVariants(initialVariants); // Hiển thị 10 biến thể đầu tiên
    } catch (error) {
      console.error("Error fetching voucher products:", error);
      setFetchedVariants([]);
    }
  };

  const loadMoreVariants = async () => {
    if (variantProduct) {
      const nextPage = variantPage + 1;
      const startIndex = variantPage * VARIANTS_PER_PAGE;
      const endIndex = nextPage * VARIANTS_PER_PAGE;
  
      // Lấy các biến thể tiếp theo
      const nextVariants = variantProduct.variantObjs.slice(startIndex, endIndex);
  
      // Map các biến thể tiếp theo thành mảng VoucherProductReqDto[]
      const requestPayload: VoucherProductReqDto[] = nextVariants.map((variant) => ({
        voucherId: voucherId,
        productId: variantProduct.id,
        varientProductId: variant.sku,
        createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedOnDate: new Date().toISOString(),
        createdOnDate: new Date().toISOString(),
      }));
  
      try {
        // Gọi API để kiểm tra các biến thể tiếp theo
        const response = await dispatch(findVoucherProductsByVcidPidVaid(requestPayload)).unwrap();
  
        // Cập nhật danh sách biến thể đã tìm thấy
        setFetchedVariants((prev) => [...prev, ...response]);
  
        // Cập nhật danh sách biến thể hiển thị
        setVisibleVariants((prev) => [...prev, ...nextVariants]);
        setVariantPage(nextPage);
      } catch (error) {
        console.error("Error fetching more voucher products:", error);
      }
    }
  };

  const isVariantSelected = (variant: VariantObjs) => {
    return (
      fetchedVariants.some(
        (voucherProduct) =>
          voucherProduct.voucherId === voucherId &&
          voucherProduct.productId === variantProduct?.id &&
          voucherProduct.varientProductId === variant.sku
      ) ||
      productsIsSelected?.some(
        (product) =>
          product.id === variantProduct?.id &&
          product.variantObjs.some((v) => v.sku === variant.sku)
      )
    );
  };

  //Xem chi tiết sản phẩm
  const [isOpenDetail, setIsOpenDetail] = useState(false);
  const [selectedDetailId, setSelectedDetailId] = useState<string | null>(
    null
  );
  const handleOpenDetail = (id: string) => {
    setIsOpenDetail(true);
    setSelectedDetailId(id);
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader className="sr-only">
          <SheetTitle></SheetTitle>
          <SheetDescription></SheetDescription>
        </SheetHeader>
        <h2 className="text-lg font-semibold">Thêm sản phẩm áp dụng Voucher</h2>
        <br />
          <form
            onSubmit={(e) => {
              e.preventDefault();
              handleFormSubmit(productsIsSelected);
            }}
            className="space-y-6 overflow-auto"
            >

            <h3 ref={errorRef} className="text-red-500 text-center">
              {errMs}
            </h3>

            <div className="p-5 m-5 border border-gray-300 rounded">
            {/* Tìm kiếm sản phẩm */}
            <div className="flex items-center">
              <Input
                placeholder="Tìm kiếm sản phẩm cần thêm dựa trên tên sản phẩm, mã sản phẩm hoặc mô tả"
                value={inputSearchString}
                onChange={(e) => setInputSearchString(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault(); // Ngăn hành vi mặc định của phím Enter
                    handleSubmitSearch(); // Gọi hàm tìm kiếm
                  }
                }}
                className="flex-1"
              />
              <button
                type="button"
                onClick={handleSubmitSearch}
                className={`p-2 ml-2 text-sm w-32 rounded font-semibold ${
                  isSearching ? "bg-gray-400 cursor-not-allowed" : "bg-blue-500 hover:bg-blue-600 text-white"
                }`}
                disabled={isSearching} // Disable nút khi đang tìm kiếm
                >
                {isSearching ? "Đang tìm..." : "Tìm kiếm"}
              </button>
            </div>
          
            {/* Danh sách sản phẩm */}
            <div className="flex">
              <div className="mt-4 border rounded p-2 w-[35%] mr-2 h-[60vh]">
                <div className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">Sản phẩm</div>
                {products !== null && products.length > 0 ? (
                  <ul className="space-y-2 max-h-[50vh] overflow-y-auto">
                    {products?.map((product) => (
                      <li 
                        onClick={() => showVariantProducts(product)}
                        key={product.id}
                        className="p-4 border rounded shadow hover:bg-gray-100"
                      >
                        <div className="flex">
                          <img className="w-[30%] h-[30%] rounded" src={product.imageUrl}/>
                          <div className="ml-2">
                            <h3 className="font-semibold">{product.name}</h3>
                            <p className="text-sm text-gray-500">{product.code}</p>
                            <button onClick={() => handleOpenDetail(product.id)} type="button" className="flex text-sm items-start text-blue-500 hover:text-blue-700" >
                              Chi tiết
                            </button>
                          </div>
                        </div>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p className="text-gray-500">Không tìm thấy sản phẩm nào.</p>
                )}
              </div>
              <div className="mt-4 border rounded p-2 w-[15%] mr-2">
                <div className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">
                  Biến thể
                </div>
                <div className="mb-2">
                  {/* Nút thêm hết */}
                  {variantProduct !== null && variantProduct.variantObjs.length > 0 ? (
                    <button
                      onClick={() => AddAllVariants(variantProduct)}
                      type="button"
                      className="w-full p-1 bg-blue-500 text-white rounded hover:bg-blue-600"
                    >
                      Thêm hết
                    </button>
                  ): (<div></div>)}
                </div>
                <div className="max-h-[43vh] overflow-y-auto">
                  {variantProduct !== null && visibleVariants.length > 0 ? (
                    <ul className="space-y-2">
                      {visibleVariants.map((variant) => (
                        <li
                          key={variant.sku}
                          className="p-4 border rounded shadow hover:bg-gray-100 relative"
                        >
                          <button
                            onClick={() => AddProductIsSelected(variantProduct, variant)}
                            type="button"
                            className={`absolute top-2 right-2 border rounded p-1 ${
                              isVariantSelected(variant)
                                ? "bg-gray-400 cursor-not-allowed"
                                : "bg-blue-500 text-white hover:bg-blue-600"
                            } text-xs`}
                            disabled={isVariantSelected(variant)} // Disable nút nếu variant đã được thêm
                          >
                            +
                          </button>
                          <div className="flex items-center">
                            <div className="w-full">
                              <h3 className="font-semibold">{variant.sku}</h3>
                              <p className="text-sm text-gray-500">{variant.price} VNĐ</p>
                            </div>
                          </div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-gray-500">Không tìm thấy biến thể của sản phẩm này.</p>
                  )}

                  {/* Nút "Xem thêm" */}
                  {variantProduct &&
                    visibleVariants.length < variantProduct.variantObjs.length && (
                      <button
                        type="button"
                        onClick={loadMoreVariants}
                        className="mt-2 p-2 text-blue-500 hover:underline"
                      >
                        Xem thêm
                      </button>
                    )}
                </div>
              </div>
              <div className="mt-4 border rounded p-2 w-[48%]">
                <div className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">
                  Đã chọn
                </div>
                <div className="mb-2">
                  {/* Nút xóa tất cả */}
                  {productsIsSelected !== null && productsIsSelected.length > 0 ? (
                    <button
                      onClick={() => removeAllProducts()}
                      type="button"
                      className="w-full p-1 bg-red-500 text-white rounded hover:bg-red-600"
                    >
                      Xóa tất cả
                    </button>
                  ): (<div></div>)}
                </div>
                <div className="max-h-[43vh] overflow-y-auto">
                  {productsIsSelected !== null && productsIsSelected.length > 0 ? (
                    <ul className="space-y-2">
                      {productsIsSelected.map((product, index) => (
                        <li 
                          key={product.id}
                          className="p-4 border rounded shadow hover:bg-gray-100 relative"
                        >
                          <button
                            onClick={() => removeProductInProductsIsSelected(index)}
                            type="button" 
                            className="absolute top-2 right-2 border rounded p-1 bg-red-500 text-white hover:bg-red-600 text-xs"
                          >
                            X
                          </button>
                          <div className="flex items-center gap-4">
                            <img className="w-[20%] rounded" src={product.imageUrl} alt={product.name} />
                            <div className="flex-1">
                              <h3 className="font-semibold">{product.name}</h3>
                              <p className="text-sm text-gray-500">{product.code}</p>
                            </div>
                          </div>
                          <div className="flex w-max-[40%] overflow-x-auto">
                            {product.variantObjs.map((variant, index) => (
                              <div className="relative p-2 border rounded mr-1 hover:bg-gray-100 bg-white" key={variant.sku}>
                                <button
                                  onClick={() => removeVariantInProductsIsSelected(product.id, index)}
                                  type="button"
                                  className="absolute top-2 right-2 border rounded p-1 bg-red-500 text-white hover:bg-red-600 text-xs"
                                >
                                  X
                                </button>
                                <h3 className="font-semibold mr-6">{variant.sku}</h3>
                                <p className="text-sm text-gray-500">{variant.price} VNĐ</p>
                              </div>
                            ))}
                          </div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-gray-500">Trống</p>
                  )}
                </div>
              </div>
            </div>

            <form>
              <Pagination                                   //fix lỗi form nhầm nút chuyển trang
                currentPage={pagination.currentPage}        //trong pagi là submit, bằng cách nhét pagi
                totalPages={pagination.totalPages}          //trong 1 form riêng, sau đó chặn hoàn toàn
                pageSize={pagination.pageSize}              //submit form riêng bằng cách cho validate
                totalRecords={pagination.totalRecords}      //của nó luôn luôn không thành công
                onPageChange={handlePageChange}
                onPageSizeChange={handlePageSizeChange}
              />
              <input required hidden/>
            </form>
          </div>
            <div className="flex justify-end gap-2 pt-4">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Đang thêm..." : "Thêm"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Thoát
              </Button>
            </div>
        </form>
        {isOpenDetail && selectedDetailId && (
          <DetailProductSheet
            productId={selectedDetailId}
            isOpen={isOpenDetail}
            onClose={() => setIsOpenDetail(false)}
          />
        )}
      </SheetContent>
    </Sheet>
  );
};

export default AddVoucherProductSheet;