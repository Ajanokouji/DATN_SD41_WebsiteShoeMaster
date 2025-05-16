import ProductReqDto, { ProductDetailResDto, ProductFilterParams, ProductResDto } from "@/types/product/product";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

class ProductService {
  private static instance: ProductService;

  private readonly endpoints = {
    product: "/Product",
  };

  private constructor() {
    this.getProducts = this.getProducts.bind(this);
    this.getProductById = this.getProductById.bind(this);
    this.createProductReq = this.createProductReq.bind(this);
    this.updateProductReq = this.updateProductReq.bind(this);
    this.deleteProductReq = this.deleteProductReq.bind(this);
  }

  static getInstance(): ProductService {
    if (!ProductService.instance) {
      ProductService.instance = new ProductService();
    }
    return ProductService.instance;
  }

  async getProducts(
    params: ProductFilterParams
  ): Promise<PaginatedResponse<ProductResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<ProductResDto>>(
        `${this.endpoints.product}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch products error:", error);
      throw new Error(`Fetch products failed: ${error}`);
    }
  }

  async getProductById(id: string): Promise<ProductDetailResDto> {
    try {
      const response = await httpClient.get<{ data: ProductDetailResDto }>(
        `${this.endpoints.product}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get product by ID error:", error);
      throw new Error(`Get product by ID failed: ${error}`);
    }
  }

  async createProductReq(userData: ProductReqDto): Promise<ProductResDto> {
    try {
          
      const response = await httpClient.post<{ data: ProductResDto }>(
        this.endpoints.product,
        userData
      );
      return response.data;
    } catch (error) {
      console.log("Create product error:", error);
      throw new Error(`Create product failed: ${error}`);
    }
  }

  async updateProductReq(id: string, formData: Partial<ProductReqDto>): Promise<ProductResDto> {
    try {
      const response = await httpClient.patch<{ data: ProductResDto }>(
        `${this.endpoints.product}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update product error:", error);
      throw new Error(`Update product failed: ${error}`);
    }
  }

  async deleteProductReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.product}/${id}`);
    } catch (error) {
      console.log("Delete product error:", error);
      throw new Error(`Delete product failed: ${error}`);
    }
  }
}

const productService = ProductService.getInstance();
export default productService;
