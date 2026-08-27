/**
 * Only the Ocelot gateway. Frontend/BFF must not address Web API instances.
 * Gateway load-balances to API replicas inside the Container Apps environment.
 */
function resolveApiBaseUrl(): string {
  const raw =
    process.env.API_BASE_URL?.trim() ||
    process.env.NEXT_PUBLIC_API_URL?.trim() ||
    "http://localhost:5124";

  return raw.replace(/\/+$/, "");
}

export const API_BASE_URL = resolveApiBaseUrl();
