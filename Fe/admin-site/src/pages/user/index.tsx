import React from "react";
import { Helmet } from "react-helmet-async";
// import ActionHeader from "./sections/Action";
import UsersTable from "./sections/Tabledata";

const Voucher: React.FC = () => {
  return (
    <>
      <Helmet>
        <title>Tài Khoản</title>
      </Helmet>
      <section className="px-8">
        {/* <ActionHeader /> */}
        {/* Phần bảng danh sách voucher */}
        {/* Tiêu đề bảng */}
        <h2 className="text-2xl mb-6">Danh sách tài khoản</h2>
        <UsersTable />
      </section>
    </>
  );
};

export default Voucher;
