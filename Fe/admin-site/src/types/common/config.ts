export interface ApiResponse<T> {
  data: T;
  status: number;
  statusText: string;
}

export interface TokenResponse {
  data: string;
}

export const API_CONFIG = {
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 120000,
  tokenCookieName: "mic-auth",
  refreshTokenCookieName: "mic-refresh",
  refreshTokenEndpoint: "/application/refresh-token",
  loginPath: "/login",
} as const;

export interface AuthResponse {
  message: string;
  access_token: string;
  refresh_token: string;
}
