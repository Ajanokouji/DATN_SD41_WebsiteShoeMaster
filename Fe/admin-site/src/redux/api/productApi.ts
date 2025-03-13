import ProductReqDto, { ProductResDto } from "@/types/product/product";
import httpClient from "./agent";
import { PaginatedResponse, PaginationParams } from "@/types/common/pagination";

class ProductService {
  private static instance: ProductService;

  private readonly endpoints = {
    fetchProducts: "/Product/filter",
    createProduct: "/Product",
    deleteProduct: "/Product",
  };

  private constructor() {
    this.getProducts = this.getProducts.bind(this);
    this.createProductReq = this.createProductReq.bind(this);
    this.deleteProductReq = this.deleteProductReq.bind(this);
  }

  static getInstance(): ProductService {
    if (!ProductService.instance) {
      ProductService.instance = new ProductService();
    }
    return ProductService.instance;
  }

  async getProducts(
    params: PaginationParams
  ): Promise<PaginatedResponse<ProductResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<ProductResDto>>(
        `${this.endpoints.fetchProducts}`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch products error:", error);
      throw new Error(`Fetch products failed: ${error}`);
    }
  }

  async createProductReq(userData: ProductReqDto): Promise<ProductReqDto> {
    try {
      const response = await httpClient.post<ProductReqDto>(
        this.endpoints.createProduct,
        userData
      );
      return response;
    } catch (error) {
      console.log("Create product error:", error);
      throw new Error(`Create product failed: ${error}`);
    }
  }

  async deleteProductReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.deleteProduct}/${id}`);
    } catch (error) {
      console.log("Delete product error:", error);
      throw new Error(`Delete product failed: ${error}`);
    }
  }
}

const productService = ProductService.getInstance();
export default productService;
