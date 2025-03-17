import { IconType } from "react-icons";
import { IoStatsChart } from "react-icons/io5";
import { RiShoppingBag3Line } from "react-icons/ri";
import { AiOutlineProduct } from "react-icons/ai";
import { LuUsers } from "react-icons/lu";
import { MdAttachMoney } from "react-icons/md";
import { LuSettings } from "react-icons/lu";
import { LuContact } from "react-icons/lu";
import { TbCirclesRelation } from "react-icons/tb";
import { FaFileInvoiceDollar } from "react-icons/fa6";

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
    title: "Relation",
    path: "/relation",
    icon: TbCirclesRelation,
    color: "#759a94",
  },
  {
    title: "Customer",
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
    title: "Contact",
    path: "/contact",
    icon: LuContact,
    color: "#f59e0b",
  },
  {
    title: "Bill",
    path: "/bill",
    icon: FaFileInvoiceDollar,
    color: "#EF4444",
  },
  {
    title: "Setting",
    path: "/setting",
    icon: LuSettings,
    color: "#3b82f6",
  },
];

export default navConfig;
