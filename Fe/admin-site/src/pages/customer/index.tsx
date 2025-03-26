import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import CustomersTable from "./sections/TableData";

const Customer: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Customer </title>
      </Helmet>
      <section className="px-8">
        <ActionHeader />
        <CustomersTable />
      </section>
    </>
  );
};

export default Customer;
