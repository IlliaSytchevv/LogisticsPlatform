export type ActivityPeriod = 1 | 2 | 3 | 4; // Day | CW | Month | Quarter

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

export type DashboardOrderCard = {
  id: string;
  number: string;
  type: 1 | 2; // CrossDock | Consolidation
  status: 1 | 2 | 3 | 4 | 5 | 6;
  typeLabel: string;
  statusLabel: string;
  hasAlert: boolean;
  createdBy: {
    name: string;
    initials: string;
    role: 1 | 2 | 3; // Admin | Dispatcher | Driver
  };
  references: {
    subOrderNumber: string;
    reference: string;
    description: string;
    alert: string | null;
  }[];
  hub: string;
  scheduledAt: string;
  quantityDisplay: string;
  declaredQty: number | null;
  actualQty: number | null;
  carrierDisplay: string;
  destinationDisplay: string;
  trailersConsolidated: number | null;
  nextAction: {
    label: string;
    kind: number | null;
    dueInSeconds: number | null;
    isAlert: boolean;
    amountCents: number | null;
    documentNumber: string | null;
  };
};

export type DashboardActiveOrders = {
  items: DashboardOrderCard[];
};

export type ActivitySeriesPoint = {
  label: string;
  value: number;
  valueCents: number;
};

export type DashboardActivity = {
  period: ActivityPeriod;
  completedTotal: number;
  spendCentsTotal: number;
  completedSeries: ActivitySeriesPoint[];
  spendSeries: ActivitySeriesPoint[];
  insights: {
    completedGrowthPercent: number;
    spendCentsTotal: number;
    avgSpendCentsPerOrder: number;
    bestWeekLabel: string | null;
    bestWeekPeakSpendCents: number;
  };
};
