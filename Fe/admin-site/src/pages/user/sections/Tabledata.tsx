import React from 'react'
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import {
  deleteUser,
  fetchUsers,
  setPage,
  setPageSize,
} from "@/redux/apps/user/userSlice";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
    selectPagination,
    selectUsers
  } from "@/redux/apps/user/userSelector";
const UserTable = <T extends { id: string }>({
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

 

  

const UsersTable: React.FC = () => {
    const dispatch = useAppDispatch();
    const users = useAppSelector(selectUsers);
    const pagination = useAppSelector(selectPagination);

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
        render?: (value: any) => React.ReactNode;
        action?: (data: ProductResDto) => void;
        deleteAction?: (id: string) => void;
        updateAction?: (id: string) => void;
        detailAction?: (id: string) => void;
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
            dispatch(deleteUser(id));
          },
          updateAction: (id: string) => {
            handleOpenDialogUpdate(id);
          },
          detailAction: (id: string) => {
            handleOpenDetail(id);
          },
        },
      ];
  return (
    <section className="mt-10">
      <UserTable<UserResDto>
        headers={headers}
        data={users}
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
  )
}

export default UsersTable;
