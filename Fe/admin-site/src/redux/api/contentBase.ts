import { ContentBaseReqDto, ContentBaseFilterParams, ContentBaseResDto } from "@/types/contentBase/contentBase";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

class ContentBaseService {
  private static instance: ContentBaseService;

  private readonly endpoints = {
    contentBases: "/ContentBase",
  };

  private constructor() {
    this.getContentBases = this.getContentBases.bind(this);
    this.getContentBaseById = this.getContentBaseById.bind(this);
    this.createContentBaseReq = this.createContentBaseReq.bind(this);
    this.updateContentBaseReq = this.updateContentBaseReq.bind(this);
    this.deleteContentBaseReq = this.deleteContentBaseReq.bind(this);
  }

  static getInstance(): ContentBaseService {
    if (!ContentBaseService.instance) {
      ContentBaseService.instance = new ContentBaseService();
    }
    return ContentBaseService.instance;
  }

  async getContentBases(params: ContentBaseFilterParams): Promise<PaginatedResponse<ContentBaseResDto>> {
    console.log("Body gửi API:", params);
    const response = await httpClient.post<PaginatedResponse<ContentBaseResDto>>(
      `${this.endpoints.contentBases}/filter`,
        {},
        { params }
    );
    return response;
}



  async getContentBaseById(id: string): Promise<ContentBaseResDto> {
    const response = await httpClient.get<{ data: ContentBaseResDto }>(
      `${this.endpoints.contentBases}/${id}`
    );
    return response.data;
  }

  async createContentBaseReq(data: ContentBaseReqDto): Promise<ContentBaseResDto> {
    const response = await httpClient.post<{ data: ContentBaseResDto }>(
      this.endpoints.contentBases,
      data
    );
    return response.data;
  }

  async updateContentBaseReq(id: string, data: Partial<ContentBaseReqDto>): Promise<ContentBaseResDto> {
    const response = await httpClient.patch<{ data: ContentBaseResDto }>(
      `${this.endpoints.contentBases}/${id}`,
      data
    );
    return response.data;
  }

  async deleteContentBaseReq(id: string): Promise<void> {
    await httpClient.delete(`${this.endpoints.contentBases}/${id}`);
  }
}

const contentBaseService = ContentBaseService.getInstance();
export default contentBaseService;