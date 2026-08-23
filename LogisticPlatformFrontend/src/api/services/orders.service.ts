import { BaseService } from "@/api/services/base.service";
import { toQuery } from "@/api/fetcher";
import type {
  CreateOrderRequest,
  CreateOrderResponse,
  OrderDetails,
  OrdersFilterOptions,
  OrdersListParams,
  OrdersListResponse,
  OrdersTabCounts,
} from "@/types/orders";

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
}

export const ordersService = new OrdersService();
