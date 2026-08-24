import { queryOptions } from "@tanstack/react-query";
import { dashboardService } from "@/api/services/dashboard.service";
import type { ActivityPeriod } from "@/types/dashboard";

export const dashboardKeys = {
  all: ["dashboard"] as const,
  metrics: () => [...dashboardKeys.all, "metrics"] as const,
  activeOrders: (take: number) => [...dashboardKeys.all, "active-orders", take] as const,
  activity: (period: ActivityPeriod) => [...dashboardKeys.all, "activity", period] as const,
};

export const dashboardMetricsOptions = () =>
  queryOptions({
    queryKey: dashboardKeys.metrics(),
    queryFn: () => dashboardService.metrics(),
  });

export const dashboardActiveOrdersOptions = (take = 4) =>
  queryOptions({
    queryKey: dashboardKeys.activeOrders(take),
    queryFn: () => dashboardService.activeOrders(take),
  });

export const dashboardActivityOptions = (period: ActivityPeriod) =>
  queryOptions({
    queryKey: dashboardKeys.activity(period),
    queryFn: () => dashboardService.activity(period),
  });
