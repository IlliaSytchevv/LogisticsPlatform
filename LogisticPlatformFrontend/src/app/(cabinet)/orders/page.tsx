"use client";

import { Suspense, useEffect, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useRouter, useSearchParams } from "next/navigation";
import { ordersService } from "@/api/services/orders.service";
import type { OrderListTab, OrdersListParams } from "@/types/orders";
import { useMediaQuery } from "@/hooks/use-media-query";
import { useSession } from "@/hooks/use-session";
import { NewOrderModal } from "./_components/new-order-modal";
import { OrderListCard } from "./_components/order-list-card";
import { OrdersTable } from "./_components/orders-table";
import {
  ordersFilterOptionsQuery,
  ordersListOptions,
  ordersTabCountsOptions,
} from "./_hooks/orders-queries";
import {
  dateRangeFromPreset,
  ORDER_TABS,
  type DatePreset,
} from "./_lib/format";

const PAGE_SIZE = 6;

export default function OrdersPage() {
  return (
    <Suspense fallback={<p style={{ color: "#6B7280", fontSize: 13 }}>Loading orders…</p>}>
      <OrdersPageContent />
    </Suspense>
  );
}

function OrdersPageContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const isMobile = useMediaQuery("(max-width: 1100px)");
  const { canWrite, isDriver, loading: sessionLoading } = useSession();
  const q = (searchParams.get("q") ?? "").trim();
  const [tab, setTab] = useState<OrderListTab>(1);
  const [view, setView] = useState<"cards" | "table">("cards");
  const [hubId, setHubId] = useState("");
  const [datePreset, setDatePreset] = useState<DatePreset>("all");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [newOrderOpen, setNewOrderOpen] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);

  const dateRange = useMemo(() => dateRangeFromPreset(datePreset), [datePreset]);

  useEffect(() => {
    setPage(1);
  }, [q]);

  const filterParams = useMemo(
    () => ({
      hubId: hubId || undefined,
      dateFrom: dateRange.dateFrom,
      dateTo: dateRange.dateTo,
      status: status || undefined,
      search: q || undefined,
    }),
    [hubId, dateRange.dateFrom, dateRange.dateTo, status, q],
  );

  const listParams: OrdersListParams = useMemo(
    () => ({
      ...filterParams,
      tab,
      page,
      pageSize: PAGE_SIZE,
      search: q || undefined,
    }),
    [filterParams, tab, page, q],
  );

  const {
    data: list,
    isLoading: listLoading,
    isError: listError,
    error: listErr,
  } = useQuery(ordersListOptions(listParams));

  const { data: counts } = useQuery(ordersTabCountsOptions(filterParams));
  const { data: filterOptions } = useQuery(ordersFilterOptionsQuery());

  const totalCount = list?.totalCount ?? 0;
  const items = list?.items ?? [];
  const showingFrom = totalCount === 0 ? 0 : (page - 1) * PAGE_SIZE + 1;
  const showingTo = Math.min(page * PAGE_SIZE, totalCount);
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  function resetPage() {
    setPage(1);
  }

  function clearSearch() {
    router.replace("/orders");
    setPage(1);
  }

  async function onExport() {
    setActionError(null);
    setExporting(true);
    try {
      await ordersService.exportCsv({ ...filterParams, tab });
    } catch (err) {
      setActionError(err instanceof Error ? err.message : "Export failed");
    } finally {
      setExporting(false);
    }
  }

  return (
    <>
      <div className="fc-crumbs">
        Home <span>›</span> Orders
      </div>
      <div className="fc-page-title">
        <h1>All Orders</h1>
        {q ? (
          <span
            style={{
              fontSize: 13,
              color: "#6B7280",
              fontWeight: 500,
              display: "inline-flex",
              alignItems: "center",
              gap: 8,
            }}
          >
            Search: “{q}”
            <button
              type="button"
              className="btn btn-secondary"
              style={{ padding: "2px 8px", fontSize: 11 }}
              onClick={clearSearch}
            >
              Clear ✕
            </button>
          </span>
        ) : null}
        <div style={{ marginLeft: "auto", display: "flex", gap: 8 }}>
          {!sessionLoading && (canWrite || isDriver) ? (
            <button
              type="button"
              className="btn btn-secondary"
              disabled={exporting}
              onClick={() => void onExport()}
            >
              {exporting ? "Exporting…" : "📥 Export CSV"}
            </button>
          ) : null}
          {!sessionLoading && canWrite ? (
            <button
              type="button"
              className="btn btn-primary"
              onClick={() => setNewOrderOpen(true)}
            >
              + New Order
            </button>
          ) : null}
        </div>
      </div>

      {actionError ? (
        <p style={{ color: "#DC2626", fontSize: 13, marginBottom: 12 }}>{actionError}</p>
      ) : null}

      <div className="tabs">
        {ORDER_TABS.map((t) => {
          const count = counts?.[t.countKey] ?? 0;
          return (
            <button
              key={t.id}
              type="button"
              className={`tab${tab === t.id ? " active" : ""}`}
              onClick={() => {
                setTab(t.id);
                resetPage();
              }}
            >
              {t.label}{" "}
              <span className={`count${t.alert ? " alert" : ""}`}>{count}</span>
            </button>
          );
        })}
      </div>

      <div className="filters-row">
        <select
          value={hubId || "all"}
          onChange={(e) => {
            setHubId(e.target.value === "all" ? "" : e.target.value);
            resetPage();
          }}
        >
          <option value="all">Hub: All</option>
          {(filterOptions?.hubs ?? []).map((h) => (
            <option key={h.id} value={h.id}>
              {h.name}
            </option>
          ))}
        </select>

        <select
          value={datePreset}
          onChange={(e) => {
            setDatePreset(e.target.value as DatePreset);
            resetPage();
          }}
        >
          <option value="all">Date: All</option>
          <option value="today">Date: Today</option>
          <option value="week">Date: This week</option>
          <option value="30">Date: Last 30 days</option>
        </select>

        <select
          value={status || "any"}
          onChange={(e) => {
            setStatus(e.target.value === "any" ? "" : e.target.value);
            resetPage();
          }}
        >
          <option value="any">Status: Any</option>
          {(filterOptions?.statuses ?? []).map((s) => (
            <option key={s.value} value={s.value}>
              {s.label}
            </option>
          ))}
        </select>

        {!isMobile ? (
          <div className="view-toggle">
            <button
              type="button"
              className={view === "cards" ? "active" : undefined}
              onClick={() => setView("cards")}
            >
              ⊞ Cards
            </button>
            <button
              type="button"
              className={view === "table" ? "active" : undefined}
              onClick={() => setView("table")}
            >
              ☰ Table
            </button>
          </div>
        ) : null}
      </div>

      {listLoading ? (
        <p style={{ color: "#6B7280", fontSize: 13 }}>Loading orders…</p>
      ) : listError ? (
        <p style={{ color: "#DC2626", fontSize: 13 }}>
          Failed to load orders
          {listErr instanceof Error ? `: ${listErr.message}` : ""}. Is API running on{" "}
          {process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5124"}?
        </p>
      ) : !isMobile && view === "table" ? (
        <OrdersTable items={items} />
      ) : (
        <div className="cards-grid cols-3">
          {items.length === 0 ? (
            <p style={{ color: "#6B7280", fontSize: 13, gridColumn: "1 / -1" }}>
              No orders found.
            </p>
          ) : (
            items.map((order) => <OrderListCard key={order.id} order={order} />)
          )}
        </div>
      )}

      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginTop: 20,
          fontSize: 12,
          color: "#6B7280",
        }}
      >
        <div>
          {totalCount === 0
            ? "Showing 0"
            : `Showing ${showingFrom}–${showingTo} of ${totalCount}`}
        </div>
        <div style={{ display: "flex", gap: 4 }}>
          <button
            type="button"
            className="btn btn-secondary"
            style={{ padding: "4px 10px" }}
            disabled={page <= 1}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
          >
            ← Prev
          </button>
          <button
            type="button"
            className="btn btn-secondary"
            style={{ padding: "4px 10px" }}
            disabled={page >= totalPages}
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
          >
            Next →
          </button>
        </div>
      </div>

      <NewOrderModal open={newOrderOpen} onClose={() => setNewOrderOpen(false)} />
    </>
  );
}
