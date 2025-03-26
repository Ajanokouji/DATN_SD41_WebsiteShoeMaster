import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import ContactsTable from "./sections/TableData";

const Contact: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Contact </title>
      </Helmet>
      <section className="px-8">
        <ActionHeader />
        <ContactsTable />
      </section>
    </>
  );
};

export default Contact;
