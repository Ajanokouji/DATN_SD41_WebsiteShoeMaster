import React, { useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { login } from "@/redux/apps/login/loginSlice";
import { selectLoginLoading, selectLoginError } from "@/redux/apps/login/loginSelector";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Eye, EyeOff } from "lucide-react";

const LoginPage: React.FC = () => {
  const dispatch = useAppDispatch();
  const loading = useAppSelector(selectLoginLoading);
  const error = useAppSelector(selectLoginError);
  const navigate = useNavigate();

  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError(null);

    if (!userName || !password) {
      setFormError("Vui lòng nhập đầy đủ tài khoản và mật khẩu.");
      return;
    }

    try {
      const resultAction = await dispatch(login({ userName, password })).unwrap();
      if (resultAction) {
        navigate("/");
      }
    } catch (err) {
      // error đã được xử lý ở slice
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-100 to-blue-100">
      <form
        onSubmit={handleSubmit}
        className="bg-white border border-gray-200 rounded-xl shadow-md w-full max-w-lg px-10 py-8"
      >
        <h2 className="text-2xl font-bold mb-8 text-center">
          Đăng nhập Admin
        </h2>
        {formError && (
          <div className="mb-3 text-red-500 text-center font-medium">{formError}</div>
        )}
        {error && (
          <div className="mb-3 text-red-500 text-center font-medium">{error}</div>
        )}
        <div className="mb-6">
          <label className="block mb-2 font-semibold text-gray-700">Tên đăng nhập</label>
          <Input
            type="text"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            placeholder="Nhập tên đăng nhập"
            autoFocus
            autoComplete="username"
          />
        </div>
        <div className="mb-8 relative">
          <label className="block mb-2 font-semibold text-gray-700">Mật khẩu</label>
          <Input
            type={showPassword ? "text" : "password"}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Nhập mật khẩu"
            autoComplete="current-password"
          />
          <button
            type="button"
            className="absolute right-3 top-9 text-gray-500 hover:text-blue-600"
            tabIndex={-1}
            onClick={() => setShowPassword((v) => !v)}
          >
            {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
          </button>
        </div>
        <Button
          type="submit"
          className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg py-2 transition"
          disabled={loading}
        >
          {loading ? "Đang đăng nhập..." : "Đăng nhập"}
        </Button>
      </form>
    </div>
  );
};

export default LoginPage;