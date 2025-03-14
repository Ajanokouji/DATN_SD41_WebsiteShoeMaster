// src/services/CustomerService.ts

import { CustomerReqDto, CustomerResDto, CustomerDetailResDto } from "@/types/customer/customer";
import httpClient from "./agent";
import { PaginatedResponse, PaginationParams } from "@/types/common/pagination";
import { generateCustomerCode } from "@/utils/generateCode";

class CustomerService {
    private static instance: CustomerService;

    private readonly endpoints = {
        customers: "/Customer", // Endpoint API cho khách hàng
    };

    private constructor() {
        // Liên kết các phương thức với instance
        this.getCustomers = this.getCustomers.bind(this);
        this.getCustomerById = this.getCustomerById.bind(this);
        this.createCustomerReq = this.createCustomerReq.bind(this);
        this.deleteCustomerReq = this.deleteCustomerReq.bind(this);
        this.updateCustomerReq = this.updateCustomerReq.bind(this);
    }

    // Phương thức Singleton để tạo hoặc lấy instance của CustomerService
    static getInstance(): CustomerService {
        if (!CustomerService.instance) {
            CustomerService.instance = new CustomerService();
        }
        return CustomerService.instance;
    }

    // Lấy danh sách khách hàng với phân trang
    async getCustomers(params: PaginationParams): Promise<PaginatedResponse<CustomerResDto>> {
        try {
            const response = await httpClient.post<PaginatedResponse<CustomerResDto>>(
                `${this.endpoints.customers}/filter`, // URL API
                {}, // Dữ liệu body (có thể thay đổi nếu cần)
                { params } // Tham số phân trang
            );
            return { data: response.data }; // Trả về dữ liệu từ API
        } catch (error) {
            console.log("Fetch customers error:", error);
            throw new Error(`Fetch customers failed: ${error}`);
        }
    }

    // Lấy chi tiết khách hàng theo ID
    async getCustomerById(id: string): Promise<CustomerDetailResDto> {
        try {
            const response = await httpClient.get<{ data: CustomerDetailResDto }>(
                `${this.endpoints.customers}/${id}` // URL API
            );
            return response.data; // Trả về dữ liệu chi tiết của khách hàng
        } catch (error) {
            console.log("Get customer by ID error:", error);
            throw new Error(`Get customer by ID failed: ${error}`);
        }
    }

    // Tạo một khách hàng mới
    async createCustomerReq(formData: CustomerReqDto): Promise<CustomerResDto> {
        try {
            formData.code = generateCustomerCode(); // Sinh mã khách hàng tự động

            const response = await httpClient.post<{ data: CustomerResDto }>(
                this.endpoints.customers, formData // URL API
            );
            return response.data; // Trả về thông tin khách hàng vừa tạo
        } catch (error) {
            console.log("Create customer error:", error);
            throw new Error(`Create customer failed: ${error}`);
        }
    }

    // Cập nhật khách hàng theo ID
    async updateCustomerReq(id: string, formData: Partial<CustomerReqDto>): Promise<CustomerResDto> {
        try {
            const response = await httpClient.patch<{ data: CustomerResDto }>(
                `${this.endpoints.customers}/${id}`, formData // URL API
            );
            return response.data; // Trả về thông tin khách hàng đã được cập nhật
        } catch (error) {
            console.log("Update customer error:", error);
            throw new Error(`Update customer failed: ${error}`);
        }
    }

    // Xóa khách hàng theo ID
    async deleteCustomerReq(id: string): Promise<void> {
        try {
            await httpClient.delete(`${this.endpoints.customers}/${id}`); // Gọi API để xóa khách hàng
        } catch (error) {
            console.log("Delete customer error:", error);
            throw new Error(`Delete customer failed: ${error}`);
        }
    }
}

// Khởi tạo instance của CustomerService và xuất nó
const customerService = CustomerService.getInstance();
export default customerService;
