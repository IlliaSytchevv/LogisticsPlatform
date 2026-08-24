"use client";

import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersService } from "@/api/services/orders.service";
import type { OrderOperationType, PalletUnit } from "@/types/orders";
import { ordersKeys } from "../../_hooks/orders-queries";
import { OPERATION_TYPE_OPTIONS, PALLET_UNIT_OPTIONS } from "../_lib/format";
import { DetailModal } from "./detail-modal";

type Props = {
  orderId: string;
  defaultTrailer?: string | null;
  open: boolean;
  onClose: () => void;
};

export function AddOperationModal({ orderId, defaultTrailer, open, onClose }: Props) {
  const queryClient = useQueryClient();
  const [type, setType] = useState<OrderOperationType>(1);
  const [trailer, setTrailer] = useState(defaultTrailer ?? "");
  const [quantity, setQuantity] = useState("1");
  const [unit, setUnit] = useState<PalletUnit>(1);
  const [unitLabel, setUnitLabel] = useState("Standard · 48×40");
  const [error, setError] = useState<string | null>(null);

  const addMutation = useMutation({
    mutationFn: () =>
      ordersService.addOperation(orderId, {
        type,
        trailer: trailer || null,
        quantity: Number(quantity) || 0,
        unit,
        unitLabel: unitLabel || null,
        appliedAt: new Date().toISOString(),
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
      onClose();
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Failed to add operation"),
  });

  return (
    <DetailModal open={open} title="Add operation" onClose={onClose}>
      <label style={{ display: "block", marginBottom: 10 }}>
        <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>Type</div>
        <select
          value={type}
          onChange={(e) => setType(Number(e.target.value) as OrderOperationType)}
          style={{ width: "100%" }}
        >
          {OPERATION_TYPE_OPTIONS.map((o) => (
            <option key={o.value} value={o.value}>
              {o.label}
            </option>
          ))}
        </select>
      </label>
      <label style={{ display: "block", marginBottom: 10 }}>
        <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>Trailer</div>
        <input value={trailer} onChange={(e) => setTrailer(e.target.value)} style={{ width: "100%" }} />
      </label>
      <label style={{ display: "block", marginBottom: 10 }}>
        <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>Quantity</div>
        <input
          type="number"
          value={quantity}
          onChange={(e) => setQuantity(e.target.value)}
          style={{ width: "100%" }}
        />
      </label>
      <label style={{ display: "block", marginBottom: 10 }}>
        <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>Unit</div>
        <select
          value={unit}
          onChange={(e) => {
            const next = Number(e.target.value) as PalletUnit;
            setUnit(next);
            setUnitLabel(PALLET_UNIT_OPTIONS.find((o) => o.value === next)?.label ?? "");
          }}
          style={{ width: "100%" }}
        >
          {PALLET_UNIT_OPTIONS.map((o) => (
            <option key={o.value} value={o.value}>
              {o.label}
            </option>
          ))}
        </select>
      </label>
      {error && <div style={{ color: "#DC2626", fontSize: 12, marginBottom: 8 }}>{error}</div>}
      <button
        type="button"
        className="btn btn-primary"
        disabled={addMutation.isPending}
        onClick={() => addMutation.mutate()}
      >
        {addMutation.isPending ? "Saving…" : "Add operation"}
      </button>
    </DetailModal>
  );
}
