import React, { useEffect, useState } from "react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { updateComment, fetchCommentById } from "@/redux/apps/comment/commentSlice";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

interface DetailCommentSheetProps {
  commentId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailCommentSheet: React.FC<DetailCommentSheetProps> = ({
  commentId,
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [status, setStatus] = useState<number>(1);
  const [loading, setLoading] = useState(true);

  const comment = useAppSelector(
    (state) => state.comment.comments.find((c) => c.id === commentId)
  );

  useEffect(() => {
    if (isOpen && commentId) {
      setLoading(true);
      dispatch(fetchCommentById(commentId)).finally(() => setLoading(false));
    }
  }, [isOpen, commentId, dispatch]);

  useEffect(() => {
    if (comment) {
      setStatus(comment.status);
    }
  }, [comment]);

  const handleSave = async () => {
    if (!comment) return;
    await dispatch(
      updateComment({ id: commentId, data: { status, id: commentId } })
    );
    setIsEditing(false);
    onClose();
  };

  if (loading || !comment) {
    return (
      <Sheet open={isOpen} onOpenChange={onClose}>
        <SheetContent>
          <div className="p-4">Đang tải dữ liệu...</div>
        </SheetContent>
      </Sheet>
    );
  }

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle>Chi tiết bình luận</SheetTitle>
        </SheetHeader>
        <div className="space-y-6 p-4">
          <div>
            <label className="block font-medium mb-1">Tên tài khoản</label>
            <input
              className="w-full border rounded px-3 py-2 bg-gray-100"
              value={comment.username || "Ẩn danh"}
              disabled
              readOnly
            />
          </div>
          <div>
            <label className="block font-medium mb-1">Nội dung đánh giá</label>
            <textarea
              className="w-full border rounded px-3 py-2 bg-gray-100"
              value={comment.message}
              rows={5}
              disabled
              readOnly
            />
          </div>
          <div>
            <label className="block font-medium mb-1">Trạng thái</label>
            <Select
              value={String(status)}
              onValueChange={(val) => setStatus(Number(val))}
              disabled={!isEditing}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Chọn trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="1">Hiển thị</SelectItem>
                <SelectItem value="0">Ẩn</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div className="flex justify-end gap-2">
            {!isEditing ? (
              <Button type="button" onClick={() => setIsEditing(true)}>
                Chỉnh sửa trạng thái
              </Button>
            ) : (
              <>
                <Button type="button" variant="outline" onClick={() => {
                  setStatus(comment.status);
                  setIsEditing(false);
                }}>
                  Hủy
                </Button>
                <Button type="button" onClick={handleSave}>
                  Lưu
                </Button>
              </>
            )}
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
};

export default DetailCommentSheet;