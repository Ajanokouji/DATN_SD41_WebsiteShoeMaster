import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { useLocation } from "react-router-dom";
import {
  fetchContentBases,
  deleteContentBase,
  setPage,
  setPageSize,
} from "@/redux/apps/contentBase/contentBaseSlice";
import {
  selectContentBases,
  selectPagination,
} from "@/redux/apps/contentBase/contentBaseSelector";
import { formatVietnamTime } from "@/utils/format";
import DetailContentBaseSheet from "./UpdateDetail/DetailContentBaseSheet";

const ContentBaseTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: TableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
    <div className="max-h-[58vh] max-w-full overflow-x-auto overflow-y-auto">
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

const ContentBasesTable: React.FC = () => {
  const [selectedContentBaseId, setSelectedContentBaseId] = useState<string | null>(null);
  const [isOpenUpdate, setIsOpenUpdate] = useState(false);

  const handleOpenDialogUpdate = (id: string) => {
    setIsOpenUpdate(true);
    setSelectedContentBaseId(id);
  };

  const dispatch = useAppDispatch();
  const contentBases = useAppSelector(selectContentBases);
  const pagination = useAppSelector(selectPagination);

  const handleDelete = (id: string) => {
    dispatch(deleteContentBase(id));
  };

  const headers = [
    { label: "Tiêu đề" },
    { label: "SEO Title" },
    { label: "SEO Uri" },
    { label: "Ngày bắt đầu xuất bản" },
    { label: "Ngày kết thúc xuất bản" },
    { label: "Trạng thái xuất bản" },
    { label: "Thao tác" },
  ];

  const columns: {
    key?: keyof typeof contentBases[0];
    isActionColumn?: boolean;
    render?: (value: any, row?: typeof contentBases[0]) => React.ReactNode;
    action?: (data: typeof contentBases[0]) => void;
    deleteAction?: (id: string) => void;
    updateAction?: (id: string) => void;
  }[] = [
    {
      key: "title",
    },
    {
      key: "seoTitle",
    },
    {
      key: "seoUri",
    },
    {
      key: "publishStartDate",
      render: (value) => formatVietnamTime(value),
    },
    {
      key: "publishEndDate",
      render: (value) => formatVietnamTime(value),
    },
    {
      key: "isPublish",
      render: (value) => (value ? "Đã xuất bản" : "Chưa xuất bản"),
    },
    {
      isActionColumn: true,
      deleteAction: (id: string) => {
        handleDelete(id);
      },
      updateAction: (id: string) => {
        handleOpenDialogUpdate(id);
      },
    },
  ];

  const location = useLocation();

  useEffect(() => {
  const params = new URLSearchParams(location.search);
  const title = params.get("title") || "";
  const seoUri = params.get("seoUri") || "";
  const isDeleted = params.get("isDeleted") || "false";

  dispatch(
    fetchContentBases({
      CurrentPage: pagination.currentPage,
      PageSize: pagination.pageSize,
      title,
      seoUri,
      isDeleted: isDeleted === "1" || isDeleted === "true", // Chuyển đổi thành boolean
    })
  );
}, [dispatch, pagination.currentPage, pagination.pageSize, location.search]);
  const handlePageChange = (newPage: number) => {
    dispatch(setPage(newPage));
  };

  const handlePageSizeChange = (newSize: number) => {
    dispatch(setPageSize(newSize));
  };

  return (
    <section className="mt-10">
      {/* Table */}
      <div className="max-w-full overflow-x-auto">
        <div className="max-w-full mx-auto">
          <ContentBaseTable<typeof contentBases[0]>
            headers={headers}
            data={contentBases}
            columns={columns}
          />
        </div>
      </div>

      {/* Pagination */}
      <Pagination
        currentPage={pagination.currentPage}
        totalPages={pagination.totalPages}
        pageSize={pagination.pageSize}
        totalRecords={pagination.totalRecords}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />

      {isOpenUpdate && selectedContentBaseId && (
        <DetailContentBaseSheet
          contentBaseId={selectedContentBaseId}
          isOpen={isOpenUpdate}
          onClose={() => setIsOpenUpdate(false)}
        />
      )}
    </section>
  );
};

export default ContentBasesTable;