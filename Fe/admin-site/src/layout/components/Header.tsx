import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
// import { useAppDispatch } from "@/hooks/use-app-dispatch";
// import { useAppSelector } from "@/hooks/use-app-selector";
// import { logoutUser } from "@/redux/apps/auth/authSlice";
import { TbLayoutDashboard } from "react-icons/tb";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

interface HeaderProps {
  title: string;
}
const Header: React.FC<HeaderProps> = ({ title }) => {
  // const dispatch = useAppDispatch();
  const navigate = useNavigate();
  // const { isAuthenticated } = useAppSelector((state) => state.auth);
  const isAuthenticated = true;
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const handleLogout = () => {
    // dispatch(logoutUser());
    navigate("/login");
  };

  const toggleDropdown = () => {
    setIsDropdownOpen(!isDropdownOpen);
  };

  return (
    <section>
      <div className="mt-3 pb-3 px-8 border-b border-gray-200">
        <div className="grid grid-cols-2 justify-between">
          <div className="flex gap-2 items-center">
            <TbLayoutDashboard className="text-gray-700 text-3xl" />
            <p className="text-2xl font-semibold">{title}</p>
          </div>

          {isAuthenticated && (
            <div className="flex gap-5 justify-end">
              <div className="flex flex-col">
                <p className="flex justify-end text-sm font-semibold ">
                  Mrs Conan
                </p>
                <p className="flex justify-end text-sm font-semibold">
                  Nhân viên kinh doanh
                </p>
              </div>
              <div className="relative">
                <Avatar onClick={toggleDropdown}>
                  <AvatarImage
                    src="https://github.com/shadcn.png"
                    alt="@shadcn"
                  />
                  <AvatarFallback>MCI</AvatarFallback>
                </Avatar>
                {isDropdownOpen && (
                  <div className="absolute right-0 mt-2 w-48 bg-white shadow-lg rounded-md py-2 z-50">
                    <button
                      className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                      onClick={handleLogout}
                    >
                      Đăng xuất
                    </button>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    </section>
  );
};

export default Header;
