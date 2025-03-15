import React, { useEffect } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { RelationResDto } from "@/types/category/relation";
import {
  deleteRelation,
  fetchRelations,
  setPage,
  setPageSize,
} from "@/redux/apps/relation/relationSlice";
import {
  selectPagination,
  selectRelations,
} from "@/redux/apps/relation/relationSelector";

const RelationTable = <T extends { id: string }>({
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

interface RelationsTableProps {
  selectedCategoryId: string | null;
}

const RelationsTable: React.FC<RelationsTableProps> = ({
  selectedCategoryId,
}) => {
  const dispatch = useAppDispatch();
  const categories = useAppSelector(selectRelations);
  const pagination = useAppSelector(selectPagination);

  const headers = [
    { label: "Category Name", className: "text-center" },
    { label: "Product Name" },
    { label: "Description" },
    { label: "Create At" },
    { label: " " },
  ];

  const columns: {
    key?: keyof RelationResDto;
    className?: string;
    isActionColumn?: boolean;
    action?: (data: RelationResDto) => void;
    deleteAction?: (id: string) => void;
  }[] = [
    { key: "categoryName", className: "text-center" },
    { key: "productName" },
    { key: "description" },
    { key: "createdOnDate" },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
      deleteAction: (id: string) => {
        dispatch(deleteRelation(id));
      },
    },
  ];

  useEffect(() => {
    if (selectedCategoryId) {
      dispatch(
        fetchRelations({
          IdDanhMuc: selectedCategoryId,
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
        })
      );
    } else {
      dispatch(
        fetchRelations({
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
        })
      );
    }
  }, [
    dispatch,
    pagination.currentPage,
    pagination.pageSize,
    selectedCategoryId,
  ]);

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
      <RelationTable<RelationResDto>
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
    </section>
  );
};

export default RelationsTable;
