import { LuSquarePen } from "react-icons/lu";
import { RiDeleteBin3Line } from "react-icons/ri";
import { TableCell, TableRow } from "./ui/table";
import { useState } from "react";
import ConfirmDeleteModal from "./ConfirmDeleteModal";

type TableRowProps<T> = {
  data?: T;
  columns: {
    key?: keyof T;
    className?: string;
    render?: (value: T[keyof T], data: T) => React.ReactNode;
    isActionColumn?: boolean;
    action?: (data: T) => void;
    deleteAction?: (id: string) => void;
  }[];
};

const TableRowComponent = <T extends { id: string },>({ data, columns }: TableRowProps<T>) => {
  const [isModalDeleteOpen, setModalDeleteOpen] = useState(false);

  const handleDelete = () => {
    if (data) {
      const deleteColumn = columns.find(col => col.deleteAction);
      deleteColumn?.deleteAction?.(data.id);
    }
    setModalDeleteOpen(false);
  };

  return (
    <>
      <TableRow>
        {columns.map((column, index) => (
          <TableCell
            key={index}
            className={`${column.className || ""} py-4 text-center`}
          >
            {column.isActionColumn ? (
              data ? (
                <div className="flex flex-grow gap-2">
                  <button onClick={() => column.action?.(data)}>
                    <LuSquarePen className="text-indigo-600" size={20} />
                  </button>
                  <button onClick={() => setModalDeleteOpen(true)}>
                    <RiDeleteBin3Line className="text-red-600" size={20} />
                  </button>
                </div>
              ) : null
            ) : column.render && data ? (
              column.render(data[column.key!], data)
            ) : data && column.key ? (
              typeof data[column.key] === "string" &&
              (data[column.key] as string).startsWith("http") ? (
                <img
                  src={data[column.key] as string}
                  alt="Product"
                  className="h-12 w-12 object-cover rounded"
                />
              ) : (
                String(data[column.key])
              )
            ) : (
              "-"
            )}
          </TableCell>
        ))}
      </TableRow>
      {data && (
        <ConfirmDeleteModal
          isOpen={isModalDeleteOpen}
          onClose={() => setModalDeleteOpen(false)}
          onConfirm={handleDelete}
          itemName={data.id}
        />
      )}
    </>
  );
};

export default TableRowComponent;
