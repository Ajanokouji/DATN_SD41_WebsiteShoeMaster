import CategoryReqDto, { CategoryResDto } from "@/types/category/category";
import httpClient from "./agent";
import { PaginatedResponse, PaginationParams } from "@/types/common/pagination";

class CategoryService {
  private static instance: CategoryService;

  private readonly endpoints = {
    fetchCategories: "/Categories/filter",
    createCategory: "/Categories",
    deleteCategory: "/Categories",
  };

  private constructor() {
    this.getCategories = this.getCategories.bind(this);
    this.createCategoryReq = this.createCategoryReq.bind(this);
    this.deleteCategoryReq = this.deleteCategoryReq.bind(this);
  }

  static getInstance(): CategoryService {
    if (!CategoryService.instance) {
      CategoryService.instance = new CategoryService();
    }
    return CategoryService.instance;
  }

  async getCategories(
    params: PaginationParams
  ): Promise<PaginatedResponse<CategoryResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<CategoryResDto>>(
        `${this.endpoints.fetchCategories}`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch categories error:", error);
      throw new Error(`Fetch categories failed: ${error}`);
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

  async deleteCategoryReq(id: string): Promise<void> {
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
