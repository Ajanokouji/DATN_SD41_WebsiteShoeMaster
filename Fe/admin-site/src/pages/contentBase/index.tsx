import React from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import ContentBaseTable from "./sections/TableData";

const ContentBase: React.FC = () => {
  return (
    <>
      <Helmet>
        <title>Content Base</title>
      </Helmet>
      <section className="px-8">
        <ActionHeader />
        <h2 className="text-2xl mb-6">Danh sách Content Base</h2>
        <ContentBaseTable />
      </section>
    </>
  );
};

export default ContentBase;