import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import CategoriesTable from "./sections/TableData";

const Category: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Danh Mục </title>
      </Helmet>
      <section className="px-8">
        
        <ActionHeader />
        <CategoriesTable />
      </section>
    </>
  );
};

export default Category;
