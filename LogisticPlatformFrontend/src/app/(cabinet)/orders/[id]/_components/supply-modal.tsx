"use client";

import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ordersService } from "@/api/services/orders.service";
import { suppliesService } from "@/api/services/supplies.service";
import type { OrderSupply } from "@/types/orders";
import { FoeSupplyPicker } from "@/components/freitty/foe-supply-picker";
import { ordersKeys } from "../../_hooks/orders-queries";
import { DetailModal } from "./detail-modal";

type Mode = "add" | "edit-admin" | "edit-qty";

type Props = {
  orderId: string;
  supply?: OrderSupply | null;
  /** add | edit-admin (full fields) | edit-qty (quantity only) */
  mode: Mode;
  open: boolean;
  onClose: () => void;
};

export function SupplyModal({ orderId, supply, mode, open, onClose }: Props) {
  const queryClient = useQueryClient();
  const isAdd = mode === "add";
  const isAdminEdit = mode === "edit-admin";
  const isQtyEdit = mode === "edit-qty";

  const [picks, setPicks] = useState<Record<string, number>>({});
  const [error, setError] = useState<string | null>(null);

  const [sku, setSku] = useState(supply?.sku ?? "");
  const [name, setName] = useState(supply?.name ?? "");
  const [category, setCategory] = useState(supply?.category ?? "");
  const [quantity, setQuantity] = useState(String(supply?.quantity ?? 1));
  const [unitDollars, setUnitDollars] = useState(
    supply ? String(supply.unitPriceCents / 100) : "1",
  );

  const {
    data: catalog,
    isLoading: catalogLoading,
    error: catalogError,
  } = useQuery({
    queryKey: ["supplies", "catalog"],
    queryFn: () => suppliesService.catalog(),
    enabled: open && isAdd,
    staleTime: 60_000,
  });

  const saveMutation = useMutation({
    mutationFn: async () => {
      if (isAdminEdit && supply) {
        return ordersService.updateSupply(orderId, supply.id, {
          sku,
          name: name || sku,
          category,
          quantity: Number(quantity) || 0,
          unitPriceCents: Math.round(Number(unitDollars) * 100) || 0,
        });
      }

      if (isQtyEdit && supply) {
        const qty = Number(quantity) || 0;
        if (qty < 1) throw new Error("Quantity must be at least 1.");
        return ordersService.updateSupplyQuantity(orderId, supply.id, qty);
      }

      const selected = Object.entries(picks).filter(([, qty]) => qty > 0);
      if (selected.length === 0) throw new Error("Pick at least one FOE SKU (qty > 0).");

      for (const [catalogItemId, qty] of selected) {
        await ordersService.addSupplyFromCatalog(orderId, {
          catalogItemId,
          quantity: qty,
        });
      }
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
      setPicks({});
      onClose();
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Save failed"),
  });

  const title = isAdd
    ? "FOE supply picker"
    : isQtyEdit
      ? "Change quantity"
      : "Edit supply (Admin)";

  return (
    <DetailModal open={open} title={title} onClose={onClose} width={isAdd ? 560 : 440}>
      {isAdminEdit ? (
        <>
          <label style={{ display: "block", marginBottom: 10 }}>
            <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>SKU</div>
            <input value={sku} onChange={(e) => setSku(e.target.value)} style={{ width: "100%" }} />
          </label>
          <label style={{ display: "block", marginBottom: 10 }}>
            <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>Display name</div>
            <input value={name} onChange={(e) => setName(e.target.value)} style={{ width: "100%" }} />
          </label>
          <label style={{ display: "block", marginBottom: 10 }}>
            <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>Category</div>
            <input
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              style={{ width: "100%" }}
            />
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
            <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>
              Unit $ (Platform price)
            </div>
            <input
              type="number"
              step="0.01"
              value={unitDollars}
              onChange={(e) => setUnitDollars(e.target.value)}
              style={{ width: "100%" }}
            />
          </label>
        </>
      ) : isQtyEdit ? (
        <label style={{ display: "block", marginBottom: 10 }}>
          <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>
            Quantity for {supply?.name || supply?.sku}
          </div>
          <input
            type="number"
            min={1}
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            style={{ width: "100%" }}
          />
          <p style={{ fontSize: 11, color: "#6B7280", marginTop: 8 }}>
            Unit price stays from FOE catalog (read-only).
          </p>
        </label>
      ) : (
        <FoeSupplyPicker
          items={catalog?.items ?? []}
          picks={picks}
          loading={catalogLoading}
          error={
            catalogError instanceof Error
              ? catalogError.message
              : catalogError
                ? String(catalogError)
                : null
          }
          onChange={(id, qty) => setPicks((prev) => ({ ...prev, [id]: qty }))}
        />
      )}

      {error ? <p style={{ color: "#DC2626", fontSize: 12 }}>{error}</p> : null}

      <div style={{ display: "flex", gap: 8, justifyContent: "flex-end", marginTop: 12 }}>
        <button type="button" className="btn btn-secondary" onClick={onClose}>
          Cancel
        </button>
        <button
          type="button"
          className="btn btn-primary"
          disabled={saveMutation.isPending}
          onClick={() => {
            setError(null);
            saveMutation.mutate();
          }}
        >
          {saveMutation.isPending ? "Saving…" : isAdd ? "Add selected" : "Save"}
        </button>
      </div>
    </DetailModal>
  );
}
