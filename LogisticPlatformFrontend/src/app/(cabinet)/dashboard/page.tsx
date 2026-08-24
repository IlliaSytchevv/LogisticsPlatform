"use client";

import { useEffect, useState } from "react";
import { getSessionAction } from "@/actions/auth/get-session.action";
import { MetricsKpis } from "./_components/metrics-kpis";
import { ActiveOrdersSection } from "./_components/active-orders";
import { ActivityBlock } from "./_components/activity-block";

export default function DashboardPage() {
  const [name, setName] = useState("User");

  useEffect(() => {
    void getSessionAction().then((user) => {
      if (user?.name) setName(user.name);
    });
  }, []);

  return (
    <>
      <div className="fc-crumbs">
        Home <span>›</span> Dashboard
      </div>
      <div className="fc-page-title">
        <h1>Welcome, {name} 👋</h1>
      </div>

      <MetricsKpis />
      <ActiveOrdersSection />
      <ActivityBlock />
    </>
  );
}
