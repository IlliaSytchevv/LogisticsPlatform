"use client";

import { FreittyShell } from "@/components/freitty/freitty-shell";
import { usePathname } from "next/navigation";
import "../freitty-detail.css";

export default function CabinetLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const showNewOrder = pathname === "/dashboard";

  return <FreittyShell showNewOrder={showNewOrder}>{children}</FreittyShell>;
}
