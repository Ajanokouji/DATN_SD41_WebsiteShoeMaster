import React, { useEffect, useState } from "react";
import { Control, useFormContext } from "react-hook-form";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { VoucherFormSchema } from "./FormSchema";
import { FaStore, FaCartShopping, FaAddressCard, FaPhone, FaEnvelope } from "react-icons/fa6";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
  setProductPage,
  setProductPageSize,
  searchProduct,
  clearProducts,
} from "@/redux/apps/voucherProduct/voucherProductSlice";
import {
  selectProductPagination,
  selectProducts,
} from "@/redux/apps/voucherProduct/voucherProductSelector";
import Pagination from "@/components/Pagination";
import { ProductDetailResDto, VariantObjs } from "@/types/product/product";
import DetailProductSheet from "@/pages/product/sections/DetailProductSheet";
import { selectUserPagination, selectUsers } from "@/redux/apps/voucherUser/voucherUserSelector";
import { clearUsers, searchUser, setUserPage, setUserPageSize } from "@/redux/apps/voucherUser/voucherUserSlice";
import { UserResDtoForSearch } from "@/types/voucherUser/voucherUser";
import { FaHome } from "react-icons/fa";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";

interface BasicInfoFieldsProps {
  control: Control<VoucherFormSchema>;
  voucherType: number;
}

export const BasicInfoFields: React.FC<BasicInfoFieldsProps> = ({ control, voucherType }) => {
  const { watch, setValue, trigger } = useFormContext();
  const discountType = watch("discountType");

  const handleDiscountTypeChange = (val: "VNĐ" | "%") => {
    setValue("discountType", val);

    if (val === "VNĐ") {
      trigger("discountAmount");
    } else {
      trigger("discountPercentage");
      trigger("maxDiscountAmount");
    }
  };

  const getLocalDateTimeString = (date: Date) => {
    const pad = (n: number) => n.toString().padStart(2, '0');
  
    const year = date.getFullYear();
    const month = pad(date.getMonth() + 1);
    const day = pad(date.getDate());
    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
  
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  useEffect(() => {
    setValue("discountType", "VNĐ");
    setValue("status", 1);
    setValue("startDate", getLocalDateTimeString(new Date()));
    setValue("endDate", getLocalDateTimeString(new Date(Date.now() + 86400000)));
    setValue("displaySettings", 1);
  }, [setValue]);

  //search product
  const [searchString, setSearchString] = useState<string>("");
  const [inputSearchString, setInputSearchString] = useState<string>("");
  const dispatch = useAppDispatch();
  const products = useAppSelector(selectProducts);
  const pagination = useAppSelector(selectProductPagination);
  const [isSearching, setIsSearching] = useState<boolean>(false);
  const [variantProduct, setVariantProduct] = useState<ProductDetailResDto | null>(null);
  const [isOpenFormAddVoucherProduct, setValueIsOpenFormAddVP] = useState(false);
  const [productsIsSelected, setProductsIsSelected] = useState<ProductDetailResDto[] | null>(null);

  const openFormAddVoucherProduct = () => {
    setIsSearching(false);
    setVariantProduct(null);
    setValueIsOpenFormAddVP(true);
    setSearchString("");
    setInputSearchString("");
    setProductsIsSelected(null);
    setProductPage(0);
    setProductPageSize(20);
    dispatch(clearProducts());
  }

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

  const showVariantProducts = (product: ProductDetailResDto) => {
    setVariantProduct(product);
  }

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

  const AddAllVariants = (product: ProductDetailResDto) => {
    setProductsIsSelected((prev) => {
      if (!prev) {
        // Nếu danh sách ban đầu là null, thêm sản phẩm mới với tất cả biến thể
        const newProduct = { ...product, variantObjs: [...product.variantObjs] };
        setValue("productsIsSelected", [newProduct]); // Lưu vào form state
        return [newProduct];
      }

      // Kiểm tra xem sản phẩm đã tồn tại trong danh sách chưa
      const existingProductIndex = prev.findIndex((p) => p.id === product.id);

      if (existingProductIndex === -1) {
        // Nếu sản phẩm chưa tồn tại, thêm sản phẩm mới với tất cả biến thể
        const newProduct = { ...product, variantObjs: [...product.variantObjs] };
        const updatedProducts = [newProduct, ...prev];
        setValue("productsIsSelected", updatedProducts); // Lưu vào form state
        return updatedProducts;
      } else {
        // Nếu sản phẩm đã tồn tại, thêm các biến thể chưa có vào danh sách variantObjs
        const updatedProducts = [...prev];
        const existingProduct = updatedProducts[existingProductIndex];

        // Lọc các biến thể chưa tồn tại
        const newVariants = product.variantObjs.filter(
          (variant) =>
            !existingProduct.variantObjs.some((v) => v.sku === variant.sku)
        );

        // Thêm các biến thể mới vào danh sách variantObjs
        existingProduct.variantObjs = [...existingProduct.variantObjs, ...newVariants];

        setValue("productsIsSelected", updatedProducts); // Lưu vào form state
        return updatedProducts;
      }
    });
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

  // Thêm các trạng thái và logic tìm kiếm user
  const [searchUserString, setSearchUserString] = useState<string>("");
  const [inputSearchUserString, setInputSearchUserString] = useState<string>("");
  const [isSearchingUser, setIsSearchingUser] = useState<boolean>(false);
  const users = useAppSelector(selectUsers);
  const userPagination = useAppSelector(selectUserPagination);
  const [showUserSearch, setShowUserSearch] = useState(false);
  
  const openUserSearch = () => {
    setShowUserSearch(true);
    setInputSearchUserString("");
    setSearchUserString("");
    setIsSearchingUser(false);
    dispatch(clearUsers());
    setSelectedUsers([]);
  };

  const handleSubmitSearchUser = async () => {
    setIsSearchingUser(true); // Bắt đầu tìm kiếm
    try {
      await dispatch(
        searchUser({
          userName: inputSearchUserString,
          CurrentPage: 1,
          PageSize: userPagination.pageSize,
        })
      ).unwrap(); // Chờ kết quả từ Redux Thunk
    } catch (error) {
      console.error("Search user failed:", error); // Xử lý lỗi nếu có
    } finally {
      setIsSearchingUser(false); // Kết thúc tìm kiếm
    }
  };

  const handleUserPageChange = async (newPage: number) => {
    dispatch(setUserPage(newPage));
    try {
      await dispatch(
        searchUser({
          userName: inputSearchUserString,
          CurrentPage: newPage,
          PageSize: userPagination.pageSize,
        })
      ).unwrap(); // Chờ kết quả từ Redux Thunk
    } catch (error) {
      console.error("Search user failed:", error);
    }
  };

  const handleUserPageSizeChange = async (newSize: number) => {
    dispatch(setUserPageSize(newSize));
    try {
      await dispatch(
        searchUser({
          userName: inputSearchUserString,
          CurrentPage: 1,
          PageSize: newSize,
        })
      ).unwrap(); // Chờ kết quả từ Redux Thunk
    } catch (error) {
      console.error("Search user failed:", error);
    }
  };

  //Thêm khách hàng áp dụng Voucher
  const [selectedUsers, setSelectedUsers] = useState<UserResDtoForSearch[]>([]);
  const addUserToSelectedList = (user: UserResDtoForSearch) => {
    setSelectedUsers((prev) => {
      // Kiểm tra nếu user đã tồn tại trong danh sách "Đã chọn"
      const isUserAlreadySelected = prev.some((selectedUser) => selectedUser.id === user.id);
      if (isUserAlreadySelected) return prev; // Nếu đã tồn tại, không thêm lại

      const updatedUsers = [user, ...prev]; // Thêm user mới vào đầu danh sách
      setValue("usersIsSelected", updatedUsers); // Lưu vào FormSchema
      return updatedUsers;
    });
  };

  const removeUserFromSelectedList = (userId: string) => {
    setSelectedUsers((prev) => {
      const updatedUsers = prev.filter((user) => user.id !== userId); // Loại bỏ user khỏi danh sách
      setValue("usersIsSelected", updatedUsers); // Lưu vào FormSchema
      return updatedUsers;
    });
  };

  const clearAllSelectedUsers = () => {
    setSelectedUsers([]); // Đặt danh sách về rỗng
    setValue("usersIsSelected", []); // Cập nhật FormSchema
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
    <div className="space-y-4">
      <div>
        <h2 className="text-lg font-semibold">Thông tin cơ bản</h2>
        <div className="mt-10 ml-5">
          <div className="mb-5">
            <FormField
              control={control}
              name="voucherName"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap4">
                    <FormLabel className="w-32">Tên Voucher</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        placeholder="Nhập tên voucher tối đa 100 ký tự"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("voucherName");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
          </div>
          <div className="mb-5">
            <FormField
              control={control}
              name="code"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Mã Voucher</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        placeholder="Nhập mã voucher tối đa 10 ký tự, chỉ bao gồm chữ cái và số, vui lòng không nhập các ký tự đặc biệt và khoảng trắng"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("code");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
          </div>
          <div className="flex mb-10">
            <FormField
              control={control}
              name="startDate"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Thời gian lưu mã</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        type="datetime-local"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("startDate");
                          trigger("endDate");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
            <div className="pl-5 pr-5">__</div>
            <FormField
              control={control}
              name="endDate"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center">
                    <FormControl>
                      <Input
                        type="datetime-local"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("endDate");
                          trigger("startDate");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center" />
                </FormItem>
              )}
            />
          </div>
        </div>
      </div>

      <hr className="border border-gray-300"/>

      <div className="mt-10">
        <div className="mt-10">
          <h2 className="text-lg font-semibold">Thiết lập mã giảm giá</h2>
        </div>
        <div className="mt-10 ml-5">
          <div className="mt-5">
            {discountType === "VNĐ" && (
              <FormField
                control={control}
                name="discountAmount"
                render={({ field }) => (
                  <FormItem>
                    <div className="flex items-center gap-4">
                      <FormLabel className="w-32">Giá trị giảm</FormLabel>
                      <div className="flex items-center gap-2 flex-1">
                        <FormControl>
                          <Input
                            type="number"
                            placeholder="Nhập số tiền giảm, phải lớn hơn 0"
                            {...field}
                            onChange={(e) => {
                              field.onChange(e);
                              trigger("discountAmount");
                            }}
                          />
                        </FormControl>
                        <Select
                          value={discountType}
                          onValueChange={(val) => handleDiscountTypeChange(val as "VNĐ" | "%")}
                        >
                          <SelectTrigger className="w-20">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectItem value="VNĐ">VNĐ</SelectItem>
                            <SelectItem value="%">%</SelectItem>
                          </SelectContent>
                        </Select>
                      </div>
                    </div>
                    <FormMessage className="text-center ml-32"/>
                  </FormItem>
                )}
              />
            )}

            {discountType === "%" && (
              <div className="flex gap-4">
                {/* Giá trị giảm */}
                <FormField
                  control={control}
                  name="discountPercentage"
                  render={({ field }) => (
                    <FormItem className="flex-1">
                      <div className="flex items-center gap-2">
                        <FormLabel className="w-32">Giá trị giảm</FormLabel>
                        <FormControl className="flex-1">
                          <Input
                            type="number"
                            placeholder="Nhập phần trăm giảm, 0 < x <= 100"
                            {...field}
                            onChange={(e) => {
                              field.onChange(e);
                              trigger("discountPercentage");
                            }}
                          />
                        </FormControl>
                        <Select
                          value={discountType}
                          onValueChange={(val) => handleDiscountTypeChange(val as "VNĐ" | "%")}
                        >
                          <SelectTrigger className="w-16">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectItem value="VNĐ">VNĐ</SelectItem>
                            <SelectItem value="%">%</SelectItem>
                          </SelectContent>
                        </Select>
                      </div>
                      <FormMessage className="text-center ml-20" />
                    </FormItem>
                  )}
                />

                {/* Tối đa */}
                <FormField
                  control={control}
                  name="maxDiscountAmount"
                  render={({ field }) => (
                    <FormItem className="flex-1 ml-9">
                      <div className="flex items-center gap-2">
                        <FormLabel className="w-20">Tối đa</FormLabel>
                        <FormControl className="flex-1">
                          <Input
                            type="number"
                            placeholder="Nhập số tiền giảm tối đa, phải lớn hơn 0"
                            {...field}
                            onChange={(e) => {
                              field.onChange(e);
                              trigger("maxDiscountAmount");
                            }}
                          />
                        </FormControl>
                      </div>
                      <FormMessage className="text-center ml-20" />
                    </FormItem>
                  )}
                />
              </div>
            )}
          </div>
          <div className="mt-5">
            <FormField
              control={control}
              name="minimumOrderAmount"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Giá trị đơn hàng tối thiểu</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        type="number"
                        placeholder="Nhập giá trị đơn hàng tối thiểu, phải lớn hơn hoặc bằng 0"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("minimumOrderAmount");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
          </div>
          <div className="mt-5">
            <FormField
              control={control}
              name="totalMaxUsage"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Tổng lượt sử dụng tối đa</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        type="number"
                        placeholder="Nhập tổng số mã giảm giá có thể sử dụng, phải lớn hơn 0"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("totalMaxUsage");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
          </div>
          <div className="mt-5">
            <FormField
              control={control}
              name="maxUsagePerCustomer"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Lượt sử dụng tối đa/người mua</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        type="number"
                        placeholder="Nhập số lượt sử dụng tối đa trên một người mua, phải lớn hơn 0 và không được lớn hơn Tổng lượt sử dụng tối đa"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("maxUsagePerCustomer");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
          </div>
          <div className="mt-5">
            <FormField
              control={control}
              name="status"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Trạng thái</FormLabel>
                    <Select
                      value={String(field.value)}
                      onValueChange={(val) => {
                        field.onChange(Number(val));
                        trigger("status");
                      }}
                    >
                      <FormControl className="flex-1">
                        <SelectTrigger>
                          <SelectValue placeholder="Chọn trạng thái" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="1">Hoạt động</SelectItem>
                        <SelectItem value="0">Không hoạt động</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <FormMessage className="text-center" />
                </FormItem>
              )}
            />
          </div>
          <div className="mt-5 mb-10">
            <FormField
              control={control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Mô tả</FormLabel>
                    <FormControl className="flex-1">
                      <Textarea
                        placeholder="Nhập mô tả dưới 256 ký tự"
                        {...field}
                        onChange={(e) => {
                          field.onChange(e);
                          trigger("description");
                        }}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center" />
                </FormItem>
              )}
            />
          </div>
        </div>
      </div>

      {/* <hr className="border border-gray-300"/>

      <div className="mt-10 pb-5">
        <div className="mt-10">
          <h2 className="text-lg font-semibold">Các sản phẩm áp dụng</h2>
        </div>
        <div className="mt-5 ml-5">
          <div className="flex items-center gap-4">
            <FormLabel className="w-32">Sản phẩm áp dụng</FormLabel>
            <div>
              {voucherType === 1 && (
                <div className="border border-gray-900 rounded p-1 hover:bg-gray-200">
                  Tất cả sản phẩm
                </div>
              )}
              {voucherType === 2 && (
                <Button onClick={openFormAddVoucherProduct} type="button" variant={"outline"} className="flex items-center">
                  <Plus size={16} /> Thêm sản phẩm
                </Button>
              )}
            </div>
          </div>
        </div>
        { isOpenFormAddVoucherProduct && (
          <div className="p-5 m-5 border border-gray-300 rounded">
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
                {variantProduct !== null && variantProduct.variantObjs.length > 0 ? (
                  <>
                    <ul className="space-y-2">
                      {variantProduct.variantObjs.map((variant) => {
                        // Kiểm tra xem variant đã được thêm vào productsIsSelected hay chưa
                        const isVariantSelected = productsIsSelected?.some(
                          (product) =>
                            product.id === variantProduct.id && // Kiểm tra sản phẩm
                            product.variantObjs.some((v) => v.sku === variant.sku) // Kiểm tra biến thể
                        );

                        return (
                          <li
                            key={variant.sku}
                            className="p-4 border rounded shadow hover:bg-gray-100 relative"
                          >
                            <button
                              onClick={() => AddProductIsSelected(variantProduct, variant)}
                              type="button"
                              className={`absolute top-2 right-2 border rounded p-1 ${
                                isVariantSelected
                                  ? "bg-gray-400 cursor-not-allowed"
                                  : "bg-blue-500 text-white hover:bg-blue-600"
                              } text-xs`}
                              disabled={isVariantSelected} // Disable nút nếu variant đã được thêm
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
                        );
                      })}
                    </ul>
                  </>
                ) : (
                  <p className="text-gray-500">Không tìm thấy biến thể của sản phẩm này.</p>
                )}
              </div>
            </div>
            <div className="mt-4 border rounded p-2 w-[48%]">
              <div className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">
                Đã chọn
              </div>
              <div className="mb-2">
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
                          <img
                            className="w-[20%] rounded"
                            src={product.imageUrl}
                            alt={product.name}
                          />
                          <div className="flex-1">
                            <h3 className="font-semibold">{product.name}</h3>
                            <p className="text-sm text-gray-500">{product.code}</p>
                          </div>
                        </div>
                        <div className="flex w-max-[40%] overflow-x-auto">
                          {product.variantObjs.map((variant, index) => (
                            <div
                              className="relative p-2 border rounded mr-1"
                              key={variant.sku}
                            >
                              <button
                                onClick={() =>
                                  removeVariantInProductsIsSelected(product.id, index)
                                }
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

            {isOpenDetail && selectedDetailId && (
              <DetailProductSheet
                productId={selectedDetailId}
                isOpen={isOpenDetail}
                onClose={() => setIsOpenDetail(false)}
              />
            )}
        </div>
        )}
      </div> */}

      <hr className="border border-gray-300"/>

      <div className="pb-3">
        <div className="mt-9">
          <h2 className="text-lg font-semibold">Thêm vào kho Voucher của khách hàng</h2>
        </div>
        <div className="mt-5 ml-5">
          <Button onClick={openUserSearch} type="button" variant={"outline"} className="flex items-center">
            <Plus size={16} /> Thêm khách hàng
          </Button>
        </div>
        
        {/*Tìm kiếm khách hàng*/}
        {showUserSearch === true && (
          <div className="mt-2 pb-5 ml-5">
            <div className="mt-5 mr-5 border rounded p-5">
              {/* Tìm kiếm user */}
              <div className="flex items-center">
                <Input
                  placeholder="Tìm kiếm khách hàng dựa trên tên tài khoản, tên khách hàng, số điện thoại, email, địa chỉ"
                  value={inputSearchUserString}
                  onChange={(e) => setInputSearchUserString(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault(); // Ngăn hành vi mặc định của phím Enter
                      handleSubmitSearchUser(); // Gọi hàm tìm kiếm
                    }
                  }}
                  className="flex-1"
                />
                <button
                  type="button"
                  onClick={handleSubmitSearchUser}
                  className={`p-2 ml-2 text-sm w-32 rounded font-semibold ${
                    isSearchingUser ? "bg-gray-400 cursor-not-allowed" : "bg-blue-500 hover:bg-blue-600 text-white"
                  }`}
                  disabled={isSearchingUser} // Disable nút khi đang tìm kiếm
                >
                  {isSearchingUser ? "Đang tìm..." : "Tìm kiếm"}
                </button>
              </div>

              <div className="flex">
                {/* Danh sách user */}
                <div className="mt-4 border rounded p-2 w-[50%] h-[60vh]">
                  <div className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">Danh sách khách hàng</div>
                  {users !== null && users.length > 0 ? (
                    <ul className="space-y-2 max-h-[50vh] overflow-y-auto">
                      {users.map((user) => {
                        // Kiểm tra nếu user đã tồn tại trong danh sách "Đã chọn"
                        const isUserSelected = selectedUsers.some((selectedUser) => selectedUser.id === user.id);

                        // Xác định trạng thái và lớp CSS
                        const isActive = user?.isActive;
                        let borderColor = "border-gray-400"; // Mặc định là xám
                        let statusText = "UNDEFINED";
                        let statusBgColor = "bg-gray-400";

                        if (isActive === true) {
                          borderColor = "border-green-500";
                          statusText = "ACTIVE";
                          statusBgColor = "bg-green-500";
                        } else if (isActive === false) {
                          borderColor = "border-red-500";
                          statusText = "INACTIVE";
                          statusBgColor = "bg-red-500";
                        }

                        return (
                          <li
                            key={user.id}
                            className="p-4 border rounded shadow hover:bg-gray-100 relative"
                          >
                            <button
                              onClick={() => addUserToSelectedList(user)}
                              type="button"
                              className={`absolute top-2 right-2 border rounded p-1 ${
                                isUserSelected
                                  ? "bg-gray-400 cursor-not-allowed"
                                  : "bg-blue-500 text-white hover:bg-blue-600"
                              } text-xs`}
                              disabled={isUserSelected} // Disable nút nếu user đã được chọn
                            >
                              +
                            </button>
                            <div className="flex items-center">
                              <div className="relative">
                                <img
                                  className={`object-cover w-16 h-16 border-2 rounded-full mr-5 ${borderColor}`}
                                  src={user.avartarUrl || "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg"}
                                  alt={user.name || "User"}
                                />
                                {/* Hiển thị trạng thái */}
                                <span
                                  className={`absolute px-1 bottom-0 left-1/2 transform -translate-x-1/2 text-xs font-semibold text-white rounded-full ${statusBgColor}`}
                                >
                                  {statusText}
                                </span>
                              </div>
                              
                              <div className="flex-1">
                                <p className="flex text-start text-sm font-semibold">
                                  <FaAddressCard className="mr-2 mt-1"/>{user.name} {user.username?"- " + user.username:""}
                                </p>
                                <p className="flex text-start text-sm text-gray-500">
                                  <FaPhone className="mr-2 mt-1"/>{user.phoneNumber}
                                </p>
                                <p className="flex text-start text-sm text-gray-500">
                                  <FaEnvelope className="mr-2 mt-1"/>{user.email}
                                </p>
                                <p className="flex text-start text-sm text-gray-500">
                                  <FaHome className="mr-2 mt-1"/>{user.address}
                                </p>
                              </div>
                            </div>
                          </li>
                        );
                      })}
                    </ul>
                  ) : (
                    <p className="text-gray-500">Không tìm thấy khách hàng nào.</p>
                  )}
                </div>

                {/* Danh sách đã chọn */}
                <div className="ml-1 mt-4 border rounded p-2 w-[50%] h-[60vh]">
                  <div className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">Đã chọn</div>
                  <div className="mb-2">
                    {/* Nút xóa tất cả */}
                    {selectedUsers.length > 0 && (
                      <button
                        onClick={clearAllSelectedUsers} // Gọi hàm xóa tất cả
                        type="button"
                        className="w-full p-1 bg-red-500 text-white rounded hover:bg-red-600"
                      >
                        Xóa tất cả
                      </button>
                    )}
                  </div>
                  {selectedUsers.length > 0 ? (
                    <ul className="space-y-2 max-h-[44vh] overflow-y-auto">
                      {selectedUsers.map((user) => {
                        // Xác định trạng thái và lớp CSS
                        const isActive = user?.isActive;
                        let borderColor = "border-gray-400"; // Mặc định là xám
                        let statusText = "UNDEFINED";
                        let statusBgColor = "bg-gray-400";

                        if (isActive === true) {
                          borderColor = "border-green-500";
                          statusText = "ACTIVE";
                          statusBgColor = "bg-green-500";
                        } else if (isActive === false) {
                          borderColor = "border-red-500";
                          statusText = "INACTIVE";
                          statusBgColor = "bg-red-500";
                        }

                        return (
                          <li
                            key={user.id}
                            className="p-4 border rounded shadow hover:bg-gray-100 relative"
                          >
                            <button
                              onClick={() => removeUserFromSelectedList(user.id)}
                              type="button"
                              className="absolute top-2 right-2 border rounded p-1 bg-red-500 text-white hover:bg-red-600 text-xs"
                            >
                              X
                            </button>
                            <div className="flex items-center">
                              <div className="relative">
                                <img
                                  className={`object-cover w-16 h-16 border-2 rounded-full mr-5 ${borderColor}`}
                                  src={user.avartarUrl || "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg"}
                                  alt={user.name || "User"}
                                />
                                {/* Hiển thị trạng thái */}
                                <span
                                  className={`absolute px-1 bottom-0 left-1/2 transform -translate-x-1/2 text-xs font-semibold text-white rounded-full ${statusBgColor}`}
                                >
                                  {statusText}
                                </span>
                              </div>
                              
                              <div className="flex-1">
                                <p className="flex text-start text-sm font-semibold">
                                  <FaAddressCard className="mr-2 mt-1"/>{user.name} {user.username?"- " + user.username:""}
                                </p>
                                <p className="flex text-start text-sm text-gray-500">
                                  <FaPhone className="mr-2 mt-1"/>{user.phoneNumber}
                                </p>
                                <p className="flex text-start text-sm text-gray-500">
                                  <FaEnvelope className="mr-2 mt-1"/>{user.email}
                                </p>
                                <p className="flex text-start text-sm text-gray-500">
                                  <FaHome className="mr-2 mt-1"/>{user.address}
                                </p>
                              </div>
                            </div>
                          </li>
                        );
                      })}
                    </ul>
                  ) : (
                    <p className="text-gray-500">Chưa có khách hàng nào được chọn.</p>
                  )}
                </div>
              </div>

              {/* Phân trang */}
              <form>
                <Pagination                                   
                  currentPage={userPagination.currentPage}
                  totalPages={userPagination.totalPages}
                  pageSize={userPagination.pageSize}
                  totalRecords={userPagination.totalRecords}
                  onPageChange={handleUserPageChange}
                  onPageSizeChange={handleUserPageSizeChange}
                />
                <input required hidden/>
              </form>
            </div>
          </div>
          )}
      </div>
    </div>
  );
};