import { CustomerFilterParams, CustomerReqDto, CustomerResDto } from "@/types/customer/customer";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

class CustomerService {
  private static instance: CustomerService;

  private readonly endpoints = {
    customers: "/Customer",
  };

  private constructor() {
    this.getCustomers = this.getCustomers.bind(this);
    this.getCustomerById = this.getCustomerById.bind(this);
    this.createCustomerReq = this.createCustomerReq.bind(this);
    this.deleteCustomerReq = this.deleteCustomerReq.bind(this);
    this.updateCustomerReq = this.updateCustomerReq.bind(this);
  }

  static getInstance(): CustomerService {
    if (!CustomerService.instance) {
      CustomerService.instance = new CustomerService();
    }
    return CustomerService.instance;
  }

  async getCustomers(
    params: CustomerFilterParams
  ): Promise<PaginatedResponse<CustomerResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<CustomerResDto>>(
        `${this.endpoints.customers}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch customers error:", error);
      throw new Error(`Fetch customers failed: ${error}`);
    }
  }

  async getCustomerById(id: string): Promise<CustomerResDto> {
    try {
      const response = await httpClient.get<{ data: CustomerResDto }>(
        `${this.endpoints.customers}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get customer by ID error:", error);
      throw new Error(`Get customer by ID failed: ${error}`);
    }
  }
  

  async createCustomerReq(formData: CustomerReqDto): Promise<CustomerResDto> {
    try {
      const response = await httpClient.post<{ data: CustomerResDto }>(
        this.endpoints.customers,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Create customer error:", error);
      throw new Error(`Create customer failed: ${error}`);
    }
  }

  async updateCustomerReq(id: string, formData: Partial<CustomerReqDto>): Promise<CustomerResDto> {
    try {
      const response = await httpClient.patch<{ data: CustomerResDto }>(
        `${this.endpoints.customers}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update customer error:", error);
      throw new Error(`Update customer failed: ${error}`);
    }
  }
  

  async deleteCustomerReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.customers}/${id}`);
    } catch (error) {
      console.log("Delete customer error:", error);
      throw new Error(`Delete customer failed: ${error}`);
    }
  }
}

const customerService = CustomerService.getInstance();
export default customerService;
