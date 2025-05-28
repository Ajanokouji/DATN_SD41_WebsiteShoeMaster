import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
  selectCategories,
  selectPagination,
} from "@/redux/apps/category/categorySelector";
import {
  deleteCategory,
  fetchCategories,
  setPage,
  setPageSize,
} from "@/redux/apps/category/categorySlice";
import { CategoryResDto } from "@/types/category/category";
import DetailCategorySheet from "./UpdateDetail/DetailCategorySheet";
import { formatVietnamTime } from "@/utils/format";

const CategoryTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: TableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
    {/* <Table className="w-full">
      <TableHeaderComponent headers={headers} />
    </Table> */}
    <div className="max-h-[58vh] max-w-full overflow-x-auto overflow-y-auto">
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

const CategoriesTable: React.FC = () => {
  const dispatch = useAppDispatch();
  const categories = useAppSelector(selectCategories);
  const pagination = useAppSelector(selectPagination);
  const [isOpenUpdate, setIsOpenUpdate] = useState(false);
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);

  const handleOpenDialogUpdate = (id: string) => {
    setIsOpenUpdate(true);
    setSelectedCategoryId(id);
  };

  const renderCreatedDate = (value: string) => {
    return formatVietnamTime(value);
  };


  const headers = [
    { label: "Mã", className: "text-center" },
    { label: "Tên danh mục" },
    { label: "Mô tả" },
    { label: "Ngày tạo" },
    { label: " " },
  ];

  const columns: {
    key?: keyof CategoryResDto;
    className?: string;
    isActionColumn?: boolean;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    render?: (value: any) => React.ReactNode;
    action?: (data: CategoryResDto) => void;
    deleteAction?: (id: string) => void;
    updateAction?: (id: string) => void;
  }[] = [
    { key: "code", className: "text-center" },
    { key: "name" },
    { key: "description" },
    { 
      key: "createdOnDate", 
      render: renderCreatedDate 
    },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
      deleteAction: (id: string) => {
        dispatch(deleteCategory(id));
      },
      updateAction: (id: string) => {
        handleOpenDialogUpdate(id);
      },
    },
  ];

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const filters = {
      CurrentPage: pagination.currentPage,
      PageSize: pagination.pageSize,
      name: params.get("name") || "",
      code: params.get("code") || "",
      isDeleted: params.get("status") || "", // Truyền trạng thái isDeleted
      description: params.get("description") || "",
    };
  
    console.log("Filters sent to API:", filters); // Log để kiểm tra
    dispatch(fetchCategories(filters));
  }, [dispatch, location.search, pagination.currentPage, pagination.pageSize]);
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
      <CategoryTable<CategoryResDto>
        headers={headers}
        data={categories}
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
      {isOpenUpdate && selectedCategoryId && (
        <DetailCategorySheet
          categoryId={selectedCategoryId}
          isOpen={isOpenUpdate}
          onClose={() => setIsOpenUpdate(false)}
        />
      )}
    </section>
  );
};

export default CategoriesTable;
