import { BaseService } from "@/api/services/base.service";
import { toQuery } from "@/api/fetcher";
import { apiV1 } from "@/lib/api/routes";
import { ApiError } from "@/types/auth";
import type {
  AddOrderOperationRequest,
  AddOrderSupplyRequest,
  AddSupplyFromCatalogRequest,
  AddTextRequest,
  CreateOrderRequest,
  CreateOrderResponse,
  OrderComment,
  OrderDetails,
  OrderOperation,
  OrderPhoto,
  OrdersFilterOptions,
  OrdersListParams,
  OrdersListResponse,
  OrdersTabCounts,
  OrderSupply,
  OrderTimelineEntry,
  UpdateOrderRequest,
  UpdateOrderSupplyRequest,
} from "@/types/orders";

const ordersApi = apiV1("/orders");

async function downloadFile(path: string) {
  const response = await fetch(`/api/backend${path}`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    const raw = await response.text();
    throw new ApiError(raw || `HTTP ${response.status}`, response.status, raw);
  }

  const blob = await response.blob();
  const disposition = response.headers.get("content-disposition") ?? "";
  const match = /filename\*?=(?:UTF-8''|")?([^\";]+)/i.exec(disposition);
  const fileName = match
    ? decodeURIComponent(match[1].replace(/"/g, ""))
    : path.split("/").pop() ?? "download";

  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = fileName;
  a.click();
  URL.revokeObjectURL(url);
}

async function postForm<T>(path: string, formData: FormData): Promise<T> {
  const response = await fetch(`/api/backend${path}`, {
    method: "POST",
    credentials: "include",
    body: formData,
  });

  if (!response.ok) {
    const raw = await response.text();
    let body: unknown = raw;
    try {
      body = raw ? JSON.parse(raw) : null;
    } catch {
      // keep raw
    }
    throw new ApiError(
      typeof body === "string" && body ? body : `HTTP ${response.status}`,
      response.status,
      body,
    );
  }

  if (response.status === 204) return undefined as T;
  const contentType = response.headers.get("content-type") ?? "";
  if (!contentType.includes("application/json")) return undefined as T;
  return (await response.json()) as T;
}

/** Photo/document URLs go through BFF so httpOnly auth applies. */
export function mediaUrl(path: string) {
  if (!path) return path;
  if (path.startsWith("http://") || path.startsWith("https://")) {
    try {
      const u = new URL(path);
      return `/api/backend${u.pathname}${u.search}`;
    } catch {
      return path;
    }
  }
  return `/api/backend${path.startsWith("/") ? path : `/${path}`}`;
}

class OrdersService extends BaseService {
  list(params: OrdersListParams = {}) {
    return this.get<OrdersListResponse>(`${ordersApi}${toQuery(params)}`);
  }

  tabCounts(params: Omit<OrdersListParams, "tab" | "page" | "pageSize"> = {}) {
    return this.get<OrdersTabCounts>(`${ordersApi}/tab-counts${toQuery(params)}`);
  }

  filterOptions() {
    return this.get<OrdersFilterOptions>(`${ordersApi}/filter-options`);
  }

  getById(id: string) {
    return this.get<OrderDetails>(`${ordersApi}/${id}`);
  }

  create(payload: CreateOrderRequest) {
    return this.post<CreateOrderResponse>(`${ordersApi}`, payload);
  }

  update(id: string, payload: UpdateOrderRequest) {
    return this.patch<{ id: string }>(`${ordersApi}/${id}`, payload);
  }

  async exportCsv(params: Omit<OrdersListParams, "page" | "pageSize"> = {}) {
    await downloadFile(`${ordersApi}/export${toQuery(params)}`);
  }

  downloadBolPdf(orderId: string) {
    return downloadFile(`${ordersApi}/${orderId}/bol.pdf`);
  }

  downloadQr(orderId: string) {
    return downloadFile(`${ordersApi}/${orderId}/qr`);
  }

  getComments(orderId: string) {
    return this.get<OrderComment[]>(`${ordersApi}/${orderId}/comments`);
  }

  addComment(orderId: string, payload: AddTextRequest) {
    return this.post<OrderComment>(`${ordersApi}/${orderId}/comments`, payload);
  }

  getTimeline(orderId: string) {
    return this.get<OrderTimelineEntry[]>(`${ordersApi}/${orderId}/timeline`);
  }

  addTimelineEntry(orderId: string, payload: AddTextRequest) {
    return this.post<OrderTimelineEntry>(`${ordersApi}/${orderId}/timeline`, payload);
  }

  addOperation(orderId: string, payload: AddOrderOperationRequest) {
    return this.post<OrderOperation>(`${ordersApi}/${orderId}/operations`, payload);
  }

  deleteOperation(orderId: string, operationId: string) {
    return this.delete<void>(`${ordersApi}/${orderId}/operations/${operationId}`);
  }

  getOperationComments(orderId: string, operationId: string) {
    return this.get<OrderComment[]>(
      `${ordersApi}/${orderId}/operations/${operationId}/comments`,
    );
  }

  addOperationComment(orderId: string, operationId: string, payload: AddTextRequest) {
    return this.post<OrderComment>(
      `${ordersApi}/${orderId}/operations/${operationId}/comments`,
      payload,
    );
  }

  getOperationPhotos(orderId: string, operationId: string) {
    return this.get<OrderPhoto[]>(
      `${ordersApi}/${orderId}/operations/${operationId}/photos`,
    );
  }

  addOperationPhoto(orderId: string, operationId: string, file: File) {
    const form = new FormData();
    form.append("file", file);
    return postForm<OrderPhoto>(
      `${ordersApi}/${orderId}/operations/${operationId}/photos`,
      form,
    );
  }

  deleteOperationPhoto(orderId: string, operationId: string, photoId: string) {
    return this.delete<void>(
      `${ordersApi}/${orderId}/operations/${operationId}/photos/${photoId}`,
    );
  }

  addSupply(orderId: string, payload: AddOrderSupplyRequest) {
    return this.post<OrderSupply>(`${ordersApi}/${orderId}/supplies`, payload);
  }

  addSupplyFromCatalog(orderId: string, payload: AddSupplyFromCatalogRequest) {
    return this.post<OrderSupply>(`${ordersApi}/${orderId}/supplies/from-catalog`, payload);
  }

  updateSupply(orderId: string, supplyId: string, payload: UpdateOrderSupplyRequest) {
    return this.patch<OrderSupply>(`${ordersApi}/${orderId}/supplies/${supplyId}`, payload);
  }

  updateSupplyQuantity(orderId: string, supplyId: string, quantity: number) {
    return this.patch<OrderSupply>(`${ordersApi}/${orderId}/supplies/${supplyId}/quantity`, {
      quantity,
    });
  }

  deleteSupply(orderId: string, supplyId: string) {
    return this.delete<void>(`${ordersApi}/${orderId}/supplies/${supplyId}`);
  }

  addWarehousePhoto(orderId: string, file: File) {
    const form = new FormData();
    form.append("file", file);
    return postForm<OrderPhoto>(`${ordersApi}/${orderId}/warehouse-photos`, form);
  }

  deleteWarehousePhoto(orderId: string, photoId: string) {
    return this.delete<void>(`${ordersApi}/${orderId}/warehouse-photos/${photoId}`);
  }
}

export const ordersService = new OrdersService();
