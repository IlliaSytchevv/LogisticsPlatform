import type { LoginRequest, LoginResponse, RegisterRequest } from "@/types/auth";
import { apiV1 } from "@/lib/api/routes";
import { API_BASE_URL } from "@/lib/api/base-url";

const authApi = `${API_BASE_URL}${apiV1("/auth")}`;

/**
 * Direct backend auth calls (used only if you bypass server actions).
 * Prefer loginAction / refreshAccessTokenAction / logoutAction.
 */
class AuthService {
  login(payload: LoginRequest) {
    return fetch(`${authApi}/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
      cache: "no-store",
    }).then(async (response) => {
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      return (await response.json()) as LoginResponse;
    });
  }

  register(payload: RegisterRequest) {
    return fetch(`${authApi}/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
      cache: "no-store",
    });
  }
}

export const authService = new AuthService();
