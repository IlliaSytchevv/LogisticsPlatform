export type DashboardMetrics = {
  activeOrders: {
    count: number;
    deltaThisWeek: number;
  };
  completedLast30Days: {
    count: number;
    vsPreviousMonth: number;
  };
  needAttention: {
    total: number;
    awaitingAction: number;
    alerts: number;
    alertSamples: { orderNumber: string; reason: string }[];
  };
};
