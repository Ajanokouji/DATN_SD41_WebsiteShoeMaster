import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { VoucherUserResDto } from "@/types/voucherUser/voucherUser";
import { UserResDtoForSearch } from "@/types/voucherUser/voucherUser";

import {
  deleteVoucherUser,
  setVoucherUserPage,
  setVoucherUserPageSize,
  fetchVoucherUsers,
  fetchUsersByIds,
} from "@/redux/apps/voucherUser/voucherUserSlice";

import {
  selectVoucherUserPagination,
  selectVoucherUsers,
} from "@/redux/apps/voucherUser/voucherUserSelector";
import { formatVietnamTime } from "@/utils/format";
import { FaAddressCard, FaEnvelope, FaPhone } from "react-icons/fa6";
import { FaHome } from "react-icons/fa";

const VoucherUserTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: TableProps<T>) => (
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
);

const VoucherUsersTable: React.FC<{ voucherId: string }> = ({ voucherId }) => {
  const dispatch = useAppDispatch();

  const voucherUsers = useAppSelector(selectVoucherUsers);
  const pagination = useAppSelector(selectVoucherUserPagination);
  const [users, setUsers] = useState<UserResDtoForSearch[]>([]);

  useEffect(() => {
    if (voucherId) {
      dispatch(
        fetchVoucherUsers({
          voucherId,
          isUsed: false,
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
        })
      );
    }
  }, [voucherId, dispatch, pagination.currentPage, pagination.pageSize]);

  useEffect(() => {
    const userIds: string[] = [...new Set(voucherUsers.map((v) => v.userId as string))];
    if (userIds.length > 0) {
      dispatch(fetchUsersByIds(userIds))
        .unwrap()
        .then((fetchedUsers) => {
          setUsers(fetchedUsers);
        })
        .catch((error) => {
          console.error("Error fetching users by IDs:", error);
        });
    }
  }, [dispatch, voucherUsers]);

  const renderDate = (date: string) => {
    if(date.includes("T")) {
      date = date.replace("T", " ").replace("Z", "");
    }
    return `${formatVietnamTime(date)}`;
  };

  const headers = [
    { label: "Thông tin khách hàng" },
    { label: "Ngày lưu trữ" },
    { label: "Thao tác"},
  ];

  const columns: {
    key?: keyof VoucherUserResDto;
    isActionColumn?: boolean;
    render?: (value: any, row?: VoucherUserResDto) => React.ReactNode;
    action?: (data: VoucherUserResDto) => void;
    deleteAction?: (id: string) => void;
  }[] = [
    {
      key: "userId",
      render: (value) => {
        const user = users.find((user) => user.id === value);

        if (user) {
          // Xác định trạng thái và lớp CSS
          const isActive = user?.isActive; // Lấy giá trị isActive từ row
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
            <div className="flex items-center relative">
              {/* Ảnh với borderColor */}
              <div className="relative">
                <img
                  className={`object-cover w-20 h-20 border-2 rounded-full mr-5 ${borderColor}`}
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

              {/* Thông tin người dùng */}
              <div className="flex-1">
                <h3 className="flex text-start font-semibold">
                  <FaAddressCard className="mr-2 mt-1" />
                  {user.name} {user.username ? "- " + user.username : ""}
                </h3>
                <p className="flex text-start text-sm text-gray-500">
                  <FaPhone className="mr-2 mt-1" />
                  {user.phoneNumber}
                </p>
                <p className="flex text-start text-sm text-gray-500">
                  <FaEnvelope className="mr-2 mt-1" />
                  {user.email}
                </p>
                <p className="flex text-start text-sm text-gray-500">
                  <FaHome className="mr-2 mt-1" />
                  {user.address}
                </p>
              </div>
            </div>
          );
        } else {
          return (
            <div className="flex items-center gap-2">
              <span>Không xác định</span>
            </div>
          );
        }
      },
    },
    {
      key: "lastModifiedOnDate",
      render: (value) => {
        return <div>{renderDate(value)}</div>
      }
    },
    {
      isActionColumn: true,
      deleteAction: (id: string) => {
        dispatch(deleteVoucherUser(id));
      }
    },
  ];

  const handlePageChange = (newPage: number) => {
    dispatch(setVoucherUserPage(newPage));
  };

  const handlePageSizeChange = (newSize: number) => {
    dispatch(setVoucherUserPageSize(newSize));
  };

  return (
    <section className="mt-10">
      <div className="max-w-full overflow-x-auto">
        <div className="max-w-full mx-auto">
          <VoucherUserTable<VoucherUserResDto>
            headers={headers}
            data={voucherUsers}
            columns={columns}
          />
        </div>
      </div>

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

export default VoucherUsersTable;