import { BaseService } from "@/api/services/base.service";
import { toQuery } from "@/api/fetcher";
import type {
  ActivityPeriod,
  DashboardActiveOrders,
  DashboardActivity,
  DashboardMetrics,
} from "@/types/dashboard";

class DashboardService extends BaseService {
  metrics() {
    return this.get<DashboardMetrics>("/api/dashboard/metrics");
  }

  activeOrders(take = 4) {
    return this.get<DashboardActiveOrders>(
      `/api/dashboard/active-orders${toQuery({ take })}`,
    );
  }

  activity(period: ActivityPeriod = 3) {
    return this.get<DashboardActivity>(
      `/api/dashboard/activity${toQuery({ period })}`,
    );
  }
}

export const dashboardService = new DashboardService();
