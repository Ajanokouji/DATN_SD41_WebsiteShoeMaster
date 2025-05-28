import { IconType } from "react-icons";
import { IoStatsChart } from "react-icons/io5";
import { RiShoppingBag3Line } from "react-icons/ri";
import { AiOutlineProduct } from "react-icons/ai";
import { LuUsers } from "react-icons/lu";
import { MdAttachMoney } from "react-icons/md";
import { LuContact } from "react-icons/lu";
import { TbCirclesRelation } from "react-icons/tb";
import { FaFileInvoiceDollar } from "react-icons/fa6";
import { FaFileAlt } from "react-icons/fa";
import { LuNewspaper } from "react-icons/lu";


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
    title: "Thống Kê",
    path: "/",
    icon: IoStatsChart,
    color: "#6366f1",
  },
  {
    title: "Danh mục",
    path: "/category",
    icon: AiOutlineProduct,
    color: "#f87171",
  },
  {
    title: "Sản phẩm",
    path: "/product",
    icon: RiShoppingBag3Line,
    color: "#8b5cf6",
  },
  // {
  //   title: "Danh Mục Quan Hệ",
  //   path: "/relation",
  //   icon: TbCirclesRelation,
  //   color: "#759a94",
  // },
  {
    title: "Khách hàng",
    path: "/customer",
    icon: LuUsers,
    color: "#ec4899",
  },
  {
    title: "Voucher",
    path: "/voucher",
    icon: MdAttachMoney,
    color: "#10b981",
  },
  {
    title: "Nhân viên",
    path: "/employee",
    icon: LuContact,
    color: "#f59e0b",
  },
  {
    title: "Hóa đơn",
    path: "/bill",
    icon: FaFileInvoiceDollar,
    color: "#EF4444",
  },
  {
    title: "Tài Nguyên",
    path: "/file-manager",
    icon: FaFileAlt,
    color: "#3b82f6",
  },
  {
    title: "Tin tức",
    path: "/contentBase",
    icon: LuNewspaper,
    color: "#4B5563",
  },
  {
    title: "Tài Khoản",
    path: "/User",
    icon: LuNewspaper,
    color: "#4B5693",
  },
];

export default navConfig;
