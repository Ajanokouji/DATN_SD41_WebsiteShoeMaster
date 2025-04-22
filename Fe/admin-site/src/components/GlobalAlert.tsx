import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { clearNotification } from "@/redux/apps/message/messageSlice";
import { useEffect } from "react";
import { toast } from "sonner";
import { Toaster } from "./ui/sonner";
import { useAppSelector } from "@/hooks/use-app-selector";
import { TriangleAlert } from "lucide-react";

const GlobalAlert = () => {
  const dispatch = useAppDispatch();

  // ✅ Tránh lỗi destructuring từ undefined
  const notification = useAppSelector((state) => state.messages?.notification);

  const message = notification?.message;
  const type = notification?.type;

  useEffect(() => {
    if (message) {
      if (type === "success") {
        toast.success(message, {
          style: {
            backgroundColor: "#E0FBE2",
            color: "#185519",
            border: "1px solid #185519",
          },
        });
      } else if (type === "error") {
        toast.error(message, {
          style: {
            backgroundColor: "#F8EDED",
            color: "#B8001F",
            border: "1px solid #800000",
          },
        });
      } else if (type === "warning") {
        toast(message, {
          style: {
            backgroundColor: "#FFF4E5",
            color: "#664d03",
            border: "1px solid #FFC107",
          },
          icon: <TriangleAlert size={22} />,
        });
      }

      const timeoutId = setTimeout(() => {
        dispatch(clearNotification());
      }, 3000);

      return () => clearTimeout(timeoutId); // Cleanup
    }
  }, [message, type, dispatch]);

  return <Toaster position="bottom-left" />;
};

export default GlobalAlert;
