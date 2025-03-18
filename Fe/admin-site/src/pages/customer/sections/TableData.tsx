import React, { useEffect } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";

import { formatVietnamTime } from "@/utils/format";
import {
  selectCustomers,
  selectPagination,
} from "@/redux/apps/customer/customerSelector";
import { CustomerResDto } from "@/types/customer/customer";
import {
  deleteCustomer,
  fetchCustomers,
  setPage,
  setPageSize,
} from "@/redux/apps/customer/customerSlice";

const CustomerTable = <T extends { id: string }>({
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

const CustomersTable: React.FC = () => {
  const dispatch = useAppDispatch();
  const customers = useAppSelector(selectCustomers);
  const pagination = useAppSelector(selectPagination);

  const renderDate = (value: string) => {
    return formatVietnamTime(value);
  };

  const headers = [
    { label: "Code", className: "text-center" },
    { label: "Customer name" },
    { label: "Email" },
    { label: "Phone" },
    { label: "Address" },
    { label: "Description" },
    { label: "Create At" },
    { label: " " },
  ];

  const columns: {
    key?: keyof CustomerResDto;
    className?: string;
    isActionColumn?: boolean;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    render?: (value: any) => React.ReactNode;
    action?: (data: CustomerResDto) => void;
    deleteAction?: (id: string) => void;
  }[] = [
    { key: "code", className: "text-center" },
    { key: "name" },
    { key: "email" },
    { key: "phoneNumber" },
    { key: "address" },
    { key: "description" },
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
        dispatch(deleteCustomer(id));
      },
    },
  ];

  useEffect(() => {
    dispatch(
      fetchCustomers({
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
      <CustomerTable<CustomerResDto>
        headers={headers}
        data={customers}
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

export default CustomersTable;
