// ActionHeader.tsx
import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
  SheetFooter,
} from "@/components/ui/sheet";
import { Label } from "@/components/ui/label";
import { FaFilter } from "react-icons/fa6";
import { Badge } from "@/components/ui/badge";
import { X } from "lucide-react";

interface FilterData {
  name: string;
  email: string;
  phoneNumber: string;
}

const ActionHeader = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const [isOpenFilter, setIsOpenFilter] = useState(false);
  const [activeFilters, setActiveFilters] = useState<
    { key: string; value: string; label: string }[]
  >([]);

  const [filterData, setFilterData] = useState<FilterData>({
    name: "",
    email: "",
    phoneNumber: "",
  });

  // Khi URL thay đổi, lấy params và chuyển thành filterData
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const newFilterData: FilterData = {
      name: params.get("name") || "",
      email: params.get("email") || "",
      phoneNumber: params.get("phoneNumber") || "",
    };

    setFilterData(newFilterData);

    // Cập nhật activeFilters hiển thị
    const newActiveFilters = Object.entries(newFilterData)
      .filter(([_, value]) => value)
      .map(([key, value]) => ({
        key,
        value,
        label: `${key}: ${value}`,
      }));

    setActiveFilters(newActiveFilters);
  }, [location.search]);

  const applyFilters = () => {
    const params = new URLSearchParams();
  
    Object.entries(filterData).forEach(([key, value]) => {
      if (value) {
        // Đổi "name" thành "Ten" khi tạo URL params
        if (key === "name") {
          params.set("Ten", value);
        } else {
          params.set(key, value);
        }
      }
    });
  
    console.log("Generated URL:", `${location.pathname}?${params.toString()}`); // Log URL
  
    navigate({
      pathname: location.pathname,
      search: params.toString(),
    });
  
    setIsOpenFilter(false);
  };
  // Xóa tất cả filter
  const clearAllFilters = () => {
    setFilterData({
      name: "",
      email: "",
      phoneNumber: "",
    });
    setActiveFilters([]);
    navigate(location.pathname);
  };

  // Xóa filter riêng lẻ
  const removeFilter = (keyToRemove: string) => {
    setFilterData((prev) => ({
      ...prev,
      [keyToRemove]: "",
    }));

    setActiveFilters((prev) => prev.filter((f) => f.key !== keyToRemove));

    const params = new URLSearchParams(location.search);
    params.delete(keyToRemove);

    navigate({
      pathname: location.pathname,
      search: params.toString(),
    });
  };

  const handleFilterChange = (name: keyof FilterData, value: string) => {
    setFilterData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  return (
    <section className="mb-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 justify-between">
        <div className="flex flex-col w-full space-y-2">
          <div className="flex gap-2 items-center">
            <Input
              type="text"
              placeholder="Tìm kiếm nhanh theo tên khách hàng"
              className="shadow-none"
              value={filterData.name}
              onChange={(e) => handleFilterChange("name", e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") applyFilters();
              }}
            />

            <Sheet open={isOpenFilter} onOpenChange={setIsOpenFilter}>
              <SheetTrigger asChild>
                <Button variant="outline" size="icon">
                  <FaFilter />
                </Button>
              </SheetTrigger>

              <SheetContent>
                <SheetHeader>
                  <SheetTitle>Lọc khách hàng</SheetTitle>
                </SheetHeader>

                <div className="grid gap-4 py-4">
                  <div className="grid gap-2">
                    <Label htmlFor="name">Tên khách hàng</Label>
                    <Input
                      id="name"
                      value={filterData.name}
                      onChange={(e) => handleFilterChange("name", e.target.value)}
                      placeholder="Nhập tên khách hàng"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="email">Email</Label>
                    <Input
                      id="email"
                      value={filterData.email}
                      onChange={(e) => handleFilterChange("email", e.target.value)}
                      placeholder="Nhập email khách hàng"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="phoneNumber">Số điện thoại</Label>
                    <Input
                      id="phoneNumber"
                      value={filterData.phoneNumber}
                      onChange={(e) => handleFilterChange("phoneNumber", e.target.value)}
                      placeholder="Nhập số điện thoại"
                    />
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
          </div>

          {activeFilters.length > 0 && (
            <div className="flex flex-wrap gap-2 mt-2">
              {activeFilters.map((filter) => (
                <Badge
                  key={filter.key}
                  variant="secondary"
                  className="flex items-center gap-1"
                >
                  {filter.label}
                  <button
                    onClick={() => removeFilter(filter.key)}
                    className="ml-1 rounded-full hover:bg-gray-200 p-1"
                    aria-label={`Xóa bộ lọc ${filter.label}`}
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
      </div>
    </section>
  );
};

export default ActionHeader;
