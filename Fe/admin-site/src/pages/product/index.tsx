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

      <section className="flex min-h-screen">
        {/* Sidebar bên trái */}
        <SideCategoryFilter onCategorySelect={handleCategorySelect} />

        {/* Nội dung chính bên phải */}
        <div className="flex-1 px-8 py-6">
          <ActionHeader />
          <CategoriesTable selectedCategoryId={selectedCategoryId} /> {/* Truyền id danh mục */}
        </div>
      </section>
    </>
  );
};

export default Product;