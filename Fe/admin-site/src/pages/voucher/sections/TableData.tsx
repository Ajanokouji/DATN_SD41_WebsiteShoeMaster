import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { VoucherResDto } from "@/types/voucher/voucher";
import { Link } from "react-router-dom";
import {
  deleteVoucher,
  setPage,
  setPageSize,
  fetchVouchers,
} from "@/redux/apps/voucher/voucherSlice";
import {
  selectPagination,
  selectVouchers,
} from "@/redux/apps/voucher/voucherSelector";
import DetailVoucherSheet from "./UpdateDetail/DetailVoucherSheet";
import { formatVietnamTime } from "@/utils/format";
import { FaEye } from "react-icons/fa6";

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

  const renderDiscount = (voucher: VoucherResDto) => {
    if (voucher.discountAmount) {
      return `${formatCurrency(voucher.discountAmount)} VNĐ`;
    }
    return `${voucher.discountPercentage}%`;
  };

  const renderMaxDiscountAmount = (voucher: VoucherResDto) => {
    if (voucher.maxDiscountAmount) {
      return `(Tối đa: ${formatCurrency(voucher.maxDiscountAmount)} VNĐ)`;
    }
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("en-us").format(value);

  const renderMinimumOrderAmount = (voucher: VoucherResDto) => {
    if (voucher.minimumOrderAmount) {
      return `${formatCurrency(voucher.minimumOrderAmount)} VNĐ`;
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
    { label: "Giảm giá" },
    { label: "Giá trị đơn hàng tối thiểu" },
    { label: "Tổng lượt sử dụng tối đa" },
    { label: "Đã dùng" },
    { label: "Thời gian lưu Mã Voucher" },
    { label: "Trạng thái hoạt động" },
    { label: "Khách hàng lưu trữ" },
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
              <div className="text-sm text-gray-500">{row?.code}</div>
            </div>
          </div>
        );
      },
    },
    {
      render: (_, row) => {
        if (row) {
          return (
            <div>
              <div>{renderDiscount(row)}</div>
              <div>{renderMaxDiscountAmount(row)}</div>
            </div>
          );
        }
      },
    },
    {
      render: (_, row) => renderMinimumOrderAmount(row!),
    },
    {
      key: "totalMaxUsage",
      render: (value) => {
        if (value != null) {
          return formatCurrency(value);
        } else {
          return "Không xác định";
        }
      },
    },
    {
      key: "redeemCount",
      render: (value) => {
        if (value != null) {
          return formatCurrency(value);
        } else {
          return "Không xác định";
        }
      },
    },
    {
      render: (_, row) => {
        const startDate = row?.startDate
          ? row.startDate.replace("T", " ").replace("Z", "")
          : "";
        const endDate = row?.endDate
          ? row.endDate.replace("T", " ").replace("Z", "")
          : "";

        const now = new Date().toISOString(); //Lấy giờ UTC hiện tại
        const start = row?.startDate.replace(" ", "T") + "Z" || "";
        const end = row?.endDate.replace(" ", "T") + "Z" || "";

        let viTri = 0
        if(now < start){
          viTri = 1;
        }else if(now >= start && now <= end){
          viTri = 2;
        }else if(now > end){
          viTri = 3;
        }

        return (
          <div className="flex space-y-2">
            {/* Đường thẳng xuyên qua các điểm */}
            <div className="relative flex flex-col">
              {/* Đường thẳng */}
              <div className="absolute ml-[7px] w-[2px] h-full bg-gray-500"></div>

              {/* Sắp diễn ra */}
              {viTri === 1 ? (
              <div className="z-10 flex items-center space-x-2">
                <div className="relative ml-[2px] flex items-center justify-center w-3 h-3 bg-gray-500 rounded-full">
                  <span className="text-[10px] text-white font-semibold"></span>
                </div>
                <div className="text-sm text-gray-600 mt-0.5">
                  Hiện tại
                </div>
              </div>
              ):""}

              {/* Điểm bắt đầu */}
              <div className="z-10 flex items-center space-x-2">
                <div className="relative flex items-center justify-center w-4 h-4 bg-blue-500 rounded-full">
                  <span className="text-[10px] text-white font-semibold">BĐ</span>
                </div>
                <div className="text-sm text-gray-600 mt-0.5">
                  {formatVietnamTime(startDate)}
                </div>
              </div>

              {/* Đang diễn ra */}
              {viTri === 2 ? (
              <div className="z-10 flex items-center space-x-2">
                <div className="relative ml-[2px] flex items-center justify-center w-3 h-3 bg-gray-500 rounded-full">
                  <span className="text-[10px] text-white font-semibold"></span>
                </div>
                <div className="text-sm text-gray-600 mt-0.5">
                  Hiện tại
                </div>
              </div>
              ):""}

              {/* Điểm kết thúc */}
              <div className="z-10 flex items-center space-x-2">
                <div className="relative flex items-center justify-center w-4 h-4 bg-red-500 rounded-full">
                  <span className="text-[10px] text-white font-semibold">KT</span>
                </div>
                <div className="text-sm text-gray-600 mt-0.5">
                  {formatVietnamTime(endDate)}
                </div>
              </div>

              {/* Đã kết thúc */}
              {viTri === 3 ? (
              <div className="z-10 flex items-center space-x-2">
                <div className="relative ml-[2px] flex items-center justify-center w-3 h-3 bg-gray-500 rounded-full">
                  <span className="text-[10px] text-white font-semibold"></span>
                </div>
                <div className="text-sm text-gray-600 mt-0.5">
                  Hiện tại
                </div>
              </div>
              ):""}
            </div>
          </div>
        );
      },
    },
    {
      key: "status",
      render: (value) => renderStatus(value),
    },
    {
      key: "voucherType",
      render: (value, row) => {
        return (
        <div>
          <div>Xem chi tiết</div>
          <Link to={`/voucher-user/${row?.id}`} target="_blank" rel="noopener noreferrer"
            className="flex items-center justify-center text-blue-500 hover:text-blue-700">
            <FaEye/>
          </Link>
        </div>
        );
      }
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
      "Tất cả": 0,
      "Đang diễn ra": 1,
      "Sắp diễn ra": 2,
      "Đã kết thúc": 3,
      "Tạm dừng": 4,
    };
  
    const trangThai = trangThaiMap[activeTab];
  
    if (trangThai !== undefined) {
      dispatch(
        fetchVouchers({
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
          statusTotal: trangThai,
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
        {["Tất cả", "Đang diễn ra", "Sắp diễn ra", "Đã kết thúc", "Tạm dừng"].map((tab) => (
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