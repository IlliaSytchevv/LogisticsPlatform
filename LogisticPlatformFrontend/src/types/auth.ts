export type LoginRequest = {
  username: string;
  password: string;
};

export type LoginResponse = {
  jwtToken: string;
};

export type RegisterRequest = {
  name: string;
  email: string;
  password: string;
  role: number;
};

export type AuthUser = {
  id: string;
  name: string;
  roles: string[];
};

export class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
    public body?: unknown,
  ) {
    super(message);
    this.name = "ApiError";
  }
}
