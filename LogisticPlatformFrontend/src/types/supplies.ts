export type SupplyCatalogItem = {
  id: string;
  sku: string;
  name: string;
  category: string;
  /** Client-visible price only (cents). WP / margin never returned. */
  platformPriceCents: number;
};

export type SupplyCatalog = {
  items: SupplyCatalogItem[];
};
