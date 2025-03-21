import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectBills, selectPagination } from "@/redux/apps/bill/billSelector";
import { BillResDto } from "@/types/bill/bill";
import {
  fetchBills,
  setPage,
  setPageSize,
} from "@/redux/apps/bill/billSlice";
import { formatVietnamTime } from "@/utils/format";
import DetailBillSheet from "./DetailBillSheet";

const BillTable = <T extends { id: string }>({
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

const BillsTable: React.FC = () => {
  const dispatch = useAppDispatch();
  const bills = useAppSelector(selectBills);
  const pagination = useAppSelector(selectPagination);
  const [isOpenUpdate, setIsOpenUpdate] = useState(false);
  const [selectedBillId, setSelectedBillId] = useState<string | null>(null);

  const handleOpenDialogUpdate = (id: string) => {
    setIsOpenUpdate(true);
    setSelectedBillId(id);
  };

  const renderCreatedDate = (value: string) => {
      return formatVietnamTime(value);
    };

  const headers = [
    { label: "Code", className: "text-center" },
    { label: "Recipient Name" },
    { label: "Recipient Phone" },
    { label: "Total Amount" },
    { label: "Discount Amount" },
    { label: "AmountToPay" },
    { label: "Status" },
    { label: "Create At" },
    { label: " " },
  ];

  const columns: {
    key?: keyof BillResDto;
    className?: string;
    isActionColumn?: boolean;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    render?: (value: any) => React.ReactNode;
    action?: (data: BillResDto) => void;
    detailAction?: (id: string) => void;
  }[] = [
    { key: "billCode", className: "text-center" },
    { key: "recipientName" },
    { key: "recipientPhone" },
    { key: "totalAmount" },
    { key: "discountAmount" },
    { key: "amountToPay" },
    { key: "status" },
    
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
      
      detailAction: (id: string) => {
        handleOpenDialogUpdate(id);
      },
    },
  ];

  useEffect(() => {
    dispatch(
      fetchBills({
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
      <BillTable<BillResDto> headers={headers} data={bills} columns={columns} />
      <Pagination
        currentPage={pagination.currentPage}
        totalPages={pagination.totalPages}
        pageSize={pagination.pageSize}
        totalRecords={pagination.totalRecords}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />
      {isOpenUpdate && selectedBillId && (
        <DetailBillSheet
          billId={selectedBillId}
          isOpen={isOpenUpdate}
          onClose={() => setIsOpenUpdate(false)}
        />
      )}
    </section>
  );
};

export default BillsTable;
