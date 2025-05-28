import React, { useEffect, useRef, useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { Input } from "@/components/ui/input";
import Pagination from "@/components/Pagination";
import {
  searchUser,
  createVoucherUsers,
  findVoucherUsersByVidUid,
  setUserPage,
  setUserPageSize,
  clearUsers,
} from "@/redux/apps/voucherUser/voucherUserSlice";
import {
  selectUsers,
  selectUserPagination,
} from "@/redux/apps/voucherUser/voucherUserSelector";
import { UserResDtoForSearch, VoucherUserResDto } from "@/types/voucherUser/voucherUser";
import VoucherUserReqDto from "@/types/voucherUser/voucherUser";
import { Button } from "@/components/ui/button";
import { FaAddressCard, FaEnvelope, FaPhone } from "react-icons/fa6";
import { FaHome } from "react-icons/fa";
import { fetchVoucherById } from "@/redux/apps/voucher/voucherSlice";

interface AddVoucherUserSheetProps {
  isOpen: boolean;
  onClose: () => void;
  voucherId: string;
  setValue: (name: string, value: any) => void;
}

const AddVoucherUserSheet: React.FC<AddVoucherUserSheetProps> = ({
  isOpen,
  onClose,
  voucherId,
  setValue,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errMs, setErrMs] = useState("");
  const [inputSearchUserString, setInputSearchUserString] = useState<string>("");
  const [isSearchingUser, setIsSearchingUser] = useState<boolean>(false);
  const [selectedUsers, setSelectedUsers] = useState<UserResDtoForSearch[]>([]);

  const users = useAppSelector(selectUsers);
  const pagination = useAppSelector(selectUserPagination);

  const errorRef = useRef<HTMLHeadingElement>(null);

  useEffect(() => {
    if (isOpen) {
      dispatch(clearUsers()); // Xóa danh sách products khi form được mở
    }
  }, [isOpen, dispatch]);

  useEffect(() => {
    if (errMs && errorRef.current) {
      errorRef.current.scrollIntoView({ behavior: "smooth", block: "center" });
    }
  }, [errMs]);

  const [existingVoucherUsers, setExistingVoucherUsers] = useState<VoucherUserResDto[]>([]);
  const handleSubmitSearchUser = async () => {
    setIsSearchingUser(true);
    try {
      // Gọi API tìm kiếm người dùng
      const searchResponse = await dispatch(
        searchUser({
          userName: inputSearchUserString,
          CurrentPage: 1,
          PageSize: pagination.pageSize,
        })
      ).unwrap();

      // Map danh sách tìm kiếm thành VoucherUserReqDto
      const voucherUsersToCheck: VoucherUserReqDto[] = searchResponse.data.content.map((user) => ({
        voucherId,
        userId: user.id,
        createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedOnDate: new Date().toISOString(),
        createdOnDate: new Date().toISOString(),
        isUsed: false,
      }));

      // Gọi API kiểm tra danh sách trong cơ sở dữ liệu
      const existingUsers = await dispatch(
        findVoucherUsersByVidUid(voucherUsersToCheck)
      ).unwrap();

      setExistingVoucherUsers(existingUsers); // Lưu danh sách đã tồn tại
    } catch (error) {
      console.error("Search user failed:", error);
    } finally {
      setIsSearchingUser(false);
    }
  };

  const isUserSelected = (user: UserResDtoForSearch) => {
    return (
      existingVoucherUsers.some(
        (voucherUser) => voucherUser.userId === user.id
      ) ||
      selectedUsers.some((selectedUser) => selectedUser.id === user.id)
    );
  };

  const handlePageChange = async (newPage: number) => {
    dispatch(setUserPage(newPage));
    try {
      await dispatch(
        searchUser({
          userName: inputSearchUserString,
          CurrentPage: newPage,
          PageSize: pagination.pageSize,
        })
      ).unwrap();
    } catch (error) {
      console.error("Search user failed:", error);
    }
  };

  const handlePageSizeChange = async (newSize: number) => {
    dispatch(setUserPageSize(newSize));
    try {
      await dispatch(
        searchUser({
          userName: inputSearchUserString,
          CurrentPage: 1,
          PageSize: newSize,
        })
      ).unwrap();
    } catch (error) {
      console.error("Search user failed:", error);
    }
  };

  const addUserToSelectedList = (user: UserResDtoForSearch) => {
    setSelectedUsers((prev) => {
      const isUserAlreadySelected = prev.some(
        (selectedUser) => selectedUser.id === user.id
      );
      if (isUserAlreadySelected) return prev;

      const updatedUsers = [user, ...prev];
      setValue("usersIsSelected", updatedUsers);
      return updatedUsers;
    });
  };

  const removeUserFromSelectedList = (userId: string) => {
    setSelectedUsers((prev) => {
      const updatedUsers = prev.filter((user) => user.id !== userId);
      setValue("usersIsSelected", updatedUsers);
      return updatedUsers;
    });
  };

  const clearAllSelectedUsers = () => {
    setSelectedUsers([]);
    setValue("usersIsSelected", []);
  };

  const handleFormSubmit = async () => {
    setIsSubmitting(true);
    try {
      if (selectedUsers.length === 0) {
        setErrMs("Vui lòng chọn ít nhất một khách hàng.");
        return;
      }

      //Check voucher tồn tại
      const voucherResponse = await dispatch(fetchVoucherById(voucherId)).unwrap();
      if ((!voucherResponse) || (voucherResponse && voucherResponse.isDeleted)) {
        setErrMs(""); // Đặt lại về rỗng để kích hoạt useEffect
        setTimeout(() => {
          setErrMs("Không tìm thấy Voucher! Vui lòng kiểm tra lại!");
        }, 0); // Đặt lỗi mới sau một khoảng thời gian ngắn
        return;
      }
      setErrMs("");

      const voucherUsers: VoucherUserReqDto[] = selectedUsers.map((user) => ({
        voucherId,
        userId: user.id,
        createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedOnDate: new Date().toISOString(),
        createdOnDate: new Date().toISOString(),
        isUsed: false,
      }));

      const existingUsers = await dispatch(
        findVoucherUsersByVidUid(voucherUsers)
      ).unwrap();

      if (existingUsers.length > 0) {
        setErrMs(
          "Trong kho Voucher của một số khách hàng đã có Voucher này! Vui lòng kiểm tra lại!"
        );
        return;
      }

      await dispatch(createVoucherUsers(voucherUsers)).unwrap();
      onClose();
    } catch (error) {
      console.error("Error adding voucher users:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle>Thêm vào kho Voucher của khách hàng</SheetTitle>
          <SheetDescription></SheetDescription>
        </SheetHeader>
        <h3 ref={errorRef} className="text-red-500 text-center">
          {errMs}
        </h3>
        <div className="p-5 m-5 border border-gray-300 rounded">
          <div className="flex items-center">
            <Input
              placeholder="Tìm kiếm khách hàng dựa trên tên tài khoản, tên khách hàng, số điện thoại, email, địa chỉ"
              value={inputSearchUserString}
              onChange={(e) => setInputSearchUserString(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  handleSubmitSearchUser();
                }
              }}
              className="flex-1"
            />
            <button
              type="button"
              onClick={handleSubmitSearchUser}
              className={`p-2 ml-2 text-sm w-32 rounded font-semibold ${
                isSearchingUser
                  ? "bg-gray-400 cursor-not-allowed"
                  : "bg-blue-500 hover:bg-blue-600 text-white"
              }`}
              disabled={isSearchingUser}
            >
              {isSearchingUser ? "Đang tìm..." : "Tìm kiếm"}
            </button>
          </div>
          <div className="flex mt-4">
            <div className="w-1/2 h-[60vh] border rounded p-2">
              <h4 className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">Danh sách khách hàng</h4>
              {users && users.length > 0 ? (
                <ul className="space-y-2 max-h-[50vh] overflow-y-auto">
                  {users.map((user) => {
                    // Xác định trạng thái và lớp CSS
                    const isActive = user?.isActive;
                    let borderColor = "border-gray-400"; // Mặc định là xám
                    let statusText = "UNDEFINED";
                    let statusBgColor = "bg-gray-400";

                    if (isActive === true) {
                      borderColor = "border-green-500";
                      statusText = "ACTIVE";
                      statusBgColor = "bg-green-500";
                    } else if (isActive === false) {
                      borderColor = "border-red-500";
                      statusText = "INACTIVE";
                      statusBgColor = "bg-red-500";
                    }

                    return (  
                      <li
                        key={user.id}
                        className="p-4 border rounded shadow hover:bg-gray-100 relative"
                      >
                        <button
                          onClick={() => addUserToSelectedList(user)}
                          type="button"
                          className={`absolute top-2 right-2 border rounded p-1 ${
                            isUserSelected(user)
                              ? "bg-gray-400 cursor-not-allowed"
                              : "bg-blue-500 text-white hover:bg-blue-600"
                          } text-xs`}
                          disabled={isUserSelected(user)} // Disable nút nếu user đã được thêm
                        >
                          +
                        </button>
                        <div className="flex items-center">
                          <div className="relative">
                            <img
                              className={`object-cover w-16 h-16 border-2 rounded-full mr-5 ${borderColor}`}
                              src={user.avartarUrl || "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg"}
                              alt={user.name || "User"}
                            />
                            {/* Hiển thị trạng thái */}
                            <span
                              className={`absolute px-1 bottom-0 left-1/2 transform -translate-x-1/2 text-xs font-semibold text-white rounded-full ${statusBgColor}`}
                            >
                              {statusText}
                            </span>
                          </div>
                          
                          <div className="flex-1">
                            <p className="flex text-start text-sm font-semibold">
                              <FaAddressCard className="mr-2 mt-1"/>{user.name} {user.username?"- " + user.username:""}
                            </p>
                            <p className="flex text-start text-sm text-gray-500">
                              <FaPhone className="mr-2 mt-1"/>{user.phoneNumber}
                            </p>
                            <p className="flex text-start text-sm text-gray-500">
                              <FaEnvelope className="mr-2 mt-1"/>{user.email}
                            </p>
                            <p className="flex text-start text-sm text-gray-500">
                              <FaHome className="mr-2 mt-1"/>{user.address}
                            </p>
                          </div>
                        </div>
                      </li>
                      );
                  })}
                </ul>
              ) : (
                <p className="text-gray-500">Không tìm thấy khách hàng nào.</p>
              )}
            </div>
            <div className="w-1/2 h-[60vh] border rounded p-2 ml-4">
              <h4 className="text-center font-semibold mb-4 bg-gray-200 rounded p-1">Đã chọn</h4>
              {selectedUsers.length > 0 ? (
                <button
                  onClick={clearAllSelectedUsers}
                  type="button"
                  className="w-full p-1 bg-red-500 text-white rounded hover:bg-red-600 mb-2"
                >
                  Xóa tất cả
                </button>
              ) : ""}
              {selectedUsers.length > 0 ? (
                <ul className="space-y-2 max-h-[44vh] overflow-y-auto">
                  {selectedUsers.map((user) => {
                    // Xác định trạng thái và lớp CSS
                    const isActive = user?.isActive;
                    let borderColor = "border-gray-400"; // Mặc định là xám
                    let statusText = "UNDEFINED";
                    let statusBgColor = "bg-gray-400";

                    if (isActive === true) {
                      borderColor = "border-green-500";
                      statusText = "ACTIVE";
                      statusBgColor = "bg-green-500";
                    } else if (isActive === false) {
                      borderColor = "border-red-500";
                      statusText = "INACTIVE";
                      statusBgColor = "bg-red-500";
                    }

                    return (
                      <li
                        key={user.id}
                        className="p-4 border rounded shadow hover:bg-gray-100 relative"
                      >
                        <button
                          onClick={() => removeUserFromSelectedList(user.id)}
                          type="button"
                          className="absolute top-2 right-2 border rounded p-1 bg-red-500 text-white hover:bg-red-600 text-xs"
                        >
                          X
                        </button>
                        <div className="flex items-center">
                          <div className="relative">
                            <img
                              className={`object-cover w-16 h-16 border-2 rounded-full mr-5 ${borderColor}`}
                              src={user.avartarUrl || "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg"}
                              alt={user.name || "User"}
                            />
                            {/* Hiển thị trạng thái */}
                            <span
                              className={`absolute px-1 bottom-0 left-1/2 transform -translate-x-1/2 text-xs font-semibold text-white rounded-full ${statusBgColor}`}
                            >
                              {statusText}
                            </span>
                          </div>
                          <div className="flex-1">
                            <p className="flex text-start text-sm font-semibold">
                              <FaAddressCard className="mr-2 mt-1"/>{user.name} {user.username?"- " + user.username:""}
                            </p>
                            <p className="flex text-start text-sm text-gray-500">
                              <FaPhone className="mr-2 mt-1"/>{user.phoneNumber}
                            </p>
                            <p className="flex text-start text-sm text-gray-500">
                              <FaEnvelope className="mr-2 mt-1"/>{user.email}
                            </p>
                            <p className="flex text-start text-sm text-gray-500">
                              <FaHome className="mr-2 mt-1"/>{user.address}
                            </p>
                          </div>
                        </div>
                      </li>
                    );
                  })}
                </ul>
              ) : (
                <p className="text-gray-500">Chưa có khách hàng nào được chọn.</p>
              )}
            </div>
          </div>
        </div>
        <Pagination
          currentPage={pagination.currentPage}
          totalPages={pagination.totalPages}
          pageSize={pagination.pageSize}
          totalRecords={pagination.totalRecords}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
        />
        <div className="flex justify-end gap-2 pt-4">
          <Button onClick={handleFormSubmit} disabled={isSubmitting}>
            {isSubmitting ? "Đang thêm..." : "Thêm"}
          </Button>
          <Button variant="outline" onClick={onClose}>
            Thoát
          </Button>
        </div>
      </SheetContent>
    </Sheet>
  );
};

export default AddVoucherUserSheet;