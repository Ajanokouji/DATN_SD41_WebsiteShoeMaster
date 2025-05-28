import React, { useEffect } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { CommentResDto } from "@/types/comment/comment";
import DetailCommentSheet from "./DetailCommentSheet";

import {
  deleteComment,
  setPage,
  setPageSize,
  fetchComments,
} from "@/redux/apps/comment/commentSlice";

import {
  selectCommentPagination,
  selectComments,
} from "@/redux/apps/comment/commentSelector";
import { formatVietnamTime } from "@/utils/format";

const CommentTable = <T extends { id: string }>({
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

const CommentsTable: React.FC<{ objectId: string }> = ({ objectId }) => {
  const dispatch = useAppDispatch();

  const comments = useAppSelector(selectComments);
  const pagination = useAppSelector(selectCommentPagination);

  // State để mở sheet chi tiết
  const [openDetail, setOpenDetail] = React.useState(false);
  const [selectedCommentId, setSelectedCommentId] = React.useState<string | null>(null);

  useEffect(() => {
    if (objectId) {
      dispatch(
        fetchComments({
          objectId,
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
        })
      );
    }
  }, [objectId, dispatch, pagination.currentPage, pagination.pageSize]);

  const renderDate = (date: string) => {
    if(date.includes("T")) {
      date = date.replace("T", " ").replace("Z", "");
    }
    return `${formatVietnamTime(date)}`;
  };

  const headers = [
    { label: "Tên tài khoản" },
    { label: "Nội dung đánh giá" },
    { label: "Ngày viết đánh giá" },
    { label: "Trạng thái" }, // Thêm cột trạng thái
    { label: "Thao tác"},
  ];

  const columns: {
    key?: keyof CommentResDto;
    isActionColumn?: boolean;
    render?: (value: any, row?: CommentResDto) => React.ReactNode;
    updateAction?: (id: string) => void;
    deleteAction?: (id: string) => void;
  }[] = [
    {
      key: "username",
      render: (value) => (
        <div>
          {value ? value : "Ẩn danh"}
        </div>
      ),
    },
    {
      key: "message",
      render: (value, row) => (
        <div
          className="cursor-pointer hover:underline"
          onClick={() => {
            setSelectedCommentId(row?.id ?? "");
            setOpenDetail(true);
          }}
        >
          {value.slice(0, 50)}..
        </div>
      ),
    },
    {
      key: "createdOnDate",
      render: (value) => <div>{renderDate(value)}</div>
    },
    {
      key: "status",
      render: (value) => (
        <span className={value === 1 ? "text-green-600" : "text-gray-400"}>
          {value === 1 ? "Hiển thị" : "Ẩn"}
        </span>
      ),
    },
    {
      isActionColumn: true,
      updateAction: (id: string) => {
        setSelectedCommentId(id);
        setOpenDetail(true);
      },
      deleteAction: (id: string) => {
        dispatch(deleteComment(id)).then(() => {
          dispatch(
            fetchComments({
              objectId,
              CurrentPage: pagination.currentPage,
              PageSize: pagination.pageSize,
            })
          );
        });
      },
    },
  ];

  const handlePageChange = (newPage: number) => {
    dispatch(setPage(newPage));
  };

  const handlePageSizeChange = (newSize: number) => {
    dispatch(setPageSize(newSize));
  };

  return (
    <section className="mt-10">
      <div className="max-w-full overflow-x-auto">
        <div className="max-w-full mx-auto">
          <CommentTable<CommentResDto>
            headers={headers}
            data={comments}
            columns={columns}
          />
        </div>
      </div>

      <Pagination
        currentPage={pagination.currentPage}
        totalPages={pagination.totalPages}
        pageSize={pagination.pageSize}
        totalRecords={pagination.totalRecords}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />

      {/* DetailCommentSheet */}
      {selectedCommentId && (
        <DetailCommentSheet
          commentId={selectedCommentId}
          isOpen={openDetail}
          onClose={() => setOpenDetail(false)}
        />
      )}
    </section>
  );
};

export default CommentsTable;