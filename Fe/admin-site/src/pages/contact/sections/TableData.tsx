import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
  selectContacts,
  selectPagination,
} from "@/redux/apps/contact/contactSelector";
import { ContactResDto } from "@/types/contact/contact";
import {
  deleteContact,
  fetchContacts,
  setPage,
  setPageSize,
} from "@/redux/apps/contact/contactSlice";
import DetailContactSheet from "./UpdateDetail/DetailContactSheet";
import { formatVietnamTime } from "@/utils/format";

const ContactTable = <T extends { id: string }>({
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

const ContactsTable: React.FC = () => {
  const dispatch = useAppDispatch();
  const contacts = useAppSelector(selectContacts);
  const pagination = useAppSelector(selectPagination);
  const [isOpenUpdate, setIsOpenUpdate] = useState(false);
  const [selectedContactId, setSelectedContactId] = useState<string | null>(
    null
  );

  const handleOpenDialogUpdate = (id: string) => {
    setIsOpenUpdate(true);
    setSelectedContactId(id);
  };

  const renderDate = (value: string) => {
    return formatVietnamTime(value);
  };

  const headers = [
    { label: "Mã Nhân viên", className: "text-center" },
    { label: "Họ tên" },
    { label: "Địa chỉ" },
    { label: "Năm sinh" },
    { label: "Email" },
    { label: "SĐT" },
    { label: "Ngày tạo" },
    { label: "Thao tác" },
  ];

  const columns: {
    key?: keyof ContactResDto;
    className?: string;
    isActionColumn?: boolean;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    render?: (value: any) => React.ReactNode;
    action?: (data: ContactResDto) => void;
    deleteAction?: (id: string) => void;
    updateAction?: (id: string) => void;
  }[] = [
    { key: "name", className: "text-center" },
    { key: "fullName" },
    { key: "address" },
    {
      key: "dateOfBirth",
      render: renderDate,
    },
    { key: "email" },
    { key: "phoneNumber" },
    {
      key: "createdOnDate",
      render: renderDate,
    },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
      deleteAction: (id: string) => {
        dispatch(deleteContact(id));
      },
      updateAction: (id: string) => {
        handleOpenDialogUpdate(id);
      },
    },
  ];

  useEffect(() => {
    dispatch(
      fetchContacts({
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
      <ContactTable<ContactResDto>
        headers={headers}
        data={contacts}
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
      {isOpenUpdate && selectedContactId && (
        <DetailContactSheet
          contactId={selectedContactId}
          isOpen={isOpenUpdate}
          onClose={() => setIsOpenUpdate(false)}
        />
      )}
    </section>
  );
};

export default ContactsTable;
