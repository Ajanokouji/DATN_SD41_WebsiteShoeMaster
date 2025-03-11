import CategoryReqDto from "@/types/category/category";
import httpClient from "./agent";

class CategoryService {
  private static instance: CategoryService;

  private readonly endpoints = {
    createCategory: "/category/",
  };

  private constructor() {
    this.createCategoryReq = this.createCategoryReq.bind(this);
  }

  static getInstance(): CategoryService {
    if (!CategoryService.instance) {
      CategoryService.instance = new CategoryService();
    }
    return CategoryService.instance;
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
}

const categoryService = CategoryService.getInstance();
export default categoryService;
