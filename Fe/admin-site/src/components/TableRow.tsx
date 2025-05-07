import { LuSquarePen } from "react-icons/lu";
import { IoEye } from "react-icons/io5";
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
    updateAction?: (id: string) => void;
    detailAction?: (id: string) => void;
  }[];
};

const TableRowComponent = <
  T extends {
    id: string;
    name?: string;
    code?: string;
    voucherName?: string;
    fullName?: string;
    billCode?: string;
    varientProductId?: string;
  }
>({
  data,
  columns,
}: TableRowProps<T>) => {
  const [isModalDeleteOpen, setModalDeleteOpen] = useState(false);

  const handleDelete = () => {
    if (data) {
      const deleteColumn = columns.find((col) => col.deleteAction);
      deleteColumn?.deleteAction?.(data.id);
    }
    setModalDeleteOpen(false);
  };

  const getDisplayName = () => {
    if (!data) return "";
    if (data.fullName) return data.fullName;
    if (data.name) return data.name;
    if (data.code) return data.code;
    if (data.voucherName) return data.voucherName;
    if (data.billCode) return data.billCode;
    if (data.varientProductId) return data.varientProductId;
    return data.id;
  };

  return (
    <>
      <TableRow>
        {columns.map((column, index) => (
          <TableCell
            key={index}
            className={`${
              column.className || ""
            } py-4 text-center whitespace-nowrap overflow-hidden text-ellipsis`}
          >
            {column.isActionColumn ? (
              data ? (
                <div className="flex flex-grow gap-2">
                  {column.updateAction && (
                    <button
                      onClick={() =>
                        column.updateAction && column.updateAction(data.id)
                      }
                    >
                      <LuSquarePen className="text-indigo-600" size={20} />
                    </button>
                  )}
                  {column.detailAction && (
                    <button
                      onClick={() =>
                        column.detailAction && column.detailAction(data.id)
                      }
                    >
                      <IoEye className="text-amber-500" size={20} />
                    </button>
                  )}
                  {column.deleteAction && (
                    <button onClick={() => setModalDeleteOpen(true)}>
                      <RiDeleteBin3Line className="text-red-600" size={20} />
                    </button>
                  )}
                </div>
              ) : null
            ) : column.render && data ? (
              <div className="overflow-hidden text-ellipsis">
                {column.render(data[column.key!], data)}
              </div>
            ) : data && column.key ? (
              column.key === "imageUrl" &&
              typeof data[column.key] === "string" &&
              (data[column.key] as string).startsWith("http") ? (
                <div className="flex justify-center">
                  <img
                    src={data[column.key] as string}
                    alt="Product"
                    className="h-12 w-12 object-cover rounded"
                  />
                </div>
              ) : (
                <div
                  className="overflow-hidden text-ellipsis max-w-xs truncate"
                  title={String(data[column.key] ?? "-")}
                >
                  {String(data[column.key] ?? "-")}
                </div>
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
          itemName={getDisplayName()}
        />
      )}
    </>
  );
};

export default TableRowComponent;
