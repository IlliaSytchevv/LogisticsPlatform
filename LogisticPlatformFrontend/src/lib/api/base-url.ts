/**
 * Only the Ocelot gateway. Frontend/BFF must not address Web API instances.
 * Gateway (5124) load-balances to 5217 / 5218.
 */
export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5124";
