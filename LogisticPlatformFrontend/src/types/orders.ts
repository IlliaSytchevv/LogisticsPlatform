export type OrderType = 1 | 2;
export type OrderStatus = 1 | 2 | 3 | 4 | 5 | 6;

/** OrderListTab: 1=All, 2=CrossDock, 3=Consolidation, 4=Alerts, 5=Drafts */
export type OrderListTab = 1 | 2 | 3 | 4 | 5;

export type OrdersListParams = {
  tab?: OrderListTab | number;
  hubId?: string;
  dateFrom?: string;
  dateTo?: string;
  status?: OrderStatus | number | string;
  search?: string;
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

export type CreateOrderSupplyLine = {
  catalogItemId: string;
  quantity: number;
};

export type CreateOrderRequest = {
  type: OrderType;
  hubId: string;
  scheduledAt?: string | null;
  destinationCity?: string | null;
  destinationRegion?: string | null;
  primaryReference?: string | null;
  /** FOE catalog picks (Cargo / Builder delegation). Platform price applied on server. */
  supplies?: CreateOrderSupplyLine[] | null;
};

export type CreateOrderResponse = {
  id: string;
  number: string;
  type: OrderType;
  status: OrderStatus;
};

/** OrderOperationType: 1=Unloading, 2=Disposal, 3=Restack, 4=Loading */
export type OrderOperationType = 1 | 2 | 3 | 4;

/** PalletUnit: 1=Standard, 2=XL */
export type PalletUnit = 1 | 2;

export type OrderHubDock = {
  code: string;
  bayLabel: string | null;
  isAssigned: boolean;
};

export type OrderAssignedDock = {
  hubName: string;
  dockCode: string | null;
  dockBay: string | null;
  trailerNumber: string | null;
  assignedAt: string | null;
  statusLabel: string | null;
  hubDocks: OrderHubDock[];
};

export type OrderQtyBlock = {
  quantity: number | null;
  unitLabel: string | null;
};

export type OrderPhoto = {
  id: string;
  fileName: string;
  contentType: string;
  downloadUrl: string;
};

export type OrderWarehouseNote = {
  text: string | null;
  photos: OrderPhoto[];
};

export type OrderOperation = {
  id: string;
  type: OrderOperationType;
  typeLabel: string;
  trailer: string | null;
  quantity: number;
  unit: PalletUnit;
  unitLabel: string | null;
  appliedAt: string;
  commentCount: number;
  photoCount: number;
};

export type OrderSupply = {
  id: string;
  sku: string;
  name: string;
  category: string;
  quantity: number;
  unitPriceCents: number;
  lineTotalCents: number;
};

export type OrderComment = {
  id: string;
  text: string;
  authorName: string | null;
  createdAt: string;
};

export type OrderTimelineEntry = {
  id: string;
  kind: string;
  text: string;
  authorName: string | null;
  createdAt: string;
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
  awaitingClientAction: boolean;
  assignedDock: OrderAssignedDock;
  expected: OrderQtyBlock;
  actual: OrderQtyBlock;
  qtyDelta: number;
  warehouseNote: OrderWarehouseNote;
  operations: OrderOperation[];
  supplies: OrderSupply[];
  suppliesSubtotalCents: number;
  isPaid: boolean;
};

export type UpdateOrderRequest = {
  number?: string | null;
  customerName?: string | null;
  primaryReference?: string | null;
  declaredQty?: number | null;
  actualQty?: number | null;
  trailerType?: string | null;
  phone?: string | null;
  truckNumber?: string | null;
  trailerNumber?: string | null;
  dockCode?: string | null;
  dockBay?: string | null;
  warehouseNote?: string | null;
  stockStatusLabel?: string | null;
  loadingStatusLabel?: string | null;
  status?: OrderStatus | null;
  awaitingClientAction?: boolean | null;
};

export type AddOrderOperationRequest = {
  type: OrderOperationType;
  trailer?: string | null;
  quantity: number;
  unit: PalletUnit;
  unitLabel?: string | null;
  appliedAt?: string | null;
};

export type AddOrderSupplyRequest = {
  sku: string;
  name: string;
  category: string;
  quantity: number;
  unitPriceCents: number;
};

export type AddSupplyFromCatalogRequest = {
  catalogItemId: string;
  quantity: number;
};

export type UpdateOrderSupplyRequest = AddOrderSupplyRequest;

export type AddTextRequest = {
  text: string;
};
