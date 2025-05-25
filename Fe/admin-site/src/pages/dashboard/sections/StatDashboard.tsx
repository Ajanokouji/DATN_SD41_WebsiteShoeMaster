import { useEffect, useState } from 'react';
import axios from 'axios';
import StatCard from "@/components/StatCard";
import { LuZap } from "react-icons/lu";
import { FiUsers } from "react-icons/fi";
import { RiShoppingBag3Line } from "react-icons/ri";
import { IoStatsChart } from "react-icons/io5";

interface GeneralStats {
  totalOrders: number;
  totalCustomers: number;
  totalProducts: number;
  orderSuccessRate: number;
}

export default function StatDashboard() {
  const [stats, setStats] = useState<GeneralStats>({
    totalOrders: 0,
    totalCustomers: 0,
    totalProducts: 0,
    orderSuccessRate: 0,
  });

  useEffect(() => {
    axios
      .get('https://localhost:7293/api/statistics/general')
      .then((res) => {
        // res.data = { code, message, data }
        if (res.data.code === 200) {
          setStats(res.data.data);
        }
      })
      .catch((error) => {
        console.error('Error loading stats:', error);
      });
  }, []);

  return (
    <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
      <StatCard
        name="Số lượng đơn hàng"
        icon={LuZap}
        value={stats.totalOrders.toString()}
        color="#6366F1"
      />
      <StatCard
        name="Số lượng khách hàng"
        icon={FiUsers}
        value={stats.totalCustomers.toString()}
        color="#8B5CF6"
      />
      <StatCard
        name="Số lượng sản phẩm"
        icon={RiShoppingBag3Line}
        value={stats.totalProducts.toString()}
        color="#EC4899"
      />
      <StatCard
        name="Tỉ lệ đơn hàng thành công"
        icon={IoStatsChart}
        value={stats.orderSuccessRate + "%"}
        color="#10B981"
      />
    </div>
  );
}
