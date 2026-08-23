import { BaseService } from "@/api/services/base.service";
import type { LoginRequest, LoginResponse, RegisterRequest } from "@/types/auth";

class AuthService extends BaseService {
  login(payload: LoginRequest) {
    return this.post<LoginResponse>("/api/auth/login", payload, { skipAuth: true });
  }

  register(payload: RegisterRequest) {
    return this.post("/api/auth/register", payload, { skipAuth: true });
  }
}

export const authService = new AuthService();
