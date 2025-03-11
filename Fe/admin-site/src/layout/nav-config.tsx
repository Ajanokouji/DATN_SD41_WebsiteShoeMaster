import { IconType } from "react-icons";
import { IoStatsChart } from "react-icons/io5";
import { RiShoppingBag3Line } from "react-icons/ri";
import { AiOutlineProduct } from "react-icons/ai";
import { LuUsers } from "react-icons/lu";
import { MdAttachMoney } from "react-icons/md";
import { LuShoppingCart } from "react-icons/lu";
import { LuSettings } from "react-icons/lu";

interface NavConfigItem {
  title: string;
  path?: string;
  icon?: IconType;
  color?: string;
  children?: NavConfigItem[];
  description?: string;
}

const navConfig: NavConfigItem[] = [
  {
    title: "Dashboard",
    path: "/",
    icon: IoStatsChart,
    color: "#6366f1",
  },
  {
    title: "Category",
    path: "/category",
    icon: AiOutlineProduct,
    color: "#f87171",
  },
  {
    title: "Product",
    path: "/product",
    icon: RiShoppingBag3Line,
    color: "#8b5cf6",
  },
  {
    title: "Customer",
    path: "/customer",
    icon: LuUsers,
    color: "#ec4899",
  },
  {
    title: "Discount",
    path: "/discount",
    icon: MdAttachMoney,
    color: "#10b981",
  },
  {
    title: "Order",
    path: "/order",
    icon: LuShoppingCart,
    color: "#f59e0b",
  },
  {
    title: "Setting",
    path: "/setting",
    icon: LuSettings,
    color: "#3b82f6",
  },
];

export default navConfig;
