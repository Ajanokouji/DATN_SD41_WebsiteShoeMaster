import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { 
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue
} from "@/components/ui/select";
import { 
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
  SheetFooter
} from "@/components/ui/sheet";
import { Label } from "@/components/ui/label";
import { FaPlus, FaFilter } from "react-icons/fa6";
import { Badge } from "@/components/ui/badge";
import { X } from "lucide-react";
import AddProductSheet from "./FormAdd/AddProductSheet";
import { FilterItem } from "@/types/common/pagination";

interface FilterData {
  tenSanPham: string;
  maSanPham: string;
  status: string;
  description: string;
}

const ActionHeader = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [isOpenAdd, setIsOpenAdd] = useState(false);
  const [isOpenFilter, setIsOpenFilter] = useState(false);
  const [activeFilters, setActiveFilters] = useState<FilterItem[]>([]);

  // Filter states
  const [filterData, setFilterData] = useState<FilterData>({
    tenSanPham: "",
    maSanPham: "",
    status: "",
    description: ""
  });

  // Status options
  const statusOptions = [
    { value: "active", label: "Hoạt động" },
    { value: "inactive", label: "Không hoạt động" },
    { value: "outOfStock", label: "Hết hàng" }
  ];

  // Parse URL params on component mount
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const newFilterData = { ...filterData };
    const newActiveFilters: FilterItem[] = [];

    // Update filterData from URL params
    Object.keys(filterData).forEach(key => {
      const value = params.get(key);
      if (value) {
        newFilterData[key as keyof FilterData] = value;
        
        // Add to active filters
        const label = getFilterLabel(key, value);
        newActiveFilters.push({ key, value, label });
      }
    });

    setFilterData(newFilterData);
    setActiveFilters(newActiveFilters);
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [location.search]);

  // Helper to get human-readable labels for filters
  const getFilterLabel = (key: string, value: string): string => {
    switch(key) {
      case "tenSanPham": 
        return `Tên: ${value}`;
      case "maSanPham": 
        return `Mã: ${value}`;
      case "status": {
        const statusLabel = statusOptions.find(s => s.value === value)?.label || value;
        return `Trạng thái: ${statusLabel}`;
      }
      case "description": 
        return `Mô tả: ${value}`;
      default: 
        return `${key}: ${value}`;
    }
  };

  // Apply filters
  const applyFilters = () => {
    // Build URL parameters
    const params = new URLSearchParams();
    const newActiveFilters: FilterItem[] = [];
    
    Object.entries(filterData).forEach(([key, value]) => {
      if (value) {
        params.set(key, value);
        const label = getFilterLabel(key, value);
        newActiveFilters.push({ key, value, label });
      }
    });

    // Update active filters state
    setActiveFilters(newActiveFilters);
    
    // Update URL with search params
    navigate({
      pathname: location.pathname,
      search: params.toString()
    });
    
    setIsOpenFilter(false);
  };

  // Clear all filters
  const clearAllFilters = () => {
    setFilterData({
      tenSanPham: "",
      maSanPham: "",
      status: "",
      description: ""
    });
    setActiveFilters([]);
    navigate(location.pathname);
  };

  // Remove single filter
  const removeFilter = (keyToRemove: string) => {
    // Update filterData
    setFilterData(prev => ({
      ...prev,
      [keyToRemove]: ""
    } as FilterData));
    
    // Update active filters
    setActiveFilters(prev => prev.filter(filter => filter.key !== keyToRemove));
    
    // Update URL params
    const params = new URLSearchParams(location.search);
    params.delete(keyToRemove);
    
    navigate({
      pathname: location.pathname,
      search: params.toString()
    });
  };

  // Handle filter input changes
  const handleFilterChange = (name: keyof FilterData, value: string) => {
    setFilterData({
      ...filterData,
      [name]: value
    });
  };

  // Handle status change with ability to clear
  const handleStatusChange = (value: string) => {
    handleFilterChange("status", value);
  };

  return (
    <section className="mb-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 justify-between">
        <div className="flex flex-col w-full space-y-2">
          {/* Quick search input */}
          <div className="flex gap-2 items-center">
            <Input 
              type="text" 
              placeholder="Tìm kiếm nhanh theo tên hoặc mã sản phẩm" 
              className="shadow-none" 
              value={filterData.tenSanPham || filterData.maSanPham}
              onChange={(e) => {
                // Quick search affects both name and code fields
                handleFilterChange("tenSanPham", e.target.value);
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
                  <SheetTitle>Lọc sản phẩm</SheetTitle>
                  <SheetDescription>
                    Thiết lập các điều kiện lọc cho danh sách sản phẩm
                  </SheetDescription>
                </SheetHeader>
                <div className="grid gap-4 py-4">
                  <div className="grid gap-2">
                    <Label htmlFor="tenSanPham">Tên sản phẩm</Label>
                    <Input
                      id="tenSanPham"
                      value={filterData.tenSanPham}
                      onChange={(e) => handleFilterChange("tenSanPham", e.target.value)}
                      placeholder="Nhập tên sản phẩm"
                    />
                  </div>
                  <div className="grid gap-2">
                    <Label htmlFor="maSanPham">Mã sản phẩm</Label>
                    <Input
                      id="maSanPham"
                      value={filterData.maSanPham}
                      onChange={(e) => handleFilterChange("maSanPham", e.target.value)}
                      placeholder="Nhập mã sản phẩm"
                    />
                  </div>
                  <div className="grid gap-2">
                    <Label htmlFor="status">Trạng thái</Label>
                    <Select
                      value={filterData.status}
                      onValueChange={handleStatusChange}
                    >
                      <SelectTrigger id="status">
                        <SelectValue placeholder="Chọn trạng thái" />
                      </SelectTrigger>
                      <SelectContent>
                        {/* Use a non-empty string for "all" option */}
                        <SelectItem value="all">Tất cả</SelectItem>
                        {statusOptions.map(option => (
                          <SelectItem key={option.value} value={option.value}>
                            {option.label}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
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
                  <Button onClick={applyFilters}>
                    Áp dụng
                  </Button>
                </SheetFooter>
              </SheetContent>
            </Sheet>
          </div>
          
          {/* Active filters display */}
          {activeFilters.length > 0 && (
            <div className="flex flex-wrap gap-2 mt-2">
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
        
        <div className="flex gap-5 justify-end">
          <Button onClick={() => setIsOpenAdd(true)}>
            <FaPlus className="mr-2" />
            Thêm sản phẩm
          </Button>
        </div>
      </div>

      {isOpenAdd && (
        <AddProductSheet
          isOpen={isOpenAdd}
          onClose={() => setIsOpenAdd(false)}
        />
      )}
    </section>
  );
};

export default ActionHeader;