// src/redux/api/userService.ts
import UserReqDto, {
  UserResDto,
  UserFilterParams,
} from "@/types/user/user";
import httpClient from "./agent"; // cùng client với product
import { PaginatedResponse } from "@/types/common/pagination";

class UserService {
  private static instance: UserService;

  /** Tất cả endpoint gói trong 1 object để dễ đổi */
  private readonly endpoints = {
    user: "/User",       // => POST /Users/filter, GET /Users/:id, ...
  };

  /* ----- Singleton pattern ----- */
  private constructor() {
    this.getUsers = this.getUsers.bind(this);
    this.getUserById = this.getUserById.bind(this);
    this.createUserReq = this.createUserReq.bind(this);
    this.updateUserReq = this.updateUserReq.bind(this);
    this.deleteUserReq = this.deleteUserReq.bind(this);
    this.checkTrungCodeUser = this.checkTrungCodeUser.bind(this);
  }
  static getInstance(): UserService {
    if (!UserService.instance) UserService.instance = new UserService();
    return UserService.instance;
  }

  /* -------- API methods ---------- */

  /** Lấy danh sách user có filter + phân trang */
  async getUsers(
    params: UserFilterParams
  ): Promise<PaginatedResponse<UserResDto>> {
    try {
      // giống Product: POST /Users/filter với params query
      return await httpClient.post<PaginatedResponse<UserResDto>>(
        `${this.endpoints.user}/filter`,
        {},
        { params }
      );
    } catch (error) {
      console.log("Fetch users error:", error);
      throw new Error(`Fetch users failed: ${error}`);
    }
  }

  /** Lấy chi tiết user theo ID */
  async getUserById(id: string): Promise<UserResDto> {
    try {
      const response = await httpClient.get<{ data: UserResDto }>(
        `${this.endpoints.user}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get user by ID error:", error);
      throw new Error(`Get user by ID failed: ${error}`);
    }
  }

  /** Tạo mới user */
  async createUserReq(userData: UserReqDto): Promise<UserResDto> {
    try {
      const response = await httpClient.post<{ data: UserResDto }>(
        this.endpoints.user,
        userData
      );
      return response.data;
    } catch (error) {
      console.log("Create user error:", error);
      throw new Error(`Create user failed: ${error}`);
    }
  }

  /** Cập nhật user (patch/put tùy BE) */
  async updateUserReq(
    id: string,
    userData: Partial<UserReqDto>
  ): Promise<UserResDto> {
    try {
      const response = await httpClient.patch<{ data: UserResDto }>(
        `${this.endpoints.user}/${id}`,
        userData
      );
      return response.data;
    } catch (error) {
      console.log("Update user error:", error);
      throw new Error(`Update user failed: ${error}`);
    }
  }

  /** Xoá user */
  async deleteUserReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.user}/${id}`);
    } catch (error) {
      console.log("Delete user error:", error);
      throw new Error(`Delete user failed: ${error}`);
    }
  }

  /** Kiểm tra trùng username (code user) */
  async checkTrungCodeUser(username: string, id?: string): Promise<boolean> {
    try {
      const response = await httpClient.post<boolean>(
        `${this.endpoints.user}/CheckTrungCodeUser`,
        JSON.stringify(username), // gửi raw string, nhớ stringify để backend nhận đúng
        {
          params: { id },
          headers: { "Content-Type": "application/json" }
        }
      );
      return response; // response là true/false
    } catch (error) {
      console.log("Check trung code user error:", error);
      throw new Error(`Check trung code user failed: ${error}`);
    }
  }
}


const userService = UserService.getInstance();
export default userService;
