export type NotificationFeedKind = "alert" | "awaiting" | string;

export type NotificationFeedItem = {
  orderId: string;
  orderNumber: string;
  kind: NotificationFeedKind;
  title: string;
  createdAt: string;
};

export type NotificationsFeed = {
  items: NotificationFeedItem[];
};
