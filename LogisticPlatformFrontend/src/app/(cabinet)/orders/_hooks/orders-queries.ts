import { queryOptions } from "@tanstack/react-query";
import { ordersService } from "@/api/services/orders.service";
import type { OrdersListParams } from "@/types/orders";

export const ordersKeys = {
  all: ["orders"] as const,
  list: (params: OrdersListParams) => [...ordersKeys.all, "list", params] as const,
  tabCounts: (params: Omit<OrdersListParams, "tab" | "page" | "pageSize">) =>
    [...ordersKeys.all, "tab-counts", params] as const,
  filterOptions: () => [...ordersKeys.all, "filter-options"] as const,
  detail: (id: string) => [...ordersKeys.all, "detail", id] as const,
  comments: (id: string) => [...ordersKeys.all, "detail", id, "comments"] as const,
  timeline: (id: string) => [...ordersKeys.all, "detail", id, "timeline"] as const,
  operationComments: (orderId: string, operationId: string) =>
    [...ordersKeys.all, "detail", orderId, "operations", operationId, "comments"] as const,
  operationPhotos: (orderId: string, operationId: string) =>
    [...ordersKeys.all, "detail", orderId, "operations", operationId, "photos"] as const,
};

export const ordersListOptions = (params: OrdersListParams) =>
  queryOptions({
    queryKey: ordersKeys.list(params),
    queryFn: () => ordersService.list(params),
  });

export const ordersTabCountsOptions = (
  params: Omit<OrdersListParams, "tab" | "page" | "pageSize">,
) =>
  queryOptions({
    queryKey: ordersKeys.tabCounts(params),
    queryFn: () => ordersService.tabCounts(params),
  });

export const ordersFilterOptionsQuery = () =>
  queryOptions({
    queryKey: ordersKeys.filterOptions(),
    queryFn: () => ordersService.filterOptions(),
    staleTime: 60_000,
  });

export const orderDetailOptions = (id: string) =>
  queryOptions({
    queryKey: ordersKeys.detail(id),
    queryFn: () => ordersService.getById(id),
    enabled: Boolean(id),
  });

export const orderCommentsOptions = (id: string) =>
  queryOptions({
    queryKey: ordersKeys.comments(id),
    queryFn: () => ordersService.getComments(id),
    enabled: Boolean(id),
  });

export const orderTimelineOptions = (id: string) =>
  queryOptions({
    queryKey: ordersKeys.timeline(id),
    queryFn: () => ordersService.getTimeline(id),
    enabled: Boolean(id),
  });

export const operationCommentsOptions = (orderId: string, operationId: string) =>
  queryOptions({
    queryKey: ordersKeys.operationComments(orderId, operationId),
    queryFn: () => ordersService.getOperationComments(orderId, operationId),
    enabled: Boolean(orderId && operationId),
  });

export const operationPhotosOptions = (orderId: string, operationId: string) =>
  queryOptions({
    queryKey: ordersKeys.operationPhotos(orderId, operationId),
    queryFn: () => ordersService.getOperationPhotos(orderId, operationId),
    enabled: Boolean(orderId && operationId),
  });
