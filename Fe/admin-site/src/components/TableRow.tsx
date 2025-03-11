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
  }[];
};

const TableRowComponent = <T,>({ data, columns }: TableRowProps<T>) => {
  const [isModalDeleteOpen, setModalDeleteOpen] = useState(false);

  const handleDelete = () => {
    console.log("Item deleted");
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
                  <button>
                    <RiDeleteBin3Line
                      className="text-red-600"
                      size={20}
                      onClick={() => setModalDeleteOpen(true)}
                    />
                  </button>
                </div>
              ) : null
            ) : column.render && data ? (
              column.render(data[column.key!], data)
            ) : (
              data && column.key ? String(data[column.key]) : "-"
            )}
          </TableCell>
        ))}
      </TableRow>
      {data && (
        <ConfirmDeleteModal
          isOpen={isModalDeleteOpen}
          onClose={() => setModalDeleteOpen(false)}
          onConfirm={handleDelete}
          itemName="Sample Item"
        />
      )}
    </>
  );
};

export default TableRowComponent;