import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import VouchersTable from "./sections/TableData";

const Voucher: React.FC = () => {
  return (
    <>
      <Helmet>
        <title>Voucher</title>
      </Helmet>
      <section className="px-8">
        <ActionHeader />
        {/* Phần bảng danh sách voucher */}
        {/* Tiêu đề bảng */}
        <h2 className="text-2xl mb-6">Danh sách Voucher</h2>
        <VouchersTable />
      </section>
    </>
  );
};

export default Voucher;
