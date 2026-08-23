export default function SettingsPage() {
  return (
    <>
      <div className="fc-crumbs">
        Home <span>›</span> Settings
      </div>
      <div className="fc-page-title">
        <h1>Settings</h1>
      </div>
      <div
        style={{
          background: "#F9FAFB",
          border: "2px dashed #D1D5DB",
          borderRadius: 10,
          padding: "50px 20px",
          textAlign: "center",
          color: "#6B7280",
        }}
      >
        <div style={{ fontSize: 42, opacity: 0.5 }}>⚙️</div>
        <div style={{ fontSize: 13, marginTop: 10 }}>Settings — coming from design section 20</div>
      </div>
    </>
  );
}
