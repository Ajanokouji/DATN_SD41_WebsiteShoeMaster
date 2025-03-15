import CategoryReqDto, { CategoryDetailResDto, CategoryFilterParams, CategoryResDto } from "@/types/category/category";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

class CategoryService {
  private static instance: CategoryService;

  private readonly endpoints = {
    categories: "/Categories",
  };

  private constructor() {
    this.getCategories = this.getCategories.bind(this);
    this.getCategoryById = this.getCategoryById.bind(this);
    this.createCategoryReq = this.createCategoryReq.bind(this);
    this.deleteCategoryReq = this.deleteCategoryReq.bind(this);
    this.updateCategoryReq = this.updateCategoryReq.bind(this);
  }

  static getInstance(): CategoryService {
    if (!CategoryService.instance) {
      CategoryService.instance = new CategoryService();
    }
    return CategoryService.instance;
  }

  async getCategories(
    params: CategoryFilterParams
  ): Promise<PaginatedResponse<CategoryResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<CategoryResDto>>(
        `${this.endpoints.categories}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch categories error:", error);
      throw new Error(`Fetch categories failed: ${error}`);
    }
  }

  async getCategoryById(id: string): Promise<CategoryDetailResDto> {
    try {
      const response = await httpClient.get<{ data: CategoryDetailResDto }>(
        `${this.endpoints.categories}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get category by ID error:", error);
      throw new Error(`Get category by ID failed: ${error}`);
    }
  }
  

  async createCategoryReq(formData: CategoryReqDto): Promise<CategoryResDto> {
    try {
      const response = await httpClient.post<{ data: CategoryResDto }>(
        this.endpoints.categories,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Create category error:", error);
      throw new Error(`Create category failed: ${error}`);
    }
  }

  async updateCategoryReq(id: string, formData: Partial<CategoryReqDto>): Promise<CategoryResDto> {
    try {
      const response = await httpClient.patch<{ data: CategoryResDto }>(
        `${this.endpoints.categories}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update category error:", error);
      throw new Error(`Update category failed: ${error}`);
    }
  }
  

  async deleteCategoryReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.categories}/${id}`);
    } catch (error) {
      console.log("Delete category error:", error);
      throw new Error(`Delete category failed: ${error}`);
    }
  }
}

const categoryService = CategoryService.getInstance();
export default categoryService;
