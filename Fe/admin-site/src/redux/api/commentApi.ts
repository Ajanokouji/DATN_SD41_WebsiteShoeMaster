import CommentReqDto, { CommentFilterParams, CommentResDto } from "@/types/comment/comment";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

export type ResponseObject<T> = { data: T };
export type ResponseList<T> = { data: T[] };

class CommentService {
  private static instance: CommentService;

  private readonly endpoints = {
    comments: "/Comments",
  };

  private constructor() {}

  static getInstance(): CommentService {
    if (!CommentService.instance) {
      CommentService.instance = new CommentService();
    }
    return CommentService.instance;
  }

  async getCommentById(id: string): Promise<CommentResDto> {
    try {
      const response = await httpClient.get<ResponseObject<CommentResDto>>(
        `${this.endpoints.comments}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get comment by ID error:", error);
      throw new Error(`Get comment by ID failed: ${error}`);
    }
  }

  async getComments(params: CommentFilterParams): Promise<PaginatedResponse<CommentResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<CommentResDto>>(
        `${this.endpoints.comments}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Get comments error:", error);
      throw new Error(`Get comments failed: ${error}`);
    }
  }

  async getCommentCount(params: CommentFilterParams): Promise<number> {
    try {
      const response = await httpClient.post<ResponseObject<number>>(
        `${this.endpoints.comments}/count`,
        {},
        { params }
      );
      return response.data;
    } catch (error) {
      console.log("Get comment count error:", error);
      throw new Error(`Get comment count failed: ${error}`);
    }
  }

  async listAllComments(queryModel: Partial<CommentReqDto>): Promise<CommentResDto[]> {
    try {
      const response = await httpClient.post<ResponseList<CommentResDto>>(
        `${this.endpoints.comments}/list`,
        queryModel
      );
      return response.data;
    } catch (error) {
      console.log("List all comments error:", error);
      throw new Error(`List all comments failed: ${error}`);
    }
  }

  async listCommentsByIds(ids: string[]): Promise<CommentResDto[]> {
    try {
      const response = await httpClient.post<ResponseList<CommentResDto>>(
        `${this.endpoints.comments}/list-by-ids`,
        ids
      );
      return response.data;
    } catch (error) {
      console.log("List comments by ids error:", error);
      throw new Error(`List comments by ids failed: ${error}`);
    }
  }

  async createComments(entities: CommentReqDto[]): Promise<CommentResDto[]> {
    try {
      const response = await httpClient.post<ResponseList<CommentResDto>>(
        this.endpoints.comments,
        entities
      );
      return response.data;
    } catch (error) {
      console.log("Create comments error:", error);
      throw new Error(`Create comments failed: ${error}`);
    }
  }

  async deleteComment(id: string): Promise<CommentResDto> {
    try {
      const response = await httpClient.delete<ResponseObject<CommentResDto>>(
        `${this.endpoints.comments}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Delete comment error:", error);
      throw new Error(`Delete comment failed: ${error}`);
    }
  }

  async deleteComments(ids: string[]): Promise<CommentResDto[]> {
    try {
      const response = await httpClient.delete<ResponseList<CommentResDto>>(
        this.endpoints.comments,
        { data: ids }
      );
      return response.data;
    } catch (error) {
      console.log("Delete comments error:", error);
      throw new Error(`Delete comments failed: ${error}`);
    }
  }

  async updateComment(id: string, data: Partial<CommentResDto>): Promise<CommentResDto> {
    try {
      const response = await httpClient.patch<ResponseObject<CommentResDto>>(
        `${this.endpoints.comments}/${id}`,
        { ...data, id } // đảm bảo có id trong body
      );
      return response.data;
    } catch (error) {
      console.log("Update comment error:", error);
      throw new Error(`Update comment failed: ${error}`);
    }
  }
}

const commentService = CommentService.getInstance();
export default commentService;