import React, { useEffect, useState } from "react";
import axios from "axios";
import {
	LineChart,
	Line,
	XAxis,
	YAxis,
	CartesianGrid,
	Tooltip,
	ResponsiveContainer,
} from "recharts";
import { motion } from "framer-motion";

interface MonthlyRevenueDto {
	month: number;
	year: number;
	totalRevenue: number;
}

const SalesOverviewChart = () => {
	const [salesData, setSalesData] = useState<{ name: string; sales: number }[]>([]);

	useEffect(() => {
		axios
			.get<MonthlyRevenueDto[]>("https://localhost:7293/api/Statistics/revenue/monthly")
			.then((response) => {
				const formatted = response.data.map((item) => ({
					name: `T${item.month}`, // Format tháng
					sales: item.totalRevenue,
				}));
				setSalesData(formatted);
			})
			.catch((error) => {
				console.error("Failed to fetch monthly revenue", error);
			});
	}, []);

	return (
		<motion.div
			className='text-gray-800 bg-opacity-50 backdrop-blur-md shadow-lg rounded-xl p-6 border border-gray-300'
			initial={{ opacity: 0, y: 20 }}
			animate={{ opacity: 1, y: 0 }}
			transition={{ delay: 0.2 }}
		>
			<h2 className='text-lg font-medium mb-4 text-gray-900'>Tổng quan đơn hàng</h2>

			<div className='h-96'> {/* Tăng chiều cao nếu cần */}
				<ResponsiveContainer width='100%' height='100%'>
					<LineChart
						data={salesData}
						margin={{ top: 10, right: 30, left: 30, bottom: 5 }} // Thêm margin trái
					>
						<CartesianGrid strokeDasharray='3 3' stroke='#4B5563' />
						<XAxis dataKey='name' stroke='#9ca3af' />
						<YAxis stroke='#9ca3af' tickMargin={10} /> {/* Thêm tickMargin */}
						<Tooltip
							contentStyle={{
								backgroundColor: "rgba(31, 41, 55, 0.8)",
								borderColor: "#4B5563",
							}}
							itemStyle={{ color: "#E5E7EB" }}
						/>
						<Line
							type='monotone'
							dataKey='sales'
							stroke='#6366F1'
							strokeWidth={3}
							dot={{ fill: "#6366F1", strokeWidth: 2, r: 6 }}
							activeDot={{ r: 8, strokeWidth: 2 }}
						/>
					</LineChart>
				</ResponsiveContainer>
			</div>
		</motion.div>
	);
};

export default SalesOverviewChart;
