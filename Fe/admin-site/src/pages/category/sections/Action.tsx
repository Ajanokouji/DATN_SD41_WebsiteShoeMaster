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
import { FaPlus, FaFilter } from "react-icons/fa6";
import { Badge } from "@/components/ui/badge";
import { X } from "lucide-react";
import AddCategorySheet from "./FormAdd/AddCategorySheet";
import { FilterItem } from "@/types/common/pagination";

interface FilterData {
  name: string;
  code: string;
  description: string;
}

const ActionHeader = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const [isOpenAdd, setIsOpenAdd] = useState(false);
  const [isOpenFilter, setIsOpenFilter] = useState(false);
  const [activeFilters, setActiveFilters] = useState<FilterItem[]>([]);

  const [filterData, setFilterData] = useState<FilterData>({
    name: "",
    code: "",
    description: "",
  });

  // Khi location.search thay đổi thì cập nhật filterData từ query params
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const newFilterData: FilterData = {
      name: params.get("name") || "",
      code: params.get("code") || "",
      description: params.get("description") || "",
    };

    setFilterData(newFilterData);

    // Tạo danh sách filter hiển thị
    const newActiveFilters: FilterItem[] = [];
    Object.entries(newFilterData).forEach(([key, value]) => {
      if (value) {
        const label = `${key}: ${value}`;
        newActiveFilters.push({ key, value, label });
      }
    });

    setActiveFilters(newActiveFilters);
  }, [location.search]);

  // Áp dụng filter (cập nhật URL)
  const applyFilters = () => {
    const params = new URLSearchParams();

    Object.entries(filterData).forEach(([key, value]) => {
      if (value) {
        params.set(key, value);
      }
    });

    navigate({
      pathname: location.pathname,
      search: params.toString(),
    });

    setIsOpenFilter(false);
  };

  // Xóa toàn bộ filter
  const clearAllFilters = () => {
    setFilterData({
      name: "",
      code: "",
      description: "",
    });
    setActiveFilters([]);
    navigate(location.pathname);
  };

  // Xóa 1 filter theo key
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

  // Thay đổi filter input
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
          {/* Thanh tìm kiếm nhanh */}
          <div className="flex gap-2 items-center">
            <Input
              type="text"
              placeholder="Tìm kiếm nhanh theo tên hoặc mã danh mục"
              className="shadow-none"
              value={filterData.name || filterData.code}
              onChange={(e) => {
                handleFilterChange("name", e.target.value);
              }}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  applyFilters();
                }
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
                  <SheetTitle>Lọc danh mục</SheetTitle>
                </SheetHeader>

                <div className="grid gap-4 py-4">
                  <div className="grid gap-2">
                    <Label htmlFor="name">Tên danh mục</Label>
                    <Input
                      id="name"
                      value={filterData.name}
                      onChange={(e) => handleFilterChange("name", e.target.value)}
                      placeholder="Nhập tên danh mục"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="code">Mã danh mục</Label>
                    <Input
                      id="code"
                      value={filterData.code}
                      onChange={(e) => handleFilterChange("code", e.target.value)}
                      placeholder="Nhập mã danh mục"
                    />
                  </div>

                  <div className="grid gap-2">
                    <Label htmlFor="description">Mô tả</Label>
                    <Input
                      id="description"
                      value={filterData.description}
                      onChange={(e) => handleFilterChange("description", e.target.value)}
                      placeholder="Tìm kiếm trong mô tả"
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

          {/* Hiển thị bộ lọc đang hoạt động */}
          {activeFilters.length > 0 && (
            <div className="flex flex-wrap gap-2 mt-2">
              {activeFilters.map((filter) => (
                <Badge key={filter.key} variant="secondary" className="flex items-center gap-1">
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

        <div className="flex gap-5 justify-end">
          <Button onClick={() => setIsOpenAdd(true)}>
            <FaPlus className="mr-2" />
            Thêm danh mục
          </Button>
        </div>
      </div>

      {/* Sheet thêm danh mục */}
      {isOpenAdd && (
        <AddCategorySheet
          isOpen={isOpenAdd}
          onClose={() => setIsOpenAdd(false)}
        />
      )}
    </section>
  );
};

export default ActionHeader;