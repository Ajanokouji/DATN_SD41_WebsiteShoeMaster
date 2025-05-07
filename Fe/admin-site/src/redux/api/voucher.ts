import VoucherReqDto, { VoucherFilterParams, VoucherResDto } from "@/types/voucher/voucher";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

class VoucherService {
  private static instance: VoucherService;

  private readonly endpoints = {
    vouchers: "/Voucher",
  };

  private constructor() {
    this.getVouchers = this.getVouchers.bind(this);
    this.getVoucherById = this.getVoucherById.bind(this);
    this.createVoucherReq = this.createVoucherReq.bind(this);
    this.deleteVoucherReq = this.deleteVoucherReq.bind(this);
    this.updateVoucherReq = this.updateVoucherReq.bind(this);
  }

  static getInstance(): VoucherService {
    if (!VoucherService.instance) {
      VoucherService.instance = new VoucherService();
    }
    return VoucherService.instance;
  }

  async getVouchers(
    params: VoucherFilterParams
  ): Promise<PaginatedResponse<VoucherResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<VoucherResDto>>(
        `${this.endpoints.vouchers}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch vouchers error:", error);
      throw new Error(`Fetch vouchers failed: ${error}`);
    }
  }

  async getVoucherById(id: string): Promise<VoucherResDto> {
    try {
      const response = await httpClient.get<{ data: VoucherResDto }>(
        `${this.endpoints.vouchers}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get voucher by ID error:", error);
      throw new Error(`Get voucher by ID failed: ${error}`);
    }
  }
  

  async createVoucherReq(formData: VoucherReqDto): Promise<VoucherResDto> {
    try {
      const response = await httpClient.post<{ data: VoucherResDto }>(
        this.endpoints.vouchers,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Create voucher error:", error);
      throw new Error(`Create voucher failed: ${error}`);
    }
  }

  async updateVoucherReq(id: string, formData: Partial<VoucherReqDto>): Promise<VoucherResDto> {
    try {
      const response = await httpClient.patch<{ data: VoucherResDto }>(
        `${this.endpoints.vouchers}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update voucher error:", error);
      throw new Error(`Update voucher failed: ${error}`);
    }
  }
  

  async deleteVoucherReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.vouchers}/${id}`);
    } catch (error) {
      console.log("Delete voucher error:", error);
      throw new Error(`Delete voucher failed: ${error}`);
    }
  }

  async checkVoucherCodeExist(code: string, voucherId?: string): Promise<boolean> {
    try {
      const response = await httpClient.post<boolean>(
        `${this.endpoints.vouchers}/IsCodeExist`,
        JSON.stringify(code), // Gửi chuỗi JSON thuần
        {
          headers: { 'Content-Type': 'application/json' },
          params: { voucherId },
        }
      );
      return response;
    } catch (error) {
      console.log("Check voucher code exist error:", error);
      throw new Error(`Check voucher code exist failed: ${error}`);
    }
  }
}

const voucherService = VoucherService.getInstance();
export default voucherService;
