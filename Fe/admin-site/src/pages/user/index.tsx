import React from "react";
import { Helmet } from "react-helmet-async";
import UsersTable from "./sections/Tabledata";

const Voucher: React.FC = () => {
  return (
    <>
      <Helmet>
        <title>Tài Khoản</title>
      </Helmet>
      <section className="px-8">
        {/* Bảng danh sách tài khoản */}
        <UsersTable />
      </section>
    </>
  );
};

export default Voucher;
