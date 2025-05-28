import React, { useEffect, useState } from "react";
import { Input } from "@/components/ui/input";
import { FolderOpen } from "lucide-react";
import categoryService from "@/redux/api/categoryApi";
import { CategoryResDto } from "@/types/category/category";

interface SideCategoryFilterProps {
  onCategorySelect: (categoryId: string | null) => void; // Callback để truyền id danh mục
}

const SideCategoryFilter: React.FC<SideCategoryFilterProps> = ({
  onCategorySelect,
}) => {
  const [categories, setCategories] = useState<CategoryResDto[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedFolder, setSelectedFolder] = useState<string | null>(null);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await categoryService.getCategories({
          CurrentPage: 1,
          PageSize: 20,
        });
        setCategories(response.data.content);
      } catch (error) {
        console.error("Failed to fetch categories", error);
      }
    };

    fetchCategories();
  }, []);

  const filteredCategories = categories.filter((category) =>
    category.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleCategoryClick = (categoryId: string | null) => {
    setSelectedFolder(categoryId); // Cập nhật danh mục được chọn
    onCategorySelect(categoryId); // Gọi callback để truyền id danh mục lên component cha
  };

  return (
    <div className="w-64 bg-background border-r p-4 overflow-y-auto max-h-screen">
      <Input
        placeholder="Tìm danh mục..."
        className="mb-4"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />

<div className="space-y-2 max-w-[170px]">
  <div className="font-medium text-sm">Danh mục</div>
  {filteredCategories.map((category) => (
    <div
      key={category.id}
      className={`flex items-center space-x-2 p-2 rounded cursor-pointer ${
        selectedFolder === category.id ? "bg-accent" : "hover:bg-accent/50"
      }`}
      onClick={() => handleCategoryClick(category.id)}
    >
      <FolderOpen size={20} />
      <span className="truncate">{category.name}</span>
    </div>
  ))}
</div>

    </div>
  );
};

export default SideCategoryFilter;