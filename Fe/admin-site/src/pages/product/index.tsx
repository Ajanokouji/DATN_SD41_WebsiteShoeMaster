import React, { useState } from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import CategoriesTable from "./sections/TableData";
import SideCategoryFilter from "./sections/SideCategoriesFilter";

const Product: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedFolder, setSelectedFolder] = useState("admin");

  return (
    <>
      <Helmet>
        <title>Sản Phẩm</title>
      </Helmet>

      <section className="flex min-h-screen">
        {/* Sidebar bên trái */}
        <SideCategoryFilter
          searchTerm={searchTerm}
          setSearchTerm={setSearchTerm}
          selectedFolder={selectedFolder}
          setSelectedFolder={setSelectedFolder}
        />

        {/* Nội dung chính bên phải */}
        <div className="flex-1 px-8 py-6">
          <ActionHeader />
          <CategoriesTable />
        </div>
      </section>
    </>
  );
};

export default Product;
