import { TableHead, TableHeader, TableRow } from "./ui/table";

type TableHeaderProps = {
  headers: { label: string; className?: string }[];
  className?: string;
};
const TableHeaderComponent: React.FC<TableHeaderProps> = ({ headers, className = "" }) => (
  <TableHeader className="bg-gray-200 w-full">
    <TableRow>
      {headers.map((header, index) => (
        <TableHead
          key={index}
          className={`${header.className || ""} ${className} text-black py-4`}
        >
          {header.label}
        </TableHead>
      ))}
    </TableRow>
  </TableHeader>
);
export default TableHeaderComponent;
