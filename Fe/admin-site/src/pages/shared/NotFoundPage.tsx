import { Button } from "@/components/ui/button";
import React from "react";
import { useNavigate } from "react-router-dom";
import NotFoundImage from "../../assets/images/not-found.webp";

const NotFoundPage: React.FC = () => {
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate("/");
  };

  return (
    <div className="flex flex-col justify-center items-center h-screen text-center">
      <div className="flex justify-center items-center">
        <img src={NotFoundImage} alt="" className="w-full" />
      </div>
      <p className="text-xl text-gray-500">
        Liên kết này không tồn tại. Vui lòng thử lại sau!
      </p>
      <div className="flex justify-center items-center mt-4 space-x-4">
        <Button variant="outline" onClick={() => navigate(-1)}>
          Quay lại
        </Button>
        <Button
          onClick={handleGoHome}
          className="border border-gray-800 bg-transparent hover:bg-transparent text-gray-800"
        >
          Trang chủ
        </Button>
      </div>
    </div>
  );
};

export default NotFoundPage;
