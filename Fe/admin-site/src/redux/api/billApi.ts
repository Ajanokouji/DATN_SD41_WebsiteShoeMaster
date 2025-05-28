import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";
import { BillDetailResDto, BillFilterParams, BillReqDto, BillResDto, InventoryCheckResponse } from "@/types/bill/bill";

class BillService {
  private static instance: BillService;

  private readonly endpoints = {
    bills: "/Bill",
    checkInventory: "/Bill/check-inventory"
  };

  private constructor() {
    this.getBills = this.getBills.bind(this);
    this.getBillById = this.getBillById.bind(this);
    this.createBillReq = this.createBillReq.bind(this);
    this.updateBillReq = this.updateBillReq.bind(this);
    this.deleteBillReq = this.deleteBillReq.bind(this);
  }

  static getInstance(): BillService {
    if (!BillService.instance) {
      BillService.instance = new BillService();
    }
    return BillService.instance;
  }

  // 🟢 Lấy danh sách hóa đơn có phân trang
  async getBills(
    params: BillFilterParams
  ): Promise<PaginatedResponse<BillResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<BillResDto>>(
        `${this.endpoints.bills}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.error("Fetch bills error:", error);
      throw new Error(`Fetch bills failed: ${error}`);
    }
  }

  // 🔵 Lấy hóa đơn theo ID
  async getBillById(id: string): Promise<BillDetailResDto> {
    try {
      const response = await httpClient.get<{ data: BillDetailResDto }>(
        `${this.endpoints.bills}/${id}`
      );
      return response.data;
    } catch (error) {
      console.error("Get bill by ID error:", error);
      throw new Error(`Get bill by ID failed: ${error}`);
    }
  }

  // 🟠 Tạo mới hóa đơn
  async createBillReq(formData: BillReqDto): Promise<BillResDto> {
    try {
      const response = await httpClient.post<{ data: BillResDto }>(
        this.endpoints.bills,
        formData
      );
      return response.data;
    } catch (error) {
      console.error("Create bill error:", error);
      throw new Error(`Create bill failed: ${error}`);
    }
  }

  // 🟡 Cập nhật hóa đơn
  async updateBillReq(
    id: string,
    formData: Partial<BillReqDto>
  ): Promise<BillDetailResDto> {
    try {
      const response = await httpClient.patch<{ data: BillDetailResDto }>(
        `${this.endpoints.bills}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.error("Update bill error:", error);
      throw new Error(`Update bill failed: ${error}`);
    }
  }

  async patchBillReq(
    id: string,
    formData: Partial<BillReqDto>
  ): Promise<BillDetailResDto> {
    try {
      const response = await httpClient.patch<{ data: BillDetailResDto }>(
        `${this.endpoints.bills}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.error("Patch bill error:", error);
      throw new Error(`Patch bill failed: ${error}`);
    }
  }

  // 🔴 Xóa hóa đơn theo ID
  async deleteBillReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.bills}/${id}`);
    } catch (error) {
      console.error("Delete bill error:", error);
      throw new Error(`Delete bill failed: ${error}`);
    }
  }

  // Kiểm tra số lượng trong kho
  async checkInventory(id: string): Promise<InventoryCheckResponse> {
    try {
      const response = await httpClient.get<{ data: InventoryCheckResponse }>(
        `${this.endpoints.checkInventory}/${id}`
      );
      return response.data;
    } catch (error) {
      console.error("Check inventory error:", error);
      throw new Error(`Check inventory failed: ${error}`);
    }
  }
}

const billService = BillService.getInstance();
export default billService;
