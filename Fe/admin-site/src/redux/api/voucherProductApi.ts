import VoucherProductReqDto, { VoucherProductFilterParams, VoucherProductResDto } from "@/types/voucherProduct/voucherProduct";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";
import { ProductDetailResDto, ProductFilterParams } from "@/types/product/product";

class VoucherProductService {
  private static instance: VoucherProductService;

  private readonly endpoints = {
    voucherProduct: "/VoucherProducts",
  };

  private constructor() {
    this.getVoucherProducts = this.getVoucherProducts.bind(this);
    this.getVoucherProductById = this.getVoucherProductById.bind(this);
    this.createVoucherProductReq = this.createVoucherProductReq.bind(this);
    this.deleteVoucherProductReq = this.deleteVoucherProductReq.bind(this);
    this.updateVoucherProductReq = this.updateVoucherProductReq.bind(this);
    this.getProductsByIds = this.getProductsByIds.bind(this);
  }

  static getInstance(): VoucherProductService {
    if (!VoucherProductService.instance) {
        VoucherProductService.instance = new VoucherProductService();
    }
    return VoucherProductService.instance;
  }

  async getVoucherProducts(
    params: VoucherProductFilterParams
  ): Promise<PaginatedResponse<VoucherProductResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<VoucherProductResDto>>(
        `${this.endpoints.voucherProduct}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch voucherProducts error:", error);
      throw new Error(`Fetch voucherProducts failed: ${error}`);
    }
  }

  async getVoucherProductById(id: string): Promise<VoucherProductResDto> {
    try {
      const response = await httpClient.get<{ data: VoucherProductResDto }>(
        `${this.endpoints.voucherProduct}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get voucherProduct by ID error:", error);
      throw new Error(`Get voucherProduct by ID failed: ${error}`);
    }
  }
  

  async createVoucherProductReq(voucherProducts: VoucherProductReqDto[]): Promise<VoucherProductResDto[]> {
    try {
      const response = await httpClient.post<{ data: VoucherProductResDto[] }>(
        `${this.endpoints.voucherProduct}`,
        voucherProducts
      );
      return response.data;
    } catch (error) {
      console.log("Create voucherProducts error:", error);
      throw new Error(`Create voucherProducts failed: ${error}`);
    }
  }

  async updateVoucherProductReq(id: string, formData: Partial<VoucherProductReqDto>): Promise<VoucherProductResDto> {
    try {
      const response = await httpClient.patch<{ data: VoucherProductResDto }>(
        `${this.endpoints.voucherProduct}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update voucherProduct error:", error);
      throw new Error(`Update voucherProduct failed: ${error}`);
    }
  }
  
  async deleteVoucherProductReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.voucherProduct}/${id}`);
    } catch (error) {
      console.log("Delete voucherProduct error:", error);
      throw new Error(`Delete voucherProduct failed: ${error}`);
    }
  }

  async getProductsByIds(ids: string[]): Promise<ProductDetailResDto[]> {
    try {
      const response = await httpClient.post<ProductDetailResDto[]>(
        `${this.endpoints.voucherProduct}/GetProductsByIds`,
        ids
      );
      return response;
    } catch (error) {
      console.log("Get products by IDs error:", error);
      throw new Error(`Get products by IDs failed: ${error}`);
    }
  }

  async searchProduct(queryModel: ProductFilterParams): Promise<PaginatedResponse<ProductDetailResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<ProductDetailResDto>>(
        `${this.endpoints.voucherProduct}/SearchProduct`,
        queryModel
      );
      return response;
    } catch (error) {
      console.log("Search product error:", error);
      throw new Error(`Search product failed: ${error}`);
    }
  }

  async findVoucherProductsByVcidPidVaid(
    voucherProducts: VoucherProductReqDto[]
  ): Promise<VoucherProductResDto[]> {
    try {
      const response = await httpClient.post<{ data: VoucherProductResDto[] }>(
        `${this.endpoints.voucherProduct}/FindByVcidPidVaid`,
        voucherProducts
      );
      return response.data;
    } catch (error) {
      console.log("Find voucherProducts by VcidPidVaid error:", error);
      throw new Error(`Find voucherProducts by VcidPidVaid failed: ${error}`);
    }
  }
}
const voucherProductService = VoucherProductService.getInstance();
export default voucherProductService;