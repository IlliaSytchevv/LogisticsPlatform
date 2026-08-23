import { BaseService } from "@/api/services/base.service";
import type { DashboardMetrics } from "@/types/dashboard";

class DashboardService extends BaseService {
  metrics() {
    return this.get<DashboardMetrics>("/api/dashboard/metrics");
  }
}

export const dashboardService = new DashboardService();
