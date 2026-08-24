import { BaseService } from "@/api/services/base.service";
import type { SupplyCatalog } from "@/types/supplies";

class SuppliesService extends BaseService {
  catalog() {
    return this.get<SupplyCatalog>("/api/supplies/catalog");
  }
}

export const suppliesService = new SuppliesService();
