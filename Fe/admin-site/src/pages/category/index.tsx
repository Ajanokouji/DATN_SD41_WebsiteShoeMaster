import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import CategoriesTable from "./sections/TableData";
import Stat from "./sections/Stat";

const Category: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Category </title>
      </Helmet>
      <section className="px-8">
        <Stat />
        <ActionHeader />
        <CategoriesTable />
      </section>
    </>
  );
};

export default Category;
