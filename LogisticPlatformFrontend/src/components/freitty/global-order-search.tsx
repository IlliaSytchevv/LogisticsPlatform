"use client";

import Link from "next/link";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { ordersListOptions } from "@/app/(cabinet)/orders/_hooks/orders-queries";

type Props = {
  placeholder?: string;
};

export function GlobalOrderSearch({
  placeholder = "Search orders by number, ref, hub, carrier…",
}: Props) {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const rootRef = useRef<HTMLDivElement>(null);
  const urlQ = (searchParams.get("q") ?? "").trim();
  const [raw, setRaw] = useState(urlQ);
  const [debounced, setDebounced] = useState(urlQ);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setRaw(urlQ);
    setDebounced(urlQ);
  }, [urlQ]);

  useEffect(() => {
    const handle = window.setTimeout(() => setDebounced(raw.trim()), 300);
    return () => window.clearTimeout(handle);
  }, [raw]);

  useEffect(() => {
    function onDocClick(event: MouseEvent) {
      if (!rootRef.current?.contains(event.target as Node)) setOpen(false);
    }
    document.addEventListener("mousedown", onDocClick);
    return () => document.removeEventListener("mousedown", onDocClick);
  }, []);

  const enabled = debounced.length >= 2;
  const { data, isFetching } = useQuery({
    ...ordersListOptions({ search: debounced, page: 1, pageSize: 8, tab: 1 }),
    enabled,
  });

  const items = data?.items ?? [];
  const totalCount = data?.totalCount ?? 0;
  const showPanel = open && raw.trim().length >= 2;

  function goToList() {
    const q = raw.trim();
    if (!q) return;
    setOpen(false);
    router.push(`/orders?q=${encodeURIComponent(q)}`);
  }

  function clearSearch() {
    setRaw("");
    setDebounced("");
    setOpen(false);
    if (pathname.startsWith("/orders") && urlQ) {
      router.replace("/orders");
    }
  }

  return (
    <div className="fc-search" ref={rootRef} style={{ position: "relative" }}>
      <span aria-hidden>🔍</span>
      <input
        type="search"
        value={raw}
        placeholder={placeholder}
        aria-label="Search orders"
        onChange={(e) => {
          setRaw(e.target.value);
          setOpen(true);
        }}
        onFocus={() => setOpen(true)}
        onKeyDown={(e) => {
          if (e.key === "Enter") {
            e.preventDefault();
            goToList();
          }
          if (e.key === "Escape") {
            if (raw) clearSearch();
            else setOpen(false);
          }
        }}
      />
      {raw ? (
        <button
          type="button"
          className="fc-search-clear"
          aria-label="Clear search"
          title="Clear search"
          onClick={clearSearch}
        >
          ✕
        </button>
      ) : null}
      {showPanel ? (
        <div className="fc-search-results" role="listbox">
          {isFetching && items.length === 0 ? (
            <div className="fc-search-empty">Searching…</div>
          ) : items.length === 0 ? (
            <div className="fc-search-empty">No orders match “{debounced}”.</div>
          ) : (
            <>
              {items.map((order) => (
                <Link
                  key={order.id}
                  href={`/orders/${order.id}`}
                  className="fc-search-item"
                  onClick={() => setOpen(false)}
                >
                  <strong>{order.number}</strong>
                  <span>
                    {order.typeLabel} · {order.hub} · {order.statusLabel}
                  </span>
                </Link>
              ))}
              <button type="button" className="fc-search-more" onClick={goToList}>
                View all {totalCount} on Orders →
              </button>
            </>
          )}
        </div>
      ) : null}
    </div>
  );
}
