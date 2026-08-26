import { BaseService } from "@/api/services/base.service";
import { apiV1 } from "@/lib/api/routes";
import type { SupplyCatalog } from "@/types/supplies";

class SuppliesService extends BaseService {
  catalog() {
    return this.get<SupplyCatalog>(apiV1("/supplies/catalog"));
  }
}

export const suppliesService = new SuppliesService();
