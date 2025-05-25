import httpClient from "./agent";
import type { UserResDto } from "@/types/user/user";

class LoginService {
  private static instance: LoginService;

  private readonly endpoints = {
    login: "/User/Login",
  };

  private constructor() {}

  static getInstance(): LoginService {
    if (!LoginService.instance) {
      LoginService.instance = new LoginService();
    }
    return LoginService.instance;
  }

  // Sử dụng POST vì backend dùng [FromBody]
  async login(user: { userName: string; password: string }): Promise<UserResDto | null> {
    try {
      const response = await httpClient.post<UserResDto>(
        this.endpoints.login,
        {
          userName: user.userName,
          password: user.password,
        }
      );
      return response;
    } catch (error) {
      console.log("Login error:", error);
      throw new Error(`Login failed: ${error}`);
    }
  }
}

const loginService = LoginService.getInstance();
export default loginService;