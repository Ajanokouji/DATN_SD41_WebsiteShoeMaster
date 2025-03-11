import { motion } from "framer-motion";
import StatCard from "@/components/StatCard";
import { LuZap } from "react-icons/lu";
import { FiUsers } from "react-icons/fi";
import { RiShoppingBag3Line } from "react-icons/ri";
import { IoStatsChart } from "react-icons/io5";

const StatDashboard = () => {
  return (
    <motion.div
      className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8"
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 1 }}
    >
      <StatCard
        name="Total Order"
        icon={LuZap}
        value="$12,345"
        color="#6366F1"
      />
      <StatCard
        name="New Customer"
        icon={FiUsers}
        value="1,234"
        color="#8B5CF6"
      />
      <StatCard
        name="Total Product"
        icon={RiShoppingBag3Line}
        value="567"
        color="#EC4899"
      />
      <StatCard
        name="Order success rate"
        icon={IoStatsChart}
        value="12.5%"
        color="#10B981"
      />
    </motion.div>
  );
};

export default StatDashboard;
