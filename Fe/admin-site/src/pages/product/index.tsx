import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import CategoriesTable from "./sections/TableData";

const Product: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Product </title>
      </Helmet>
      <section className="px-10">
        <ActionHeader />
        <CategoriesTable />
      </section>
    </>
  );
};

export default Product;
