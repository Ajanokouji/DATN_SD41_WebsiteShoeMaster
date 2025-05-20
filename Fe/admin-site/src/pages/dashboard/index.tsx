import React from "react";
import { Helmet } from "react-helmet-async";
import StatDashboard from "./sections/StatDashboard";
import SalesOverviewChart from "./sections/SalesOverviewChart";
import CategoryDistributionChart from "./sections/CategoryDistributionChart";
import SalesChannelChart from "./sections/SalesChannelChart";


const Dashboard: React.FC = () => {
  return (
    <>
      <Helmet>
        <title> Dashboard </title>
      </Helmet>
      <section className="px-8">
        <StatDashboard />
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 pb-16">
          <SalesOverviewChart />
          <CategoryDistributionChart />
          <SalesChannelChart />
        </div>
      </section>
    </>
  );
};

export default Dashboard;