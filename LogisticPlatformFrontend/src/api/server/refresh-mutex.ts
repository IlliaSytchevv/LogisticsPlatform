import { refreshAccessTokenAction } from "@/actions/auth/refresh-access-token.action";

const inflightByRefresh = new Map<string, Promise<boolean>>();

/**
 * Single-flight refresh: concurrent 401s with the same refresh token wait on one POST.
 */
export async function refreshAccessTokenOnce(refreshToken: string): Promise<boolean> {
  const existing = inflightByRefresh.get(refreshToken);
  if (existing) return existing;

  const promise = refreshAccessTokenAction().finally(() => {
    inflightByRefresh.delete(refreshToken);
  });
  inflightByRefresh.set(refreshToken, promise);
  return promise;
}
