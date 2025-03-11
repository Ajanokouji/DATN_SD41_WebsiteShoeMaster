import React, { useEffect, useState } from "react";
import Sidebar from "./components/Sidebar";
import Header from "./components/Header";
import navConfig from "./nav-config";
import { useLocation } from "react-router-dom";

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const location = useLocation();
  const [title, setTitle] = useState("Dashboard");

  useEffect(() => {
    const currentNav = navConfig.find((nav) => nav.path === location.pathname);
    if (currentNav) {
      setTitle(currentNav.title);
    }
  }, [location.pathname]);

  return (
    <div className="flex min-h-screen w-screen overflow-x-hidden">
      <Sidebar />
      <main className="flex-grow">
        <Header title={title}/>
        <div className="pt-8">{children}</div>
      </main>
    </div>
  );
};

export default Layout;
