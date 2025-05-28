import StatCard from "@/components/StatCard";
import { motion } from "framer-motion";
import { LuPackage, LuDollarSign } from "react-icons/lu";
import { FiAlertTriangle } from "react-icons/fi";
import axios from "axios";
import React, { useEffect, useState } from "react";

interface GeneralStats {
  totalOrders: number;
  totalCustomers: number;
  totalProducts: number;
  orderSuccessRate: number;
  totalRevenue: number;
}

export default function StatDashboard() {
  const [stats, setStats] = useState<GeneralStats>({
    totalOrders: 0,
    totalCustomers: 0,
    totalProducts: 0,
    orderSuccessRate: 0,
    totalRevenue: 0,
  });

  useEffect(() => {
    axios
      .get("https://localhost:7293/api/Statistics/general")
      .then((res) => {
        console.log("API data:", res.data.data);
        if (res.data.code === 200 && res.data.data) {
          const data = res.data.data;
          setStats({
            totalOrders: typeof data.totalOrders === "number" ? data.totalOrders : 0,
            totalCustomers: typeof data.totalCustomers === "number" ? data.totalCustomers : 0,
            totalProducts: typeof data.totalProducts === "number" ? data.totalProducts : 0,
            orderSuccessRate: typeof data.orderSuccessRate === "number" ? data.orderSuccessRate : 0,
            totalRevenue: typeof data.totalRevenue === "number" ? data.totalRevenue : 0,
          });
        }
      })
      .catch((error) => {
        console.error("Error loading stats:", error);
      });
  }, []);

  return (
    <div>
      <motion.div
        className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 1 }}
      >
        <StatCard
          name="Tổng số đơn hàng"
          icon={LuPackage}
          value={stats.totalOrders}
          color="#6366F1"
        />
        <StatCard
          name="Tỉ lệ đơn hàng thành công"
          icon={FiAlertTriangle}
          value={`${stats.orderSuccessRate.toFixed(2)}%`}
          color="#F59E0B"
        />
        <StatCard
          name="Doanh thu"
          icon={LuDollarSign}
          value={`$${stats.totalRevenue.toLocaleString()}`}
          color="#EF4444"
        />
      </motion.div>
    </div>
  );
}
