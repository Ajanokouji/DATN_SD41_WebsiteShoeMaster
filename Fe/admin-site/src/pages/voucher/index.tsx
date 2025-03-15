import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import VouchersTable from "./sections/TableData";

const Voucher: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Voucher </title>
      </Helmet>
      <section className="px-10">
        <ActionHeader />
        <VouchersTable />
      </section>
    </>
  );
};

export default Voucher;
