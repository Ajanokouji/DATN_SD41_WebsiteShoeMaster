import React, { useState } from "react";
import { Helmet } from "react-helmet-async";
import ActionHeader from "./sections/Action";
import RelationsTable from "./sections/TableData";

const Relation: React.FC = () => {
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);

  const handleCategorySelect = (categoryId: string) => {
    setSelectedCategoryId(categoryId);
  };

  return (
    <>
      <Helmet>
        <title> Relation </title>
      </Helmet>
      <section className="px-8">
        <ActionHeader onCategorySelect={handleCategorySelect}/>
        <RelationsTable selectedCategoryId={selectedCategoryId} />
      </section>
    </>
  );
};

export default Relation;
