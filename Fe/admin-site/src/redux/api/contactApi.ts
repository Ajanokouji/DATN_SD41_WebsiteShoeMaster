
import ContactReqDto, { ContactFilterParams, ContactResDto } from "@/types/contact/contact";
import httpClient from "./agent";
import { PaginatedResponse } from "@/types/common/pagination";

class ContactService {
  private static instance: ContactService;

  private readonly endpoints = {
    contacts: "/Contact",
  };

  private constructor() {
    this.getContacts = this.getContacts.bind(this);
    this.getContactById = this.getContactById.bind(this);
    this.createContactReq = this.createContactReq.bind(this);
    this.deleteContactReq = this.deleteContactReq.bind(this);
    this.updateContactReq = this.updateContactReq.bind(this);
  }

  static getInstance(): ContactService {
    if (!ContactService.instance) {
      ContactService.instance = new ContactService();
    }
    return ContactService.instance;
  }

  async getContacts(
    params: ContactFilterParams
  ): Promise<PaginatedResponse<ContactResDto>> {
    try {
      const response = await httpClient.post<PaginatedResponse<ContactResDto>>(
        `${this.endpoints.contacts}/filter`,
        {},
        { params }
      );
      return response;
    } catch (error) {
      console.log("Fetch contacts error:", error);
      throw new Error(`Fetch contacts failed: ${error}`);
    }
  }

  async getContactById(id: string): Promise<ContactResDto> {
    try {
      const response = await httpClient.get<{ data: ContactResDto }>(
        `${this.endpoints.contacts}/${id}`
      );
      return response.data;
    } catch (error) {
      console.log("Get contact by ID error:", error);
      throw new Error(`Get contact by ID failed: ${error}`);
    }
  }

  async createContactReq(formData: ContactReqDto): Promise<ContactResDto> {
    try {
      const response = await httpClient.post<{ data: ContactResDto }>(
        this.endpoints.contacts,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Create contact error:", error);
      throw new Error(`Create contact failed: ${error}`);
    }
  }

  async updateContactReq(
    id: string,
    formData: Partial<ContactReqDto>
  ): Promise<ContactResDto> {
    try {
      const response = await httpClient.patch<{ data: ContactResDto }>(
        `${this.endpoints.contacts}/${id}`,
        formData
      );
      return response.data;
    } catch (error) {
      console.log("Update contact error:", error);
      throw new Error(`Update contact failed: ${error}`);
    }
  }

  async deleteContactReq(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.endpoints.contacts}/${id}`);
    } catch (error) {
      console.log("Delete contact error:", error);
      throw new Error(`Delete contact failed: ${error}`);
    }
  }
}

const contactService = ContactService.getInstance();
export default contactService;
