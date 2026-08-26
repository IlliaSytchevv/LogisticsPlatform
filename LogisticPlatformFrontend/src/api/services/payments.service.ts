import { BaseService } from "@/api/services/base.service";
import { apiV1 } from "@/lib/api/routes";

export type CreateOrderCheckoutResponse = {
  paymentId: string;
  checkoutUrl: string;
  amountCents: number;
  currency: string;
};

class PaymentsService extends BaseService {
  createCheckout(orderId: string) {
    return this.post<CreateOrderCheckoutResponse>(
      apiV1(`/payments/orders/${orderId}/checkout`),
    );
  }
}

export const paymentsService = new PaymentsService();
