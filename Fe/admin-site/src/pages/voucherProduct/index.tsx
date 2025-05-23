import React, { useEffect, useState } from "react";
import VoucherProductsTable from "./sections/TableData";
import { useParams } from "react-router-dom";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { fetchVoucherById } from "@/redux/apps/voucher/voucherSlice";
import { selectVoucher } from "@/redux/apps/voucher/voucherSelector";
import { Helmet } from "react-helmet-async";
import { formatVietnamTime } from "@/utils/format";
import { Button } from "@/components/ui/button";
import AddVoucherProductSheet from "./sections/FormAdd/AddVoucherProductSheet";
import { useForm } from "react-hook-form";

const VoucherProducts: React.FC = () => {
  const methods = useForm();
  //open form add
  const [isOpenAdd, setIsOpenAdd] = useState(false);

  const {voucherId} = useParams();
  const dispatch = useAppDispatch();
  const voucher = useAppSelector(selectVoucher);

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("en-us").format(value);

  const renderStatusDate = () => {
    if(voucher)
    {
      const now = new Date().toISOString(); //Lấy giờ UTC hiện tại
      const start = voucher.startDate.replace(" ", "T") + "Z" || "";
      const end = voucher.endDate.replace(" ", "T") + "Z" || "";

      let label = "";
      let color = "";

      if (voucher.status === 0) {
        if (now < start || (now >= start && now <= end)) {
          label = "Tạm dừng";
          color = "text-sm text-yellow-500";
        } else {
          label = "Đã kết thúc";
          color = "text-sm text-red-500";
        }
      } else if (voucher.status === 1) {
        if (now < start) {
          label = "Sắp diễn ra";
          color = "text-sm text-blue-500";
        } else if (now >= start && now <= end) {
          label = "Đang diễn ra";
          color = "text-sm text-green-500";
        } else {
          label = "Đã kết thúc";
          color = "text-sm text-red-500";
        }
      } else {
        label = "Không xác định";
        color = "text-sm text-gray-500";
      }

      return (
        <span className={color}>
          - {label}
        </span>
      );
    }
  }

  const [isLoading, setIsLoading] = useState(false);

  // Fetch voucher khi có voucherId
  useEffect(() => {
    if (voucherId) {
      setIsLoading(true); // Bắt đầu loading
      dispatch(fetchVoucherById(voucherId))
        .unwrap()
        .catch((error) => {
          console.error("Error fetching voucher:", error);
        })
        .finally(() => {
          setIsLoading(false); // Kết thúc loading
        });
    }
  }, [voucherId, dispatch]);

  // Các điều kiện kiểm tra dữ liệu
  if (isLoading) {
    return (
      <div className="text-center text-blue-500">
        Đang tải...
      </div>
    );
  }
  
  if (!voucherId) {
    return (
      <div className="text-center text-red-500">
        Không tìm thấy Voucher! Vui lòng kiểm tra lại!
      </div>
    );
  }

  if ((!voucher) || (voucher && voucher.isdeleted)) {
    return (
      <div className="text-center text-red-500">
        Không tìm thấy Voucher! Vui lòng kiểm tra lại!
      </div>
    );
  }

  if (voucher.id != voucherId) {
    return (
      <div className="text-center text-red-500">
        Không tìm thấy Voucher! Vui lòng kiểm tra lại!
      </div>
    );
  }

  if (voucher.voucherType === 1) {
    return (
      <div className="text-center text-red-500">
        Đây là "Voucher toàn shop" áp dụng cho toàn bộ sản phẩm trong cửa hàng!
        Vui lòng kiểm tra lại!
      </div>
    );
  }

  const handleOpenDialogAdd = () => {
    setIsOpenAdd(true);
  };

  return (
    <>
      <section className="px-8">
        <Helmet>
          <title>Danh sách sản phẩm áp dụng Voucher {voucher.voucherName}</title>
        </Helmet>
        <h2 className="text-2xl mb-2">
          Danh Sách Sản Phẩm Áp Dụng Voucher: {voucher.voucherName} {renderStatusDate()}
        </h2>
        <div className="flex text-sm text-gray-500">
          <div className="w-1/2">
            - Mã Voucher: {voucher.code} <br/> 
            - Loại Voucher: {voucher.voucherType === 1 ? "Voucher toàn shop" : "Voucher sản phẩm"} <br/>
            - Giảm giá: {voucher.discountAmount? formatCurrency(voucher.discountAmount) + " VNĐ" : formatCurrency(voucher.discountPercentage?voucher.discountPercentage:0) + "%" + " (Tối đa: " + formatCurrency(voucher.maxDiscountAmount?voucher.maxDiscountAmount:0) + " VNĐ)" } <br/>
            - Giá trị đơn hàng tối thiểu: {voucher.minimumOrderAmount? formatCurrency(voucher.minimumOrderAmount) + " VNĐ" : "Không yêu cầu"}
          </div>
          <div className="w-1/2">
            - Thời gian lưu mã: {formatVietnamTime(voucher.startDate)} - {formatVietnamTime(voucher.endDate)} <br/>
            - Tổng lượt sử dụng tối đa: {voucher.totalMaxUsage} <br/>
            - Đã dùng: {voucher.redeemCount} <br/>
            - Trạng thái: {voucher.status === 0 ? "Dừng hoạt động" : voucher.status === 1 ? "Hoạt động" : "Không xác định"}
          </div>
        </div>
        <Button onClick={() => handleOpenDialogAdd()} className="mt-4 px-4 py-2">
            Thêm sản phẩm
        </Button>
        <VoucherProductsTable voucherId={voucherId} />

        {isOpenAdd && (
          <AddVoucherProductSheet
            isOpen={isOpenAdd}
            onClose={() => setIsOpenAdd(false)}
            voucherId={voucherId}
            setValue={methods.setValue}
          />
        )}
      </section>
    </>
  );
};

export default VoucherProducts;