import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import Stat from "./sections/Stat";
import BillsTable from "./sections/TableData";


const Bill: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Bill </title>
      </Helmet>
      <section className="px-8">
        <Stat />
        <ActionHeader />
        <BillsTable />
      </section>
    </>
  );
};

export default Bill;
