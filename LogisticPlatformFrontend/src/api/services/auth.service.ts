import type { LoginRequest, LoginResponse, RegisterRequest } from "@/types/auth";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5124";

/**
 * Direct backend auth calls (used only if you bypass server actions).
 * Prefer loginAction / refreshAccessTokenAction / logoutAction.
 */
class AuthService {
  login(payload: LoginRequest) {
    return fetch(`${API_URL}/api/auth/login`, {
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
    return fetch(`${API_URL}/api/auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
      cache: "no-store",
    });
  }
}

export const authService = new AuthService();
