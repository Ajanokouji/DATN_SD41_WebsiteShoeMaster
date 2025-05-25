import React, { useEffect, useState } from "react";
import axios from "axios";
import { motion } from "framer-motion";
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, Legend } from "recharts";

const COLORS = ["#6366F1", "#8B5CF6", "#EC4899", "#10B981", "#F59E0B"];

interface CategorySale {
  categoryId: string;
  categoryName: string;
  totalQuantitySold: number;
}

interface PieChartData {
  name: string;
  value: number;
}

const CategoryDistributionChart = () => {
  const [categoryData, setCategoryData] = useState<PieChartData[]>([]);

  useEffect(() => {
    axios
      .get<CategorySale[]>("https://localhost:7293/api/Statistics/sales-by-category")
      .then((response) => {
        const filtered = response.data.filter(item => item.totalQuantitySold > 0); // loại bỏ 0%
        const formattedData: PieChartData[] = filtered.map((item) => ({
          name: item.categoryName,
          value: item.totalQuantitySold,
        }));
        setCategoryData(formattedData);
      })
      .catch((error) => {
        console.error("Failed to fetch category sales data", error);
      });
  }, []);

  const getColor = (index: number) => COLORS[index] ?? `hsl(${index * 60}, 70%, 50%)`;

  return (
    <motion.div
      className='text-gray-800 bg-opacity-50 backdrop-blur-md shadow-lg rounded-xl p-6 border border-gray-300'
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: 0.3 }}
    >
      <h2 className='text-lg font-medium mb-4 text-gray-900'>Phân phối danh mục</h2>
      <div className='h-80'>
        <ResponsiveContainer width={"100%"} height={"100%"}>
          <PieChart>
            <Pie
              data={categoryData}
              cx={"50%"}
              cy={"50%"}
              labelLine={false}
              outerRadius={80}
              fill='#8884d8'
              dataKey='value'
              label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
            >
              {categoryData.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={getColor(index)} />
              ))}
            </Pie>
            <Tooltip
              contentStyle={{
                backgroundColor: "rgba(31, 41, 55, 0.8)",
                borderColor: "#4B5563",
              }}
              itemStyle={{ color: "#E5E7EB" }}
            />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </div>
    </motion.div>
  );
};

export default CategoryDistributionChart;
