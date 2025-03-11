import StatCard from "@/components/StatCard";
import { motion } from "framer-motion";
import { LuPackage } from "react-icons/lu";
import { FiAlertTriangle } from "react-icons/fi";
import { LuDollarSign } from "react-icons/lu";

const Stat = () => {
  return (
    <div>
      <motion.div
        className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 1 }}
      >
        <StatCard
          name="Total"
          icon={LuPackage}
          value={1234}
          color="#6366F1"
        />
        <StatCard
          name="Low Stock"
          icon={FiAlertTriangle}
          value={23}
          color="#F59E0B"
        />
        <StatCard
          name="Revenue"
          icon={LuDollarSign}
          value={"$543,210"}
          color="#EF4444"
        />
      </motion.div>
    </div>
  );
};

export default Stat;
