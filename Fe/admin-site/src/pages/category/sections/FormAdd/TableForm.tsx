import CustomSelect from "@/components/CustomSelect";
import DatePickerInput from "@/components/DateInput";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Button } from "@/components/ui/button";
import { Table, TableBody } from "@/components/ui/table";
import { Comment } from "@/types/category/category";
import {
  comments,
  resultCommentOptions,
  statusCommentOptions,
} from "@/types/category/seed";
import CategoryTableProps from "@/types/category/table";
import { PlusCircle } from "lucide-react";
import React from "react";

interface CustomTableFormProps {
  values: Comment[];
  handleChange: (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => void;
  setFieldValue: (
    field: string,
    value: string | number | Comment[],
    shouldValidate?: boolean
  ) => void;
}

const CustomTable = <T,>({ headers, data, columns }: CategoryTableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
    <div className="max-h-80 max-w-full overflow-x-auto overflow-y-auto">
      <Table className="w-full">
        <TableHeaderComponent headers={headers} />
        <TableBody>
          {data.map((row, index) => (
            <TableRowComponent key={index} data={row} columns={columns} />
          ))}
        </TableBody>
      </Table>
    </div>
    <div className="w-full">
      <Button
        variant="outline"
        type="button"
        className="text-yellow-600 border-2 rounded-none border-dashed p-[25px] w-full sticky bottom-0"
      >
        <PlusCircle /> Thêm
      </Button>
    </div>
  </div>
);

const TableForm: React.FC<CustomTableFormProps> = ({
  values,
  setFieldValue,
}) => {
  const headers = [
    { label: "Lần", className: "w-[50px] text-center" },
    { label: "Ngày", className: "text-center" },
    { label: "Kết quả chăm sóc", className: "text-center" },
    { label: "Cập nhật trạng thái", className: "text-center" },
  ];

  const handleCommentChange = (
    index: number,
    field: keyof Comment,
    value: string | number
  ) => {
    const updatedComments = [...values];
    updatedComments[index] = {
      ...updatedComments[index],
      [field]: value,
    };
    setFieldValue("comments", updatedComments);
  };
  
  const columns: {
    key: keyof Comment;
    className?: string;
    render?: (
      value: Comment[keyof Comment],
      data: Comment,
      index: number
    ) => React.ReactNode;
  }[] = [
    { key: "index", className: "text-center" },
    {
      key: "time",
      className: "text-center",
      render: (_, __, index) => (
        <DatePickerInput
          selected={
            values[index]?.time ? new Date(values[index]?.time) : undefined
          }
          onChange={(date) =>
            handleCommentChange(index, "time", date.toISOString())
          }
        />
      ),
    },
    {
      key: "title",
      className: "text-center",
      render: (_, __, index) => (
        <CustomSelect
          options={resultCommentOptions}
          onChange={(value) => handleCommentChange(index, "title", value)}
        />
      ),
    },
    {
      key: "status_id",
      className: "text-center",
      render: (_, __, index) => (
        <CustomSelect
          options={statusCommentOptions}
          onChange={(value) => handleCommentChange(index, "status_id", value)}
        />
      ),
    },
  ];

  return (
    <section className="mt-10">
      <CustomTable<Comment>
        headers={headers}
        data={comments}
        columns={columns}
      />
    </section>
  );
};

export default TableForm;
