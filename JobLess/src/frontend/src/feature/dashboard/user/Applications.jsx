export default function Applications() {
    const applications = [
      { id: 1, company: "Tech Solutions", position: "Frontend Dev", status: "U razmatranju" },
      { id: 2, company: "DataCorp", position: "Backend Dev", status: "Odbijen" },
      { id: 3, company: "InnovateSoft", position: "React Developer", status: "Prihvaćen" },
    ];
  
    const statusClass = (status) => {
      if (status === "Odbijen") return "status-rejected";
      if (status === "Prihvaćen") return "status-accepted";
      return "status-pending";
    };
  
    return (
      <div>
        <h2>Moje prijave</h2>
  
        {applications.map((app, i) => (
          <div key={app.id} className="application-card" style={{ animationDelay: `${i * 0.06}s` }}>
            <div className="application-card-info">
              <h4>{app.position}</h4>
              <p>{app.company}</p>
            </div>
            <span className={statusClass(app.status)}>{app.status}</span>
          </div>
        ))}
      </div>
    );
  }