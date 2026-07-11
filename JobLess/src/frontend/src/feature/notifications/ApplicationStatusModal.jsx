import { useEffect, useState } from "react";
import { getCompanyById, getAdvertisementById } from "../../api/clientApi";

export default function ApplicationStatusModal({ companyId, advertisementId, onClose }) {
  const [company, setCompany] = useState(null);
  const [advertisement, setAdvertisement] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);

      try {
        const [companyInfo, ad] = await Promise.all([
          companyId ? getCompanyById(companyId) : Promise.resolve(null),
          advertisementId ? getAdvertisementById(advertisementId) : Promise.resolve(null),
        ]);

        if (cancelled) return;

        if (!companyInfo && !ad) {
          setError("Podaci o prijavi nisu pronađeni.");
        }

        setCompany(companyInfo);
        setAdvertisement(ad);
      } catch (err) {
        if (!cancelled) {
          setError(err.message || "Greška pri učitavanju podataka o prijavi.");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => { cancelled = true; };
  }, [companyId, advertisementId]);

  return (
    <div
      className="modal-overlay"
      onClick={(e) => e.target === e.currentTarget && onClose()}
      role="presentation"
    >
      <div className="modal-box notification-detail-modal" role="dialog" aria-modal="true">
        <div className="modal-header">
          <h3>Detalji prijave</h3>
          <button type="button" className="modal-close" onClick={onClose} aria-label="Zatvori">
            ✕
          </button>
        </div>

        <div className="notification-detail-body">
          {loading && <p className="notification-detail-loading">Učitavanje...</p>}
          {error && <div className="notification-detail-error" role="alert">{error}</div>}

          {!loading && advertisement && (
            <section className="notification-detail-section">
              <h4>Oglas</h4>
              <p className="notification-detail-highlight">{advertisement.title}</p>
              <p className="notification-detail-sub">
                {advertisement.position}
                {advertisement.city ? ` · ${advertisement.city}` : ""}
                {advertisement.country ? `, ${advertisement.country}` : ""}
              </p>
            </section>
          )}

          {!loading && company && (
            <section className="notification-detail-section">
              <h4>Kompanija</h4>
              <dl className="notification-detail-list">
                <div className="notification-detail-row">
                  <dt>Naziv</dt>
                  <dd>{company.name || "—"}</dd>
                </div>
                <div className="notification-detail-row">
                  <dt>Email</dt>
                  <dd>{company.email || "—"}</dd>
                </div>
                <div className="notification-detail-row">
                  <dt>Lokacija</dt>
                  <dd>{company.location || "—"}</dd>
                </div>
                <div className="notification-detail-row">
                  <dt>Web sajt</dt>
                  <dd>{company.website || "—"}</dd>
                </div>
              </dl>
            </section>
          )}
        </div>
      </div>
    </div>
  );
}
