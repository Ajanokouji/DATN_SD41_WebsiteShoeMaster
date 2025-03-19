import RelationReqDto, { RelationFilterParams, RelationResDto } from "@/types/category/relation";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

class RelationService {
  private static instance: RelationService;

  private readonly endpoints = {
    relations: "/ProductCategoriesRelation",
  };

  private constructor() {
    this.getRelations = this.getRelations.bind(this);
    this.getRelationById = this.getRelationById.bind(this);
    this.createRelationReq = this.createRelationReq.bind(this);
    this.deleteRelationReq = this.deleteRelationReq.bind(this);
    this.updateRelationReq = this.updateRelationReq.bind(this);
  }

  static getInstance(): RelationService {
    if (!RelationService.instance) {
      RelationService.instance = new RelationService();
    }
    return RelationService.instance;
  }

  async getRelations(
    params: RelationFilterParams
  ): Promise<PaginatedResponse<RelationResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<RelationResDto>>(
        `${this.endpoints.relations}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch relations error:", error);
      throw new Error(`Fetch relations failed: ${error}`);
    }
  }

  async getRelationById(id: string): Promise<RelationResDto> {
    try {
      const response = await httpClient.get<{ data: RelationResDto }>(
        `${this.endpoints.relations}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get relation by ID error:", error);
      throw new Error(`Get relation by ID failed: ${error}`);
    }
  }

  async createRelationReq(formData: RelationReqDto): Promise<RelationResDto> {
    try {
      const response = await httpClient.post<{ data: RelationResDto }>(
        this.endpoints.relations,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Create relation error:", error);
      throw new Error(`Create relation failed: ${error}`);
    }
  }

  async updateRelationReq(
    id: string,
    formData: Partial<RelationReqDto>
  ): Promise<RelationResDto> {
    try {
      const response = await httpClient.patch<{ data: RelationResDto }>(
        `${this.endpoints.relations}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update relation error:", error);
      throw new Error(`Update relation failed: ${error}`);
    }
  }

  async deleteRelationReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.relations}/${id}`);
    } catch (error) {
      console.log("Delete relation error:", error);
      throw new Error(`Delete relation failed: ${error}`);
    }
  }
}

const relationService = RelationService.getInstance();
export default relationService;
