"use client";

import { FreittyShell } from "@/components/freitty/freitty-shell";
import "../freitty-detail.css";

export default function CabinetLayout({ children }: { children: React.ReactNode }) {
  // + New Order lives on the Orders page header only — not in the shell sidebar.
  return <FreittyShell showNewOrder={false}>{children}</FreittyShell>;
}
