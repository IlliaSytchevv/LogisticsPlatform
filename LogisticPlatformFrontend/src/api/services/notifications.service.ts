import { BaseService } from "@/api/services/base.service";
import { toQuery } from "@/api/fetcher";
import type { NotificationsFeed } from "@/types/notifications";

class NotificationsService extends BaseService {
  feed(days = 7, take = 20) {
    return this.get<NotificationsFeed>(
      `/api/notifications/feed${toQuery({ days, take })}`,
    );
  }
}

export const notificationsService = new NotificationsService();
