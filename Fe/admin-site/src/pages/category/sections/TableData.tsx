import React, { useEffect } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import CategoryTableProps from "@/types/category/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectProducts } from "@/redux/apps/category/CategorySelector";
import { fetchProducts } from "@/redux/apps/category/categorySlice";
import { Product } from "@/types/category/category";

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
    { label: "Code", className: "text-center" },
    { label: "Name" },
    { label: "Image" },
    { label: "Description" },
    { label: "Status" },
    { label: "Create At" },
    { label: " " },
  ];

  const columns: {
    key?: keyof Product;
    className?: string;
    isActionColumn?: boolean;
    action?: (data: Product) => void;
  }[] = [
    { key: "code", className: "text-center" },
    { key: "name" },
    { key: "imageUrl" },
    { key: "description" },
    { key: "status" },
    { key: "createdOnDate" },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
    },
  ];

  const dispatch = useAppDispatch();
  const products = useAppSelector(selectProducts);

  useEffect(() => {
    dispatch(fetchProducts({ CurrentPage: 1, PageSize: 20 }));
  }, [dispatch]);

  return (
    
    <section className="mt-10">
      <CategoryTable<Product>
        headers={headers}
        data={products}
        columns={columns}
      />
      <Pagination />
    </section>
  );
};

export default CategoriesTable;
