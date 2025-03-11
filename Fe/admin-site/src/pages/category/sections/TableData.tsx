import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import { categories } from "@/types/category/seed";
import CategoryTableProps from "@/types/category/table";
import React from "react";

type Category = {
  index: number;
  code: string;
  name: string;
  source: string;
  description : string;
  creator : string;
  createAt: string;
};

const CategoryTable = <T,>({ headers, data, columns }: CategoryTableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
    {/* <Table className="w-full">
      <TableHeaderComponent headers={headers} />
    </Table> */}
    <div className="md:max-h-80 lg:max-h-full max-w-full overflow-x-auto overflow-y-auto">
      <Table className="w-full">
        <TableHeaderComponent headers={headers} className="text-center" />
        <TableBody>
          {data.length ? (
            data.map((row, index) => (
              <TableRowComponent key={index} data={row} columns={columns} />
            ))
          ) : (
            <TableRowComponent
              key="empty-row"
              columns={columns}
              data={undefined}
            />
          )}
        </TableBody>
      </Table>
    </div>
  </div>
);

const CategoriesTable: React.FC = () => {
  const headers = [
    { label: "#", className: "text-center" },
    { label: "Code" },
    { label: "Name" },
    { label: "Source" },
    { label: "Description" },
    { label: "Creator" },
    { label: "Create At" },
    { label: " " },
  ];

  const columns: {
    key?: keyof Category;
    className?: string;
    isActionColumn?: boolean;
    action?: (data: Category) => void;
  }[] = [
    { key: "index", className: "text-center" },
    { key: "code" },
    { key: "name" },
    { key: "source" },
    { key: "description" },
    { key: "creator" },
    { key: "createAt" },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
    },
  ];

  return (
    <section className="mt-10">
      <CategoryTable<Category>
        headers={headers}
        data={categories}
        columns={columns}
      />
      <Pagination />
    </section>
  );
};

export default CategoriesTable;
