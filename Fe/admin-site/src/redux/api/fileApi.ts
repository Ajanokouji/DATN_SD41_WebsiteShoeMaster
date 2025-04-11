import httpClient from "./agent";

class FileService {
  private static instance: FileService;
  private readonly endpoints = {
    upload: "/files/upload",
  };

  private constructor() {}

  public static getInstance(): FileService {
    if (!FileService.instance) {
      FileService.instance = new FileService();
    }
    return FileService.instance;
  }

  async uploadFile(file: File): Promise<string> {
    try {
      const formData = new FormData();
      formData.append("File", file);
      formData.append("FileName", file.name);

      const response = await httpClient.post<{ data: string }>(
        this.endpoints.upload,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Upload file error:", error);
      throw error;
    }
  }
}

export default FileService.getInstance(); 