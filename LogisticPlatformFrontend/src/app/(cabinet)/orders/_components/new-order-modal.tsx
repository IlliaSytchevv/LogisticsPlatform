"use client";

import { useMemo, useState, type CSSProperties } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { FoeSupplyPicker, picksToLines } from "@/components/freitty/foe-supply-picker";
import { ordersService } from "@/api/services/orders.service";
import { suppliesService } from "@/api/services/supplies.service";
import type { OrderType } from "@/types/orders";
import { ordersFilterOptionsQuery, ordersKeys } from "../_hooks/orders-queries";

type Props = {
  open: boolean;
  onClose: () => void;
};

type Step = 1 | 2;

export function NewOrderModal({ open, onClose }: Props) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { data: options } = useQuery(ordersFilterOptionsQuery());
  const {
    data: catalog,
    isLoading: catalogLoading,
    error: catalogError,
  } = useQuery({
    queryKey: ["supplies", "catalog"],
    queryFn: () => suppliesService.catalog(),
    enabled: open,
    staleTime: 60_000,
  });

  const [step, setStep] = useState<Step>(1);
  const [type, setType] = useState<OrderType>(1);
  const [hubId, setHubId] = useState("");
  const [primaryReference, setPrimaryReference] = useState("");
  const [destinationCity, setDestinationCity] = useState("");
  const [destinationRegion, setDestinationRegion] = useState("ON");
  const [builderDelegation, setBuilderDelegation] = useState(false);
  const [picks, setPicks] = useState<Record<string, number>>({});
  const [error, setError] = useState<string | null>(null);

  const hubs = options?.hubs ?? [];
  const effectiveHubId = hubId || hubs[0]?.id || "";

  const showCargoStep = type === 1 || builderDelegation;

  const createMutation = useMutation({
    mutationFn: () =>
      ordersService.create({
        type,
        hubId: effectiveHubId,
        primaryReference: primaryReference || null,
        destinationCity: destinationCity || null,
        destinationRegion: destinationRegion || null,
        scheduledAt: new Date().toISOString(),
        supplies: showCargoStep ? picksToLines(picks) : [],
      }),
    onSuccess: async (created) => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.all });
      resetAndClose();
      router.push(`/orders/${created.id}`);
    },
    onError: (err) => {
      setError(err instanceof Error ? err.message : "Failed to create order");
    },
  });

  function resetAndClose() {
    setStep(1);
    setPicks({});
    setError(null);
    setBuilderDelegation(false);
    onClose();
  }

  const selectedCount = useMemo(
    () => Object.values(picks).filter((q) => q > 0).length,
    [picks],
  );

  if (!open) return null;

  return (
    <div
      className="no-print"
      style={{
        position: "fixed",
        inset: 0,
        background: "rgba(15, 23, 42, 0.45)",
        display: "grid",
        placeItems: "center",
        zIndex: 50,
        padding: 16,
      }}
      onClick={resetAndClose}
    >
      <div
        className="order-card"
        style={{ width: "min(560px, 100%)", cursor: "default", margin: 0, maxHeight: "90vh", overflow: "auto" }}
        onClick={(e) => e.stopPropagation()}
      >
        <div style={{ fontWeight: 700, fontSize: 16, marginBottom: 6 }}>New Order</div>
        <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 14 }}>
          {step === 1 ? "Step 1 · Basics" : "Step 2 · Cargo (FOE supply picker)"}
        </div>

        {step === 1 ? (
          <>
            <label style={{ display: "block", fontSize: 12, color: "#6B7280", marginBottom: 4 }}>
              Type
            </label>
            <select
              value={type}
              onChange={(e) => setType(Number(e.target.value) as OrderType)}
              style={{ width: "100%", marginBottom: 12 }}
            >
              <option value={1}>Cross-Dock</option>
              <option value={2}>Consolidation</option>
            </select>

            <label style={{ display: "block", fontSize: 12, color: "#6B7280", marginBottom: 4 }}>
              Hub
            </label>
            <select
              value={effectiveHubId}
              onChange={(e) => setHubId(e.target.value)}
              style={{ width: "100%", marginBottom: 12 }}
            >
              {hubs.map((h) => (
                <option key={h.id} value={h.id}>
                  {h.name}
                </option>
              ))}
            </select>

            <label style={{ display: "block", fontSize: 12, color: "#6B7280", marginBottom: 4 }}>
              Destination city (optional)
            </label>
            <input
              value={destinationCity}
              onChange={(e) => setDestinationCity(e.target.value)}
              placeholder="Toronto"
              style={inputStyle}
            />

            <label style={{ display: "block", fontSize: 12, color: "#6B7280", marginBottom: 4 }}>
              Region (optional)
            </label>
            <input
              value={destinationRegion}
              onChange={(e) => setDestinationRegion(e.target.value)}
              placeholder="ON"
              style={inputStyle}
            />

            <label style={{ display: "block", fontSize: 12, color: "#6B7280", marginBottom: 4 }}>
              Primary reference (optional)
            </label>
            <input
              value={primaryReference}
              onChange={(e) => setPrimaryReference(e.target.value)}
              placeholder="REF-…"
              style={inputStyle}
            />

            <label
              style={{
                display: "flex",
                alignItems: "center",
                gap: 8,
                fontSize: 12,
                color: "#374151",
                marginBottom: 12,
                cursor: "pointer",
              }}
            >
              <input
                type="checkbox"
                checked={builderDelegation}
                onChange={(e) => setBuilderDelegation(e.target.checked)}
              />
              Builder delegation mode (enable FOE Cargo step for Consolidation)
            </label>

            {!showCargoStep ? (
              <p style={{ fontSize: 11, color: "#6B7280", marginBottom: 12 }}>
                Cross-Dock always includes Cargo step. For Consolidation, turn on Builder delegation
                to pick FOE supplies now.
              </p>
            ) : null}
          </>
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

        {error ? (
          <p style={{ color: "#DC2626", fontSize: 12, marginTop: 10, marginBottom: 0 }}>{error}</p>
        ) : null}

        <div style={{ display: "flex", gap: 8, justifyContent: "flex-end", marginTop: 16 }}>
          <button type="button" className="btn btn-secondary" onClick={resetAndClose}>
            Cancel
          </button>
          {step === 2 ? (
            <button type="button" className="btn btn-secondary" onClick={() => setStep(1)}>
              ← Back
            </button>
          ) : null}
          {step === 1 && showCargoStep ? (
            <button
              type="button"
              className="btn btn-primary"
              disabled={!effectiveHubId}
              onClick={() => {
                setError(null);
                setStep(2);
              }}
            >
              Next: Cargo →
            </button>
          ) : (
            <button
              type="button"
              className="btn btn-primary"
              disabled={!effectiveHubId || createMutation.isPending}
              onClick={() => {
                setError(null);
                createMutation.mutate();
              }}
            >
              {createMutation.isPending
                ? "Creating…"
                : step === 2
                  ? `Create draft${selectedCount ? ` · ${selectedCount} SKU` : ""}`
                  : "Create draft"}
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

const inputStyle: CSSProperties = {
  width: "100%",
  marginBottom: 12,
  padding: "8px 10px",
  border: "1px solid #E5E7EB",
  borderRadius: 8,
};
