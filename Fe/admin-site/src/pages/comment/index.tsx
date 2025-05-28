import React, { useEffect, useState } from "react";
import CommentsTable from "./sections/TableData";
import { useParams } from "react-router-dom";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { fetchProductById } from "@/redux/apps/product/productSlice";
import { selectProduct } from "@/redux/apps/product/productSelector";
import { Helmet } from "react-helmet-async";
import DetailProductSheet from "../product/sections/DetailProductSheet";

const VoucherUsers: React.FC = () => {
  const {objectId} = useParams();
  const dispatch = useAppDispatch();
  const product = useAppSelector(selectProduct);

  const [isLoading, setIsLoading] = useState(false);
  const [openProductDetail, setOpenProductDetail] = useState(false);

  // Fetch product khi có ObjectId
  useEffect(() => {
    if (objectId) {
      setIsLoading(true); // Bắt đầu loading
      dispatch(fetchProductById(objectId))
        .unwrap()
        .catch((error) => {
          console.error("Error fetching product:", error);
        })
        .finally(() => {
          setIsLoading(false); // Kết thúc loading
        });
    }
  }, [objectId, dispatch]);

  // Các điều kiện kiểm tra dữ liệu
  if (isLoading) {
    return (
      <div className="text-center text-blue-500">
        Đang tải...
      </div>
    );
  }
  
  if (!objectId) {
    return (
      <div className="text-center text-red-500">
        Không tìm thấy Sản phẩm! Vui lòng kiểm tra lại!
      </div>
    );
  }

  if (!product) {
    return (
      <div className="text-center text-red-500">
        Không tìm thấy Sản phẩm! Vui lòng kiểm tra lại!
      </div>
    );
  }

  if (product.id != objectId) {
    return (
      <div className="text-center text-red-500">
        Không tìm thấy Sản phẩm! Vui lòng kiểm tra lại!
      </div>
    );
  }

  return (
    <>
      <section className="px-8">
        <Helmet>
          <title>Danh sách đánh giá sản phẩm {product.name}</title>
        </Helmet>
        <div className="flex items-center gap-2 mb-2">
          <h2 className="text-2xl">
            Danh sách đánh giá sản phẩm: {product.name}
          </h2>
          <button
            type="button"
            className="p-1 rounded hover:bg-blue-100 text-blue-600"
            title="Xem chi tiết sản phẩm"
            onClick={() => setOpenProductDetail(true)}
          >
            {/* icon con mắt */}
            <svg width="20" height="20" fill="none" viewBox="0 0 24 24">
              <path stroke="currentColor" strokeWidth="2" d="M1 12s4-7 11-7 11 7 11 7-4 7-11 7S1 12 1 12Zm11 3a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z"/>
            </svg>
          </button>
        </div>
        <CommentsTable objectId={objectId} />
      </section>
      <DetailProductSheet
        productId={product.id}
        isOpen={openProductDetail}
        onClose={() => setOpenProductDetail(false)}
      />
    </>
  );
};

export default VoucherUsers;