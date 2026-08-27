import { ApiError } from "@/types/auth";

export type ApiValidationIssue = {
  field?: string;
  message: string;
};

function readIssue(item: unknown): ApiValidationIssue | null {
  if (typeof item === "string" && item.trim()) {
    return { message: item.trim() };
  }
  if (!item || typeof item !== "object") return null;

  const row = item as {
    errorMessage?: string;
    ErrorMessage?: string;
    message?: string;
    identifier?: string;
    Identifier?: string;
    propertyName?: string;
  };

  const message = row.errorMessage ?? row.ErrorMessage ?? row.message;
  if (!message || typeof message !== "string") return null;

  const field = row.identifier ?? row.Identifier ?? row.propertyName;
  return {
    message,
    field: typeof field === "string" && field ? field : undefined,
  };
}

/** Parse ASP.NET / Ardalis validation (or conflict) bodies into a list. */
export function parseApiIssues(body: unknown): ApiValidationIssue[] {
  if (body == null) return [];

  if (typeof body === "string" && body.trim()) {
    return [{ message: body.trim() }];
  }

  if (Array.isArray(body)) {
    return body.map(readIssue).filter((x): x is ApiValidationIssue => Boolean(x));
  }

  if (typeof body === "object") {
    const row = body as {
      title?: string;
      detail?: string;
      errorMessage?: string;
      message?: string;
      errors?: unknown;
    };

    if (row.errors != null) {
      if (Array.isArray(row.errors)) return parseApiIssues(row.errors);
      if (typeof row.errors === "object") {
        const fromMap: ApiValidationIssue[] = [];
        for (const [field, value] of Object.entries(row.errors as Record<string, unknown>)) {
          if (Array.isArray(value)) {
            for (const msg of value) {
              if (typeof msg === "string" && msg) fromMap.push({ field, message: msg });
            }
          } else if (typeof value === "string" && value) {
            fromMap.push({ field, message: value });
          }
        }
        if (fromMap.length) return fromMap;
      }
    }

    const single = row.errorMessage ?? row.message ?? row.detail ?? row.title;
    if (typeof single === "string" && single.trim()) {
      return [{ message: single.trim() }];
    }
  }

  return [];
}

export function formatApiErrorMessage(body: unknown, status: number): string {
  const issues = parseApiIssues(body);
  if (issues.length) {
    return issues
      .map((i) => (i.field ? `${i.field}: ${i.message}` : i.message))
      .join("; ");
  }
  return `HTTP ${status}`;
}

export function getErrorIssues(err: unknown): ApiValidationIssue[] {
  if (err instanceof ApiError) {
    const fromBody = parseApiIssues(err.body);
    if (fromBody.length) return fromBody;
    if (err.message && !err.message.startsWith("HTTP ")) {
      return [{ message: err.message }];
    }
  }
  if (err instanceof Error && err.message) {
    return [{ message: err.message }];
  }
  return [{ message: "Something went wrong" }];
}
