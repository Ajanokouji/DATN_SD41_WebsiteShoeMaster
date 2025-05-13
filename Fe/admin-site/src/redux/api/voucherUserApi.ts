import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";
import VoucherUserReqDto, { VoucherUserFilterParams, VoucherUserResDto, UserFilterParamsForSearch, UserResDtoForSearch } from "@/types/voucherUser/voucherUser";

class VoucherUserService {
  private static instance: VoucherUserService;

  private readonly endpoints = {
    voucherUser: "/VoucherUsers",
  };

  private constructor() {
    this.getVoucherUserById = this.getVoucherUserById.bind(this);
    this.getVoucherUsers = this.getVoucherUsers.bind(this);
    this.createVoucherUsers = this.createVoucherUsers.bind(this);
    this.updateVoucherUser = this.updateVoucherUser.bind(this);
    this.deleteVoucherUser = this.deleteVoucherUser.bind(this);
    this.searchUser = this.searchUser.bind(this);
  }

  static getInstance(): VoucherUserService {
    if (!VoucherUserService.instance) {
      VoucherUserService.instance = new VoucherUserService();
    }
    return VoucherUserService.instance;
  }

  async getVoucherUserById(id: string): Promise<VoucherUserResDto> {
    try {
      const response = await httpClient.get<{ data: VoucherUserResDto }>(
        `${this.endpoints.voucherUser}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get VoucherUser by ID error:", error);
      throw new Error(`Get VoucherUser by ID failed: ${error}`);
    }
  }

  async getVoucherUsers(
    params: VoucherUserFilterParams
  ): Promise<PaginatedResponse<VoucherUserResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<VoucherUserResDto>>(
        `${this.endpoints.voucherUser}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch VoucherUser error:", error);
      throw new Error(`Fetch VoucherUser failed: ${error}`);
    }
  }

  async createVoucherUsers(voucherUsers: VoucherUserReqDto[]): Promise<VoucherUserResDto[]> {
    try {
      const response = await httpClient.post<{ data: VoucherUserResDto[] }>(
        `${this.endpoints.voucherUser}`,
        voucherUsers
      );
      return response.data;
    } catch (error) {
      console.log("Create VoucherUser error:", error);
      throw new Error(`Create VoucherUser failed: ${error}`);
    }
  }

  async updateVoucherUser(id: string, formData: Partial<VoucherUserReqDto>): Promise<VoucherUserResDto> {
    try {
      const response = await httpClient.patch<{ data: VoucherUserResDto }>(
        `${this.endpoints.voucherUser}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update VoucherUser error:", error);
      throw new Error(`Update VoucherUser failed: ${error}`);
    }
  }

  async deleteVoucherUser(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.voucherUser}/${id}`);
    } catch (error) {
      console.log("Delete VoucherUser error:", error);
      throw new Error(`Delete VoucherUser failed: ${error}`);
    }
  }

  async searchUser(queryModel: UserFilterParamsForSearch): Promise<PaginatedResponse<UserResDtoForSearch>> {
    try {
      const response = await httpClient.post<PaginatedResponse<UserResDtoForSearch>>(
        `${this.endpoints.voucherUser}/SearchUser`,
        queryModel
      );
      return response;
    } catch (error) {
      console.log("Search user error:", error);
      throw new Error(`Search user failed: ${error}`);
    }
  }

  async getUsersByIds(ids: string[]): Promise<UserResDtoForSearch[]> {
    try {
      const response = await httpClient.post<UserResDtoForSearch[]>(
        `${this.endpoints.voucherUser}/GetUsersByIds`,
        ids
      );
      return response;
    } catch (error) {
      console.log("Get users by IDs error:", error);
      throw new Error(`Get users by IDs failed: ${error}`);
    }
  }

  async findVoucherUsersByVidUid(
    voucherUsers: VoucherUserReqDto[]
  ): Promise<VoucherUserResDto[]> {
    try {
      const response = await httpClient.post<{ data: VoucherUserResDto[] }>(
        `${this.endpoints.voucherUser}/FindByVidUid`,
        voucherUsers
      );
      return response.data;
    } catch (error) {
      console.log("Find VoucherUsers by VidUid error:", error);
      throw new Error(`Find VoucherUsers by VidUid failed: ${error}`);
    }
  }
}

const voucherUserService = VoucherUserService.getInstance();
export default voucherUserService;