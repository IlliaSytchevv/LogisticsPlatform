import { BaseService } from "@/api/services/base.service";
import { toQuery } from "@/api/fetcher";
import { apiV1 } from "@/lib/api/routes";
import type {
  ActivityPeriod,
  DashboardActiveOrders,
  DashboardActivity,
  DashboardMetrics,
} from "@/types/dashboard";

class DashboardService extends BaseService {
  metrics() {
    return this.get<DashboardMetrics>(apiV1("/dashboard/metrics"));
  }

  activeOrders(take = 4) {
    return this.get<DashboardActiveOrders>(
      `${apiV1("/dashboard/active-orders")}${toQuery({ take })}`,
    );
  }

  activity(period: ActivityPeriod = 3) {
    return this.get<DashboardActivity>(
      `${apiV1("/dashboard/activity")}${toQuery({ period })}`,
    );
  }
}

export const dashboardService = new DashboardService();
