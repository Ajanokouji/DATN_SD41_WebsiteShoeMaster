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
import { useLocation } from "react-router-dom";

const BillTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: TableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
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

const BillsTable: React.FC = () => {
  const dispatch = useAppDispatch();
  const location = useLocation();
  const bills = useAppSelector(selectBills);
  const pagination = useAppSelector(selectPagination);
  const [isOpenDetail, setIsOpenDetail] = useState(false);
  const [selectedBillId, setSelectedBillId] = useState<string | null>(null);

  const handleOpenDetail = (id: string) => {
    setIsOpenDetail(true);
    setSelectedBillId(id);
  };

  const renderCreatedDate = (value: string) => {
    return formatVietnamTime(value);
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(value);
  };

  const renderStatus = (status: string) => {
    const statusMap: { [key: string]: { label: string; className: string } } = {
      PendingConfirmation: { label: "Chờ xác nhận", className: "text-yellow-500" },
      Confirmed: { label: "Đã xác nhận", className: "text-blue-500" },
      Rejected: { label: "Bị từ chối", className: "text-red-500" },
      Paid: { label: "Đã thanh toán", className: "text-green-500" },
      Packed: { label: "Đã đóng gói", className: "text-purple-500" },
      Shipping: { label: "Đang vận chuyển", className: "text-orange-500" },
      Delivered: { label: "Đã giao hàng", className: "text-teal-500" },
      Completed: { label: "Hoàn thành", className: "text-green-700" },
      Cancelled: { label: "Đã hủy", className: "text-red-700" },
      DeliveryFailed: { label: "Giao hàng thất bại", className: "text-red-600" },
      ReturnProcessing: { label: "Đang xử lý hoàn trả", className: "text-yellow-600" },
      Returned: { label: "Đã hoàn trả", className: "text-gray-500" },
      OutOfStock: { label: "Không đủ hàng", className: "text-red-500" }
    };

    const statusInfo = statusMap[status] || { label: status, className: "text-gray-500" };
    return <span className={statusInfo.className}>{statusInfo.label}</span>;
  };

  const headers = [
    { label: "Mã hóa đơn", className: "text-center" },
    { label: "Tên khách hàng" },
    { label: "Số điện thoại" },
    { label: "Tổng tiền" },
    { label: "Giảm giá" },
    { label: "Phải trả" },
    { label: "Trạng thái" },
    { label: "Ngày tạo" },
    { label: " " },
  ];

  const columns: {
    key?: keyof BillResDto;
    className?: string;
    isActionColumn?: boolean;
    render?: (value: any) => React.ReactNode;
    action?: (data: BillResDto) => void;
    detailAction?: (id: string) => void;
  }[] = [
    { key: "billCode", className: "text-center" },
    { key: "recipientName" },
    { key: "recipientPhone" },
    { 
      key: "totalAmount",
      render: (value) => formatCurrency(value)
    },
    { 
      key: "discountAmount",
      render: (value) => formatCurrency(value)
    },
    { 
      key: "amountToPay",
      render: (value) => formatCurrency(value)
    },
    { 
      key: "status",
      render: renderStatus
    },
    {
      key: "createdOnDate",
      render: renderCreatedDate,
    },
    {
      isActionColumn: true,
      className: "text-center",
      action: (bill) => {
        console.log("Performing action for:", bill);
      },
      detailAction: (id: string) => {
        handleOpenDetail(id);
      },
    },
  ];

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const filters = {
      CurrentPage: pagination.currentPage,
      PageSize: pagination.pageSize,
      BillCode: params.get("BillCode") || "",
      RecipientName: params.get("RecipientName") || "",
      TotalAmount: params.get("TotalAmount") || "",
      RecipientAddress: params.get("RecipientAddress") || "",
      CreatedOnDate: params.get("CreatedOnDate") || "",
      Status: params.get("Status") || "",
    };

    dispatch(fetchBills(filters));
  }, [dispatch, location.search, pagination.currentPage, pagination.pageSize]);

  const handlePageChange = (newPage: number) => {
    dispatch(setPage(newPage));
  };

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
      {isOpenDetail && selectedBillId && (
        <DetailBillSheet
          billId={selectedBillId}
          isOpen={isOpenDetail}
          onClose={() => setIsOpenDetail(false)}
        />
      )}
    </section>
  );
};

export default BillsTable;
