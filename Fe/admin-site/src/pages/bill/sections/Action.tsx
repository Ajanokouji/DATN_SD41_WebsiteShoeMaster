import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
  SheetFooter,
} from "@/components/ui/sheet";
import { Label } from "@/components/ui/label";
import { Select, SelectTrigger, SelectValue, SelectContent, SelectItem } from "@/components/ui/select";
import { FaFilter } from "react-icons/fa6";
import { Badge } from "@/components/ui/badge";
import { X } from "lucide-react";
import { FilterItem } from "@/types/common/pagination";

interface FilterData {
  BillCode: string;
  RecipientName: string;
  TotalAmount: string;
  RecipientAddress: string;
  CreatedOnDate: string;
  Status: string;
}

const statusOptions = [
  { value: "PendingConfirmation", label: "Chờ xác nhận" },
  { value: "Confirmed", label: "Đã xác nhận" },
  { value: "Rejected", label: "Bị từ chối" },
  { value: "Paid", label: "Đã thanh toán" },
  { value: "Packed", label: "Đã đóng gói" },
  { value: "Shipping", label: "Đang vận chuyển" },
  { value: "Delivered", label: "Đã giao hàng" },
  { value: "Completed", label: "Hoàn thành" },
  { value: "Cancelled", label: "Đã hủy" },
  { value: "DeliveryFailed", label: "Giao hàng thất bại" },
  { value: "ReturnProcessing", label: "Đang xử lý hoàn trả" },
  { value: "Returned", label: "Đã hoàn trả" },
];

const ActionHeader = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [isOpenFilter, setIsOpenFilter] = useState(false);
  const [activeFilters, setActiveFilters] = useState<FilterItem[]>([]);

  const [filterData, setFilterData] = useState<FilterData>({
    BillCode: "",
    RecipientName: "",
    TotalAmount: "",
    RecipientAddress: "",
    CreatedOnDate: "",
    Status: "",
  });

  // Parse URL params on component mount
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const newFilterData = { ...filterData };
    const newActiveFilters: FilterItem[] = [];

    Object.keys(filterData).forEach(key => {
      const value = params.get(key);
      if (value) {
        newFilterData[key as keyof FilterData] = value;
        const label = getFilterLabel(key, value);
        newActiveFilters.push({ key, value, label });
      }
    });

    setFilterData(newFilterData);
    setActiveFilters(newActiveFilters);
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [location.search]);

  const getFilterLabel = (key: string, value: string): string => {
    switch(key) {
      case "BillCode":
        return `Mã hóa đơn: ${value}`;
      case "RecipientName":
        return `Tên khách hàng: ${value}`;
      case "TotalAmount":
        return `Số tiền: ${value}`;
      case "RecipientAddress":
        return `Địa chỉ: ${value}`;
      case "CreatedOnDate":
        return `Ngày tạo: ${value}`;
      case "Status": {
        const statusLabel = statusOptions.find(s => s.value === value)?.label || value;
        return `Trạng thái: ${statusLabel}`;
      }
      default:
        return `${key}: ${value}`;
    }
  };

  const applyFilters = () => {
    const params = new URLSearchParams();
    const newActiveFilters: FilterItem[] = [];

    Object.entries(filterData).forEach(([key, value]) => {
      if (value) {
        params.set(key, value);
        const label = getFilterLabel(key, value);
        newActiveFilters.push({ key, value, label });
      }
    });

    setActiveFilters(newActiveFilters);

    navigate({
      pathname: location.pathname,
      search: params.toString(),
    });

    setIsOpenFilter(false);
  };

  const clearAllFilters = () => {
    setFilterData({
      BillCode: "",
      RecipientName: "",
      TotalAmount: "",
      RecipientAddress: "",
      CreatedOnDate: "",
      Status: "",
    });
    setActiveFilters([]);
    navigate(location.pathname);
  };

  const removeFilter = (keyToRemove: string) => {
    setFilterData(prev => ({
      ...prev,
      [keyToRemove]: ""
    }));
    
    setActiveFilters(prev => prev.filter(filter => filter.key !== keyToRemove));
    
    const params = new URLSearchParams(location.search);
    params.delete(keyToRemove);
    
    navigate({
      pathname: location.pathname,
      search: params.toString()
    });
  };

  const handleFilterChange = (name: keyof FilterData, value: string) => {
    setFilterData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  return (
    <section className="mb-4">
      <div className="flex flex-col gap-4">
        {/* Filter Button and Active Filters Row */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <Sheet open={isOpenFilter} onOpenChange={setIsOpenFilter}>
              <SheetTrigger asChild>
                <Button variant="outline" className="flex items-center gap-2">
                  <FaFilter className="h-4 w-4" />
                  <span>Bộ lọc</span>
                </Button>
              </SheetTrigger>
              <SheetContent>
                <SheetHeader>
                  <SheetTitle>Lọc hóa đơn</SheetTitle>
                  <SheetDescription>
                    Thiết lập các điều kiện lọc cho danh sách hóa đơn
                  </SheetDescription>
                </SheetHeader>

                <div className="grid gap-4 py-4">
                  <div className="grid gap-2">
                    <Label htmlFor="BillCode">Mã hóa đơn</Label>
                    <Input
                      id="BillCode"
                      value={filterData.BillCode}
                      onChange={(e) => handleFilterChange("BillCode", e.target.value)}
                      placeholder="Nhập mã hóa đơn"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="RecipientName">Tên khách hàng</Label>
                    <Input
                      id="RecipientName"
                      value={filterData.RecipientName}
                      onChange={(e) => handleFilterChange("RecipientName", e.target.value)}
                      placeholder="Nhập tên khách hàng"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="TotalAmount">Số tiền</Label>
                    <Input
                      id="TotalAmount"
                      type="number"
                      value={filterData.TotalAmount}
                      onChange={(e) => handleFilterChange("TotalAmount", e.target.value)}
                      placeholder="Nhập số tiền"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="RecipientAddress">Địa chỉ</Label>
                    <Input
                      id="RecipientAddress"
                      value={filterData.RecipientAddress}
                      onChange={(e) => handleFilterChange("RecipientAddress", e.target.value)}
                      placeholder="Nhập địa chỉ"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="CreatedOnDate">Ngày khởi tạo</Label>
                    <Input
                      id="CreatedOnDate"
                      type="date"
                      value={filterData.CreatedOnDate}
                      onChange={(e) => handleFilterChange("CreatedOnDate", e.target.value)}
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="Status">Trạng thái</Label>
                    <Select
                      value={filterData.Status}
                      onValueChange={(value) => handleFilterChange("Status", value)}
                    >
                      <SelectTrigger id="Status">
                        <SelectValue placeholder="Chọn trạng thái" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="">Tất cả</SelectItem>
                        {statusOptions.map((option) => (
                          <SelectItem key={option.value} value={option.value}>
                            {option.label}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>

                <SheetFooter>
                  <Button variant="outline" onClick={clearAllFilters}>
                    Xóa bộ lọc
                  </Button>
                  <Button onClick={applyFilters}>Áp dụng</Button>
                </SheetFooter>
              </SheetContent>
            </Sheet>

            {/* Quick search input can be added here if needed */}
          </div>
        </div>

        {/* Active Filters Display */}
        {activeFilters.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {activeFilters.map((filter) => (
              <Badge key={filter.key} variant="secondary" className="flex items-center gap-1">
                {filter.label}
                <button
                  onClick={() => removeFilter(filter.key)}
                  className="ml-1 rounded-full hover:bg-gray-200 p-1"
                >
                  <X size={12} />
                </button>
              </Badge>
            ))}
            {activeFilters.length > 1 && (
              <Button
                variant="ghost"
                size="sm"
                onClick={clearAllFilters}
                className="h-6 text-xs"
              >
                Xóa tất cả
              </Button>
            )}
          </div>
        )}
      </div>
    </section>
  );
};

export default ActionHeader;