import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { VoucherResDto } from "@/types/voucher/voucher";
import {
  deleteVoucher,
  fetchVouchersByStatusDate,
  setPage,
  setPageSize,
  fetchVouchers,
} from "@/redux/apps/voucher/voucherSlice";
import {
  selectPagination,
  selectVouchers,
} from "@/redux/apps/voucher/voucherSelector";
import DetailVoucherSheet from "./UpdateDetail/DetailVoucherSheet";
import { date } from "zod";

const VoucherTable = <T extends { id: string }>({
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

const VouchersTable: React.FC = () => {
  const dispatch = useAppDispatch();
  const vouchers = useAppSelector(selectVouchers);
  const pagination = useAppSelector(selectPagination);
  const [isOpenUpdate, setIsOpenUpdate] = useState(false);
  const [selectedVoucherId, setSelectedVoucherId] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<string>("Tất cả"); // Tab hiện tại

  const handleOpenDialogUpdate = (id: string) => {
    setIsOpenUpdate(true);
    setSelectedVoucherId(id);
  };

  const formatDateTime = (rawDate: string) => {
    var date = new Date();
    if(rawDate.includes("T") && rawDate.includes("Z")) {
      date = new Date(rawDate);
    }
    else
    {
      //Chuyển đổi về định dạng UTC (Db đã lưu giờ UTC nhưng định dạng ko chuẩn nên phải chuyển đổi lại)
      date = new Date(rawDate.replace(" ", "T") + "Z");
    }

    const dateLocal = new Date(date); //Tự chuyển đổi về giờ local
    const options: Intl.DateTimeFormatOptions = {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    };
    return new Intl.DateTimeFormat("vi-vn", options).format(dateLocal);
  };
  
  const renderDateRange = (startDate: string, endDate: string) => {
    return `${formatDateTime(startDate)} - ${formatDateTime(endDate)}`;
  };  

  const renderDiscount = (voucher: VoucherResDto) => {
    const formatCurrency = (value: number) =>
      new Intl.NumberFormat("en-us").format(value);
    if (voucher.discountAmount) {
      return `${formatCurrency(voucher.discountAmount)} $`;
    }
    return `${voucher.discountPercentage}%`;
  };

  const renderMinimumOrderAmount = (voucher: VoucherResDto) => {
    const formatCurrency = (value: number) =>
      new Intl.NumberFormat("en-us").format(value);
    if (voucher.minimumOrderAmount) {
      return `${formatCurrency(voucher.minimumOrderAmount)} $`;
    }
  };

  const renderStatus = (status: number | null) => {
    switch (status) {
      case 1:
        return "Hoạt động";
      case 0:
        return "Dừng hoạt động";
      default:
        return "Không xác định";
    }
  };

  const headers = [
    { label: "Tên Voucher | Mã voucher" },
    { label: "Loại mã" },
    { label: "Giảm giá" },
    { label: "Giá trị đơn hàng tối thiểu" },
    { label: "Thời gian lưu Mã Voucher" },
    { label: "Trạng thái" },
    { label: "Thao tác" },
  ];

  const columns: {
    key?: keyof VoucherResDto;
    isActionColumn?: boolean;
    render?: (value: any, row?: VoucherResDto) => React.ReactNode;
    action?: (data: VoucherResDto) => void;
    deleteAction?: (id: string) => void;
    updateAction?: (id: string) => void;
  }[] = [
    {
      key: "voucherName",
      render: (value, row) => {
        const now = new Date().toISOString(); //Lấy giờ UTC hiện tại
        const start = row?.startDate.replace(" ", "T") + "Z" || "";
        const end = row?.endDate.replace(" ", "T") + "Z" || "";
    
        let label = "";
        let color = "";
    
        if (row?.status === 0) {
          if (now < start || (now >= start && now <= end)) {
            label = "Tạm dừng";
            color = "bg-yellow-500 text-white";
          } else {
            label = "Đã kết thúc";
            color = "bg-red-500 text-white";
          }
        } else if (row?.status === 1) {
          if (now < start) {
            label = "Sắp diễn ra";
            color = "bg-blue-500 text-white";
          } else if (now >= start && now <= end) {
            label = "Đang diễn ra";
            color = "bg-green-500 text-white";
          } else {
            label = "Đã kết thúc";
            color = "bg-red-500 text-white";
          }
        } else {
          label = "Không xác định";
          color = "bg-gray-400 text-white";
        }
    
        return (
          <div className="space-y-1">
            <div className="flex space-x-2">
              <span className={`text-xs px-4 py-1 text-white ${color} rounded`}
                style={{clipPath: "polygon(0% 0%, 100% 0, 90% 50%, 100% 100%, 0% 100%)",}} //Tạo cờ đuôi cá
              >
                {label}
              </span>
            </div>
            <div>
              <span>{value}</span>
              <div className="text-sm text-gray-800">{row?.code}</div>
            </div>
          </div>
        );
      },
    },    
    {
      key: "voucherType",
      render: (value) =>
        value === 1
          ? "Voucher toàn Shop"
          : value === 2
          ? "Voucher sản phẩm"
          : "Không xác định",
    },
    {
      render: (_, row) => renderDiscount(row!),
    },
    {
      render: (_, row) => renderMinimumOrderAmount(row!),
    },
    {
      render: (_, row) =>
        renderDateRange(row?.startDate || "", row?.endDate || ""),
    },
    {
      key: "status",
      render: (value) => renderStatus(value),
    },
    {
      isActionColumn: true,
      action: (voucher) => {
        console.log("Chi tiết voucher:", voucher);
      },
      deleteAction: (id: string) => {
        dispatch(deleteVoucher(id));
      },
      updateAction: (id: string) => {
        handleOpenDialogUpdate(id);
      },
    },
  ];

  useEffect(() => {
    const trangThaiMap: { [key: string]: number | undefined } = {
      "Tất cả": undefined,
      "Đang diễn ra": 1,
      "Sắp diễn ra": 2,
      "Đã kết thúc": 3,
    };
  
    const trangThai = trangThaiMap[activeTab];
  
    if (trangThai) {
      dispatch(
        fetchVouchersByStatusDate({
          params: {
            CurrentPage: pagination.currentPage,
            PageSize: pagination.pageSize,
          },
          trangThai,
        })
      );
    } else {
      dispatch(
        fetchVouchers({
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
        })
      );
    }
  }, [dispatch, pagination.currentPage, pagination.pageSize, activeTab]);  

  const handlePageChange = (newPage: number) => {
    dispatch(setPage(newPage));
  };

  const handlePageSizeChange = (newSize: number) => {
    dispatch(setPageSize(newSize));
  };

  return (
    <section className="mt-10">
      {/* Tabs */}
      <div className="flex space-x-4 mb-6 border-b border-gray-300">
        {["Tất cả", "Đang diễn ra", "Sắp diễn ra", "Đã kết thúc"].map((tab) => (
          <button
            key={tab}
            className={`relative px-4 py-2 text-sm font-medium transition-colors duration-300 ${
              activeTab === tab
                ? "text-gray-900"
                : "text-gray-500 hover:text-gray-900"
            }`}
            onClick={() => {
              setActiveTab(tab);
              dispatch(setPage(1)); // Đặt lại trang về 1 khi chuyển tab
            }}
          >
            {tab}
            {/* Hiệu ứng gạch chân */}
            {activeTab === tab && (
              <span className="absolute left-0 bottom-0 w-full h-[2px] bg-gray-500 rounded-full"></span>
            )}
          </button>
        ))}
      </div>

      {/* Container giới hạn chiều rộng và cuộn ngang */}
      <div className="max-w-full overflow-x-auto">
        <div className="max-w-full mx-auto">
          <VoucherTable<VoucherResDto>
            headers={headers}
            data={vouchers}
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
      {isOpenUpdate && selectedVoucherId && (
        <DetailVoucherSheet
          voucherId={selectedVoucherId}
          isOpen={isOpenUpdate}
          onClose={() => setIsOpenUpdate(false)}
        />
      )}
    </section>
  );
};

export default VouchersTable;