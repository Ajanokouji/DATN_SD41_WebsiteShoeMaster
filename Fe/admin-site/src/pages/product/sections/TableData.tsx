import React, { useEffect } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";

import { ProductResDto } from "@/types/product/product";
import { deleteProduct, fetchProducts, setPage, setPageSize } from "@/redux/apps/product/productSlice";
import { selectPagination, selectProducts } from "@/redux/apps/product/productSelector";

const ProductTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: TableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
    {/* <Table className="w-full">
      <TableHeaderComponent headers={headers} />
    </Table> */}
    <div className="max-h-80 max-w-full overflow-x-auto overflow-y-auto">
      <Table className="w-full">
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
);

const ProductsTable: React.FC = () => {
  const headers = [
    { label: "Code", className: "text-center" },
    { label: "Name" },
    { label: "Image" },
    { label: "Description" },
    { label: "Status" },
    { label: "Create At" },
    { label: " " },
  ];

  const columns: {
    key?: keyof ProductResDto;
    className?: string;
    isActionColumn?: boolean;
    action?: (data: ProductResDto) => void;
    deleteAction?: (id: string) => void;
  }[] = [
    { key: "code", className: "text-center" },
    { key: "name" },
    { key: "imageUrl" },
    { key: "description" },
    { key: "status" },
    { key: "createdOnDate" },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
      deleteAction: (id: string) => {
        dispatch(deleteProduct(id));
      },
    },
  ];

  const dispatch = useAppDispatch();
  const products = useAppSelector(selectProducts);
  const pagination = useAppSelector(selectPagination);

  useEffect(() => {
    dispatch(
      fetchProducts({
        CurrentPage: pagination.currentPage,
        PageSize: pagination.pageSize,
      })
    );
  }, [dispatch, pagination.currentPage, pagination.pageSize]);

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
    </section>
  );
};

export default ProductsTable;
