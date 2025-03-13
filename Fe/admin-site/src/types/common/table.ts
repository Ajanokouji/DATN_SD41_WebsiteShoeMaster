type TableProps<T> = {
  headers: { label: string; className?: string }[];
  data: T[];
  columns: TableColumn<T>[];
};

type TableColumn<T> = {
  key?: keyof T;
  className?: string;
  render?: (value: T[keyof T], data: T) => React.ReactNode;
  isActionColumn?: boolean;
  action?: (data: T) => void;
};

export default TableProps;
