export type OrderType = 1 | 2;
export type OrderStatus = 1 | 2 | 3 | 4 | 5 | 6;

export type OrdersListParams = {
  /** OrderListTab: 1=All, 2=CrossDock, 3=Consolidation, 4=Alerts, 5=Drafts */
  tab?: string | number;
  hubId?: string;
  dateFrom?: string;
  dateTo?: string;
  status?: string;
  q?: string;
  page?: number;
  pageSize?: number;
};

export type OrderListItem = {
  id: string;
  number: string;
  type: OrderType;
  status: OrderStatus;
  typeLabel: string;
  statusLabel: string;
  subtitle: string;
  referenceSummary: string;
  hasAlert: boolean;
  alertReason: string | null;
  isDraftIncomplete: boolean;
  createdBy: {
    name: string;
    initials: string;
    role: number;
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
  nextAction: {
    label: string;
    kind: number | null;
    dueInSeconds: number | null;
    isAlert: boolean;
    amountCents: number | null;
    documentNumber: string | null;
  };
};

export type OrdersListResponse = {
  totalCount: number;
  page: number;
  pageSize: number;
  items: OrderListItem[];
};

export type OrdersTabCounts = {
  all: number;
  crossDock: number;
  consolidation: number;
  alerts: number;
  drafts: number;
};

export type OrdersFilterOptions = {
  hubs: { id: string; name: string }[];
  statuses: { value: string; label: string }[];
};

export type CreateOrderRequest = {
  type: OrderType;
  hubId: string;
  scheduledAt?: string | null;
  destinationCity?: string | null;
  destinationRegion?: string | null;
  primaryReference?: string | null;
};

export type CreateOrderResponse = {
  id: string;
  number: string;
  type: OrderType;
  status: OrderStatus;
};

export type OrderDetails = {
  id: string;
  number: string;
  type: OrderType;
  typeLabel: string;
  status: OrderStatus;
  statusLabel: string;
  primaryReference: string | null;
  customerName: string | null;
  phone: string | null;
  hubId: string;
  hubName: string;
  hubRegionCode: string | null;
  scheduledAt: string;
  carrierId: string | null;
  carrierName: string | null;
  trailerType: string | null;
  truckNumber: string | null;
  trailerNumber: string | null;
  assignedToUserId: string | null;
  assignedToUserName: string | null;
  services: string[];
  stockStatusLabel: string | null;
  loadingStatusLabel: string | null;
  hasAlert: boolean;
  alertReason: string | null;
  qtyDelta: number;
  suppliesSubtotalCents: number;
};
