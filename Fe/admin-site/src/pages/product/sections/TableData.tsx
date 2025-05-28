import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";

import { ProductResDto } from "@/types/product/product";
import {
  deleteProduct,
  fetchProducts,
  setPage,
  setPageSize,
} from "@/redux/apps/product/productSlice";
import {
  selectPagination,
  selectProducts,
} from "@/redux/apps/product/productSelector";
import UpdateProductSheet from "./UpdateDetail/DetailProductSheet";
import { formatVietnamTime } from "@/utils/format";
import { useLocation } from "react-router-dom";
import DetailProductSheet from "./DetailProductSheet";


const ProductTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: TableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
    {/* <Table className="w-full">
      <TableHeaderComponent headers={headers} />
    </Table> */}
    <div className="max-h-[58vh] max-w-full overflow-x-auto overflow-y-auto">
    <div className="w-full overflow-auto max-w-[1400px] mx-auto">
  <Table className="min-w-full">
    <TableHeaderComponent headers={headers} className="text-center" />
    <TableBody>
      {data.length ? (
        data.map((row, index) => (
          <TableRowComponent key={index} data={row} columns={columns} />
        ))
      ) : (
        <TableRowComponent
          key="empty-row"
          columns={columns}
          data={undefined}
        />
      )}
    </TableBody>
  </Table>
</div>

    </div>
  </div>
);

interface ProductsTableProps {
  selectedCategoryId: string | null; // Nhận id danh mục từ props
}


const ProductsTable: React.FC<ProductsTableProps> = ({ selectedCategoryId }) => {
  const dispatch = useAppDispatch();
  const products = useAppSelector(selectProducts);
  const pagination = useAppSelector(selectPagination);
  const location = useLocation();
  const [isOpenDetail, setIsOpenDetail] = useState(false);
  const [selectedDetailId, setSelectedDetailId] = useState<string | null>(
    null
  );
  const [isOpenUpdate, setIsOpenUpdate] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState<string | null>(
    null
  );

  const handleOpenDialogUpdate = (id: string) => {
    setIsOpenUpdate(true);
    setSelectedProductId(id);
  };
  const handleOpenDetail = (id: string) => {
    setIsOpenDetail(true);
    setSelectedDetailId(id);
  };

  const renderCreatedDate = (value: string) => {
    return formatVietnamTime(value);
  };
  const headers = [
    { label: "Mã Sản Phẩm", className: "text-center" },
    { label: "Tên Sản Phẩm" },
    { label: "Hình Ảnh" },
    { label: "Mô Tả" },
    { label: "Trạng Thái" },
    { label: "Ngày Tạo" },
    { label: " " },
  ];

  const columns: {
      key?: keyof ProductResDto;
      className?: string;
      isActionColumn?: boolean;
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      render?: (value: any, row?: any) => React.ReactNode;
      action?: (data: ProductResDto) => void;
      deleteAction?: (id: string) => void;
      updateAction?: (id: string) => void;
      detailAction?: (id: string) => void;
      starAction?: (id: string) => void; // Thêm action cho sao
    }[] = [
    { key: "code", className: "text-center" },
    { key: "name" },
    { key: "imageUrl" },
    { key: "description" },
    { key: "status" },
    {
      key: "createdOnDate",
      render: renderCreatedDate,
    },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
      deleteAction: (id: string) => {
        dispatch(deleteProduct(id));
      },
      updateAction: (id: string) => {
        handleOpenDialogUpdate(id);
      },
      detailAction: (id: string) => {
        handleOpenDetail(id);
      },
        starAction: (id: string) => {
        window.open(`/comment/${id}`, "_blank");
      },
    },
  ];

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const filters = {
      CurrentPage: pagination.currentPage,
      PageSize: pagination.pageSize,
      tenSanPham: params.get("tenSanPham") || "",
      maSanPham: params.get("maSanPham") || "",
      status: params.get("status") || "",
      description: params.get("description") || "",
      mainCategoryId : selectedCategoryId || null,
    };
    dispatch(fetchProducts(filters));
  }, [dispatch, location.search, pagination.currentPage, pagination.pageSize,selectedCategoryId]);

  // Xử lý khi thay đổi trang
  const handlePageChange = (newPage: number) => {
    dispatch(setPage(newPage));
  };

  // Xử lý khi thay đổi số lượng sản phẩm trên trang
  const handlePageSizeChange = (newSize: number) => {
    dispatch(setPageSize(newSize));
  };

  return (
    <section className="mt-10">
      <ProductTable<ProductResDto>
        headers={headers}
        data={products}
        columns={columns}
      />
      <Pagination
        currentPage={pagination.currentPage}
        totalPages={pagination.totalPages}
        pageSize={pagination.pageSize}
        totalRecords={pagination.totalRecords}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />
      {isOpenUpdate && selectedProductId && (
        <UpdateProductSheet
          productId={selectedProductId}
          isOpen={isOpenUpdate}
          onClose={() => setIsOpenUpdate(false)}
        />
      )}
      {isOpenDetail && selectedDetailId && (
        <DetailProductSheet
          productId={selectedDetailId}
          isOpen={isOpenDetail}
          onClose={() => setIsOpenDetail(false)}
        />
      )}
    </section>
  );
};

export default ProductsTable;
