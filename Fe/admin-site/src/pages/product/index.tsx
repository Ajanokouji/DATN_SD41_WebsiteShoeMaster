import React, { useState } from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import CategoriesTable from "./sections/TableData";
import SideCategoryFilter from "./sections/SideCategoriesFilter";

const Product: React.FC = () => {
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(
    null
  ); // State để lưu id danh mục được chọn

  const handleCategorySelect = (categoryId: string | null) => {
    setSelectedCategoryId(categoryId); // Cập nhật id danh mục được chọn
  };

  return (
    <>
      <Helmet>
        <title>Sản Phẩm</title>
      </Helmet>

      <section className="min-h-screen bg-white">
  <div className="mx-auto flex w-full max-w-[1500px]">
    {/* Sidebar bên trái */}
    <aside className="w-64 shrink-0 border-r px-0.1 py-6">
      <SideCategoryFilter onCategorySelect={handleCategorySelect} />
    </aside>

    {/* Nội dung chính bên phải */}
    <main className="flex-1 px-0.1 py-6 overflow-auto">
      <ActionHeader />
      <CategoriesTable selectedCategoryId={selectedCategoryId} />
    </main>
  </div>
</section>




    </>
  );
};

export default Product;