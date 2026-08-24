import { BaseService } from "@/api/services/base.service";
import { toQuery } from "@/api/fetcher";
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
    return this.get<OrdersListResponse>(`/api/orders${toQuery(params)}`);
  }

  tabCounts(params: Omit<OrdersListParams, "tab" | "page" | "pageSize"> = {}) {
    return this.get<OrdersTabCounts>(`/api/orders/tab-counts${toQuery(params)}`);
  }

  filterOptions() {
    return this.get<OrdersFilterOptions>("/api/orders/filter-options");
  }

  getById(id: string) {
    return this.get<OrderDetails>(`/api/orders/${id}`);
  }

  create(payload: CreateOrderRequest) {
    return this.post<CreateOrderResponse>("/api/orders", payload);
  }

  update(id: string, payload: UpdateOrderRequest) {
    return this.patch<{ id: string }>(`/api/orders/${id}`, payload);
  }

  async exportCsv(params: Omit<OrdersListParams, "page" | "pageSize"> = {}) {
    await downloadFile(`/api/orders/export${toQuery(params)}`);
  }

  downloadBolPdf(orderId: string) {
    return downloadFile(`/api/orders/${orderId}/bol.pdf`);
  }

  downloadQr(orderId: string) {
    return downloadFile(`/api/orders/${orderId}/qr`);
  }

  getComments(orderId: string) {
    return this.get<OrderComment[]>(`/api/orders/${orderId}/comments`);
  }

  addComment(orderId: string, payload: AddTextRequest) {
    return this.post<OrderComment>(`/api/orders/${orderId}/comments`, payload);
  }

  getTimeline(orderId: string) {
    return this.get<OrderTimelineEntry[]>(`/api/orders/${orderId}/timeline`);
  }

  addTimelineEntry(orderId: string, payload: AddTextRequest) {
    return this.post<OrderTimelineEntry>(`/api/orders/${orderId}/timeline`, payload);
  }

  addOperation(orderId: string, payload: AddOrderOperationRequest) {
    return this.post<OrderOperation>(`/api/orders/${orderId}/operations`, payload);
  }

  deleteOperation(orderId: string, operationId: string) {
    return this.delete<void>(`/api/orders/${orderId}/operations/${operationId}`);
  }

  getOperationComments(orderId: string, operationId: string) {
    return this.get<OrderComment[]>(
      `/api/orders/${orderId}/operations/${operationId}/comments`,
    );
  }

  addOperationComment(orderId: string, operationId: string, payload: AddTextRequest) {
    return this.post<OrderComment>(
      `/api/orders/${orderId}/operations/${operationId}/comments`,
      payload,
    );
  }

  getOperationPhotos(orderId: string, operationId: string) {
    return this.get<OrderPhoto[]>(
      `/api/orders/${orderId}/operations/${operationId}/photos`,
    );
  }

  addOperationPhoto(orderId: string, operationId: string, file: File, sortOrder?: number) {
    const form = new FormData();
    form.append("file", file);
    if (sortOrder !== undefined) form.append("sortOrder", String(sortOrder));
    return postForm<OrderPhoto>(
      `/api/orders/${orderId}/operations/${operationId}/photos`,
      form,
    );
  }

  deleteOperationPhoto(orderId: string, operationId: string, photoId: string) {
    return this.delete<void>(
      `/api/orders/${orderId}/operations/${operationId}/photos/${photoId}`,
    );
  }

  addSupply(orderId: string, payload: AddOrderSupplyRequest) {
    return this.post<OrderSupply>(`/api/orders/${orderId}/supplies`, payload);
  }

  addSupplyFromCatalog(orderId: string, payload: AddSupplyFromCatalogRequest) {
    return this.post<OrderSupply>(`/api/orders/${orderId}/supplies/from-catalog`, payload);
  }

  updateSupply(orderId: string, supplyId: string, payload: UpdateOrderSupplyRequest) {
    return this.patch<OrderSupply>(`/api/orders/${orderId}/supplies/${supplyId}`, payload);
  }

  updateSupplyQuantity(orderId: string, supplyId: string, quantity: number) {
    return this.patch<OrderSupply>(`/api/orders/${orderId}/supplies/${supplyId}/quantity`, {
      quantity,
    });
  }

  deleteSupply(orderId: string, supplyId: string) {
    return this.delete<void>(`/api/orders/${orderId}/supplies/${supplyId}`);
  }

  addWarehousePhoto(orderId: string, file: File, sortOrder?: number) {
    const form = new FormData();
    form.append("file", file);
    if (sortOrder !== undefined) form.append("sortOrder", String(sortOrder));
    return postForm<OrderPhoto>(`/api/orders/${orderId}/warehouse-photos`, form);
  }

  deleteWarehousePhoto(orderId: string, photoId: string) {
    return this.delete<void>(`/api/orders/${orderId}/warehouse-photos/${photoId}`);
  }
}

export const ordersService = new OrdersService();
