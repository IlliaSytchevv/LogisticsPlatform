import { fetcher } from "@/api/fetcher";

export abstract class BaseService {
  protected get<T>(path: string, init?: RequestInit & { token?: string | null }) {
    return fetcher<T>(path, { ...init, method: "GET" });
  }

  protected post<T>(
    path: string,
    body?: unknown,
    init?: RequestInit & { token?: string | null; skipAuth?: boolean },
  ) {
    return fetcher<T>(path, {
      ...init,
      method: "POST",
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  }

  protected put<T>(path: string, body?: unknown, init?: RequestInit & { token?: string | null }) {
    return fetcher<T>(path, {
      ...init,
      method: "PUT",
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  }

  protected patch<T>(
    path: string,
    body?: unknown,
    init?: RequestInit & { token?: string | null },
  ) {
    return fetcher<T>(path, {
      ...init,
      method: "PATCH",
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  }

  protected delete<T>(path: string, init?: RequestInit & { token?: string | null }) {
    return fetcher<T>(path, { ...init, method: "DELETE" });
  }
}
