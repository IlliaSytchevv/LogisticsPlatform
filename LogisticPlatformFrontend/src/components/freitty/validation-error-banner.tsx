"use client";

import type { ApiValidationIssue } from "@/lib/api/errors";

type Props = {
  issues: ApiValidationIssue[] | null | undefined;
  title?: string;
};

export function ValidationErrorBanner({
  issues,
  title = "Please fix the following:",
}: Props) {
  if (!issues?.length) return null;

  return (
    <div
      role="alert"
      style={{
        marginTop: 10,
        marginBottom: 8,
        padding: "10px 12px",
        borderRadius: 8,
        background: "#FEF2F2",
        border: "1px solid #FECACA",
        color: "#991B1B",
        fontSize: 12,
      }}
    >
      <div style={{ fontWeight: 700, marginBottom: issues.length > 1 ? 6 : 0 }}>{title}</div>
      {issues.length === 1 ? (
        <div>
          {issues[0].field ? (
            <>
              <strong>{issues[0].field}</strong>: {issues[0].message}
            </>
          ) : (
            issues[0].message
          )}
        </div>
      ) : (
        <ul style={{ margin: 0, paddingLeft: 18 }}>
          {issues.map((issue, idx) => (
            <li key={`${issue.field ?? "msg"}-${idx}`} style={{ marginBottom: 2 }}>
              {issue.field ? (
                <>
                  <strong>{issue.field}</strong>: {issue.message}
                </>
              ) : (
                issue.message
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
