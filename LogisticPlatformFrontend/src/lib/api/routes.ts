export const API_V1 = "/api/v1";

export const apiV1 = (path: string) =>
  `${API_V1}${path.startsWith("/") ? path : `/${path}`}`;
