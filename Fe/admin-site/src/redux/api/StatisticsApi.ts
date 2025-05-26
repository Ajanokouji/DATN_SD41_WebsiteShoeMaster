// src/services/dashboardService.ts
import httpClient from "./agent";

class DashboardService {
  private static instance: DashboardService;

  private readonly endpoints = {
    generalStats: "/Dashboard/general-statistics",
  };

  private constructor() {
    this.getGeneralStatistics = this.getGeneralStatistics.bind(this);
  }

  static getInstance(): DashboardService {
    if (!DashboardService.instance) {
      DashboardService.instance = new DashboardService();
    }
    return DashboardService.instance;
  }

  // Trả về dữ liệu trực tiếp từ API (giả định API trả về đúng format)
  async getGeneralStatistics(): Promise<any> {
    const response = await httpClient.get<{ code: number; message: string; data: any }>(this.endpoints.generalStats);
    // response.data là phần dữ liệu chính từ axios, có thể chứa { code, message, data }
    return response.data;
  }
}

const dashboardService = DashboardService.getInstance();
export default dashboardService;
