import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
  deleteUser,
  setUserPage,
  setUserPageSize,
  fetchUsers,
} from "@/redux/apps/user/userSlice";
import {
  selectPagination,
  selectUsers,
} from "@/redux/apps/user/userSelector";
import type { UserResDto } from "@/types/user/user";
import { formatVietnamTime } from "@/utils/format";
import { Eye, EyeOff } from "lucide-react";
import { Button } from "@/components/ui/button";
import AddUserSheet from "./FormAdd/AddUserSheet";
import EditUserSheet from "./FormEdit/EditUserSheet";

const UserTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: {
  headers: { label: string; className?: string }[];
  data: T[];
  columns: any[];
}) => (
  <div>
    <div className="border border-gray-300 rounded-t-xl overflow-hidden">
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
  </div>
);

const UsersTable: React.FC = () => {
  const dispatch = useAppDispatch();
  const users = useAppSelector(selectUsers);
  const pagination = useAppSelector(selectPagination);
  
  const [userType, setUserType] = useState<number>(0);
  const [openAdd, setOpenAdd] = useState(false);
  const [showPasswordRow, setShowPasswordRow] = useState<{ [id: string]: boolean }>({});
  const [editUserId, setEditUserId] = useState<string | null>(null);
  const [openEdit, setOpenEdit] = useState(false);

  const headers = [
    { label: "Tên đăng nhập", className: "text-center" },
    { label: "Tên người dùng" },
    { label: "Mật khẩu" },
    { label: "Trạng thái" },
    { label: "Ngày tạo" },
    { label: "Thao tác", className: "text-center" },
  ];

  const columns: {
    key?: keyof UserResDto;
    className?: string;
    isActionColumn?: boolean;
    render?: (value: any, row?: UserResDto) => React.ReactNode;
    action?: (data: UserResDto) => void;
    deleteAction?: (id: string) => void;
    updateAction?: (id: string) => void;
    detailAction?: (id: string) => void;
  }[] = [
    { key: "username", className: "text-center" },
    { key: "name" },
    {
      key: "password",
      render: (value: string, row?: UserResDto) => {
        const isShow = row && showPasswordRow[row.id];
        return (
          <div className="flex items-center justify-center gap-2 h-10">
            <span className="inline-block align-middle select-all">
              {isShow ? value : "•".repeat(value?.length || 8)}
            </span>
            <button
              type="button"
              className="ml-1 text-gray-500 hover:text-blue-600 flex items-center"
              tabIndex={-1}
              onClick={() =>
                row &&
                setShowPasswordRow((prev) => ({
                  ...prev,
                  [row.id]: !prev[row.id],
                }))
              }
            >
              {isShow ? <EyeOff size={18} /> : <Eye size={18} />}
            </button>
          </div>
        );
      },
    },
    {
      key: "isActive",
      render: (value: boolean) =>
        value === true ? (
          <span className="text-green-600 font-semibold">Hoạt động</span>
        ) : (
          <span className="text-red-500 font-semibold">Dừng hoạt động</span>
        ),
    },
    {
      key: "createdOnDate",
      render: (value: string) =>
        value
          ? formatVietnamTime(value)
          : "",
    },
    {
      isActionColumn: true,
      className: "text-center",
      updateAction: (id: string) => {
        setEditUserId(id);
        setOpenEdit(true);
      },
      deleteAction: (id: string) => {
        dispatch(deleteUser(id));
      },
    },
  ];

  useEffect(() => {
  dispatch(
    fetchUsers({
      CurrentPage: pagination.currentPage,
      PageSize: pagination.pageSize,
      type: userType,
    })
  );
}, [dispatch, pagination.currentPage, pagination.pageSize, userType]);
    
  const handlePageChange = (newPage: number) => {
    dispatch(setUserPage(newPage));
  };

  const handlePageSizeChange = (newSize: number) => {
    dispatch(setUserPageSize(newSize));
  };

  return (
    <section className="mt-10">
      <div className="flex justify-end mb-4">
        <Button onClick={() => setOpenAdd(true)}>+ Thêm tài khoản</Button>
      </div>
      <AddUserSheet
        isOpen={openAdd}
        onClose={() => setOpenAdd(false)}
        setUserType={setUserType} // truyền hàm này
      />
      {editUserId && (
        <EditUserSheet
          isOpen={openEdit}
          onClose={() => setOpenEdit(false)}
          userId={editUserId}
        />
      )}
      <div className="flex items-center gap-4 mb-4">
        <label className="font-semibold">Loại tài khoản:</label>
        <Select value={userType.toString()} onValueChange={val => setUserType(Number(val))}>
          <SelectTrigger className="w-[160px]">
            <SelectValue placeholder="Chọn loại tài khoản" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="0">Admin</SelectItem>
            <SelectItem value="1">Nhân viên</SelectItem>
          </SelectContent>
        </Select>
      </div>

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
  );
};

export default UsersTable;
