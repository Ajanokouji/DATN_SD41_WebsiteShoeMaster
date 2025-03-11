import CategoryReqDto, { Product } from "@/types/category/category";
import httpClient from "./agent";
import { PaginatedResponse, PaginationParams } from "@/types/common/pagination";

class CategoryService {
  private static instance: CategoryService;

  private readonly endpoints = {
    fetchCategories: "/Product/filter",
    createCategory: "/Product",
    deleteCategory: "/Product",
  };

  private constructor() {
    this.getProducts = this.getProducts.bind(this);
    this.createCategoryReq = this.createCategoryReq.bind(this);
    this.deleteProductReq = this.deleteProductReq.bind(this);
  }

  static getInstance(): CategoryService {
    if (!CategoryService.instance) {
      CategoryService.instance = new CategoryService();
    }
    return CategoryService.instance;
  }

  async getProducts(
    params: PaginationParams
  ): Promise<PaginatedResponse<Product>> {
    try {
      const response = await httpClient.post<PaginatedResponse<Product>>(
        `${this.endpoints.fetchCategories}`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch products error:", error);
      throw new Error(`Fetch products failed: ${error}`);
    }
  }

  async createCategoryReq(userData: CategoryReqDto): Promise<CategoryReqDto> {
    try {
      const response = await httpClient.post<CategoryReqDto>(
        this.endpoints.createCategory,
        userData
      );
      return response;
    } catch (error) {
      console.log("Create category error:", error);
      throw new Error(`Create category failed: ${error}`);
    }
  }

  async deleteProductReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.deleteCategory}/${id}`);
    } catch (error) {
      console.log("Delete category error:", error);
      throw new Error(`Delete category failed: ${error}`);
    }
  }
}

const categoryService = CategoryService.getInstance();
export default categoryService;
