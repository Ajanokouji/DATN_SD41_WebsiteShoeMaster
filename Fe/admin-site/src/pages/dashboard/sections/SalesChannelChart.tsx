import React, { useEffect, useState } from "react";
import axios from "axios";
import { motion } from "framer-motion";
import {
	BarChart,
	Bar,
	XAxis,
	YAxis,
	CartesianGrid,
	Tooltip,
	ResponsiveContainer,
	Cell,
} from "recharts";

const COLORS = ["#6366F1", "#8B5CF6", "#EC4899", "#10B981", "#F59E0B"];

interface ChannelItem {
	name: string;
	value: number;
	source: number;
	color: string;
}

interface SalesChannelDto {
	channelName: string;
	totalQuantitySold: number;
	source: number;
}

const SalesChannelChart = () => {
	const [channelData, setChannelData] = useState<ChannelItem[]>([]);

	useEffect(() => {
		axios
			.get<SalesChannelDto[]>("https://localhost:7293/api/Statistics/sales-by-channel")
			.then((response) => {
				const formatted: ChannelItem[] = response.data
					.filter((item) => item.totalQuantitySold > 0)
					.map((item, index) => ({
						name: item.channelName,
						value: item.totalQuantitySold,
						source: item.source,
						color: COLORS[index % COLORS.length],
					}));
				setChannelData(formatted);
			})
			.catch((error) => {
				console.error("Failed to fetch sales by channel", error);
			});
	}, []);

	// Legend đặt bên ngoài chart
	const renderCustomLegend = () => (
		<div className="flex justify-center flex-wrap gap-4 mt-4 text-sm text-gray-700">
			{channelData.map((item, index) => {
				const sourceLabel =
					item.source === 1
						? "Bán hàng online"
						: item.source === 2
						? "Bán hàng offline"
						: "Khác";
				return (
					<div key={index} className="flex items-center gap-1">
						<span
							className="w-3 h-3 inline-block rounded-full"
							style={{ backgroundColor: item.color }}
						></span>
						<span>{sourceLabel}</span>
					</div>
				);
			})}
		</div>
	);

	return (
		<motion.div
			className='text-gray-800 bg-opacity-50 backdrop-blur-md shadow-lg rounded-xl p-6 lg:col-span-2 border border-gray-300'
			initial={{ opacity: 0, y: 20 }}
			animate={{ opacity: 1, y: 0 }}
			transition={{ delay: 0.4 }}
		>
			<h2 className='text-lg font-medium mb-4 text-gray-900'>Các Kênh Bán Hàng</h2>

			<div className='h-80'>
				<ResponsiveContainer>
					<BarChart data={channelData}>
						<CartesianGrid strokeDasharray='3 3' stroke='#4B5563' />
						<XAxis dataKey='name' stroke='#9CA3AF' />
						<YAxis stroke='#9CA3AF' />
						<Tooltip
							formatter={(value: number) => [`${value}`, 'Số lượng']}
							labelStyle={{ color: "#fff" }}
							contentStyle={{
								backgroundColor: "rgba(31, 41, 55, 0.8)",
								borderColor: "#4B5563",
							}}
							itemStyle={{ color: "#E5E7EB" }}
						/>
						{/* Ẩn legend mặc định */}
						{/* <Legend content={renderCustomLegend} /> */}
						<Bar dataKey={"value"}>
							{channelData.map((entry, index) => (
								<Cell key={`cell-${index}`} fill={entry.color} />
							))}
						</Bar>
					</BarChart>
				</ResponsiveContainer>
			</div>

			{/* Hiển thị chú thích ở giữa */}
			{renderCustomLegend()}
		</motion.div>
	);
};

export default SalesChannelChart;
