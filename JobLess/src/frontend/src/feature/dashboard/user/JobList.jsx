import { useState, useEffect, useCallback } from "react";
import { useAuth } from "../../../context/AuthContext";
import {
  applyForJob,
  getClientProfileByEmail,
  getMyJobApplications,
  storeClientId,
} from "../../../api/clientApi";



const WORK_TYPE_OPTIONS = [
  { value: "", label: "Tip rada" },
  { value: 0, label: "U kancelaciji" },
  { value: 1, label: "Rad od kuće" },
  { value: 2, label: "Hibridno" },
];

const SENIORITY_OPTIONS = [
  { value: "", label: "Nivo iskustva" },
  { value: 0, label: "Početnik" },
  { value: 1, label: "Junior" },
  { value: 2, label: "Medior" },
  { value: 3, label: "Senior" },
];

const EMPLOYMENT_TYPE_OPTIONS = [
  { value: "", label: "Vrsta zaposlenja" },
  { value: 0, label: "Stalno" },
  { value: 1, label: "Ugovor" },
  { value: 2, label: "Praksa" },
];

const WORK_SCHEDULE_OPTIONS = [
  { value: "", label: "Tip rada" },
  { value: 0, label: "Puno radno vreme" },
  { value: 1, label: "Skraćeno radno vreme" },
];


const getWorkTypeLabel = (val) =>
  WORK_TYPE_OPTIONS.find((o) => o.value === val)?.label ?? "Nepoznato";

const getSeniorityLabel = (val) =>
  SENIORITY_OPTIONS.find((o) => o.value === val)?.label ?? "Nepoznato";

const getEmploymentTypeLabel = (val) =>
  EMPLOYMENT_TYPE_OPTIONS.find((o) => o.value === val)?.label ?? "Nepoznato";

const getWorkScheduleLabel = (val) =>
  WORK_SCHEDULE_OPTIONS.find((o) => o.value === val)?.label ?? "Nepoznato";



const DEFAULT_FILTERS = {
  title: "",
  city: "",
  country: "",
  workType: "",
  seniorityLevel: "",
  employmentType: "",
  workSchedule: "",
  minExperience: "",
  maxExperience: "",
  salaryFrom: "",
  salaryTo: "",
  currency: "",
};

function hasActiveFilters(filters) {
  return Object.values(filters).some((v) => v !== "" && v !== null && v !== undefined);
}

function buildQueryString(filters, page, pageSize) {
  const params = new URLSearchParams();
  params.set("pageNumber", page);
  params.set("pageSize", pageSize);
  Object.entries(filters).forEach(([key, value]) => {
    if (value !== "" && value !== null && value !== undefined) {
      params.set(key, value);
    }
  });
  return params.toString();
}



export default function JobList() {
  const { user } = useAuth();
  const email = user?.email ?? "";
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState(DEFAULT_FILTERS);
  const [appliedFilters, setAppliedFilters] = useState(DEFAULT_FILTERS);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [showAdvanced, setShowAdvanced] = useState(false);
  const [candidateProfile, setCandidateProfile] = useState(null);
  const [appliedAdvertisementIds, setAppliedAdvertisementIds] = useState(() => new Set());
  const [applyLoadingId, setApplyLoadingId] = useState(null);
  const [applyMessage, setApplyMessage] = useState("");
  const [applyError, setApplyError] = useState("");

  const PAGE_SIZE = 10;

  useEffect(() => {
    let cancelled = false;

    async function resolveClientId() {
      if (!email) {
        if (!cancelled) setCandidateProfile(null);
        return;
      }

      try {
        const profile = await getClientProfileByEmail(email);
        if (cancelled) return;

        if (profile?.clientId) {
          storeClientId(profile.clientId);
          setCandidateProfile(profile);
        } else {
          setCandidateProfile(null);
        }
      } catch {
        if (!cancelled) setCandidateProfile(null);
      }
    }

    resolveClientId();
    return () => { cancelled = true; };
  }, [email]);

  useEffect(() => {
    let cancelled = false;

    async function loadAppliedJobs() {
      if (!user?.accessToken) {
        if (!cancelled) setAppliedAdvertisementIds(new Set());
        return;
      }

      try {
        const applications = await getMyJobApplications(user.accessToken);
        if (cancelled) return;

        const ids = new Set(
          applications.map((item) => Number(item.advertisementId)).filter(Boolean)
        );
        setAppliedAdvertisementIds(ids);
      } catch {
        if (!cancelled) setAppliedAdvertisementIds(new Set());
      }
    }

    loadAppliedJobs();
    return () => { cancelled = true; };
  }, [user?.accessToken]);

  const handleApply = async (job) => {
    if (!user?.accessToken) {
      setApplyError("Morate biti prijavljeni da biste se prijavili na oglas.");
      return;
    }

    if (!candidateProfile) {
      setApplyError("Profil kandidata nije pronađen. Prvo sačuvajte profil.");
      return;
    }

    const companyId = job.companyId ?? job.CompanyId;

    if (!companyId) {
      setApplyError("Nedostaju podaci o kompaniji za ovaj oglas.");
      return;
    }

    try {
      setApplyLoadingId(job.id);
      setApplyError("");
      setApplyMessage("");

      await applyForJob({
        advertisementId: Number(job.id),
        companyId: Number(companyId),
      }, user.accessToken);

      setAppliedAdvertisementIds((prev) => new Set(prev).add(Number(job.id)));
      setApplyMessage(`Uspešno ste se prijavili na oglas #${job.id}.`);
    } catch (err) {
      setApplyError(err.message || "Prijava nije uspela.");
    } finally {
      setApplyLoadingId(null);
    }
  };

  const fetchJobs = useCallback(async (activeFilters, activePage) => {
    try {
      setLoading(true);
      setError(null);

      const qs = buildQueryString(activeFilters, activePage, PAGE_SIZE);

      const endpoint = hasActiveFilters(activeFilters)
        ? `/api/Advertisements/Search?${qs}`
        : `/api/Advertisements/All?${qs}`;

      const response = await fetch(endpoint, {
        headers: { Authorization: `Bearer ${user?.accessToken}` },
      });
      if (!response.ok) {
        const body = await response.text();
        let message = "Greška pri učitavanju oglasa.";
        try {
          const parsed = JSON.parse(body);
          if (parsed.errors?.length) message = parsed.errors.join(" ");
        } catch {
          if (body) message = body;
        }
        throw new Error(message);
      }

      const data = await response.json();
      const ads = data.advertisements ?? data.Advertisements ?? [];

      setJobs(
        (Array.isArray(ads) ? ads : []).map((job) => ({
          ...job,
          id: job.id ?? job.Id,
        }))
      );
      setTotalPages(data.totalPages ?? data.TotalPages ?? 1);
      setTotalCount(data.totalCount ?? data.TotalCount ?? 0);
    } catch (err) {
      setError(err.message || "Nije moguće učitati oglase. Proverite da li Advertisement servis radi (port 5104).");
    } finally {
      setLoading(false);
    }
  }, [user]);

  useEffect(() => {
    fetchJobs(appliedFilters, page);
  }, [appliedFilters, page, fetchJobs]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  const applyFilters = () => {
    setPage(1);
    setAppliedFilters({ ...filters });
  };

  const resetFilters = () => {
    setFilters(DEFAULT_FILTERS);
    setAppliedFilters(DEFAULT_FILTERS);
    setPage(1);
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") applyFilters();
  };

  return (
    <>
      <h2>
        Oglasi za posao
        {!loading && <span className="jobs-count">{totalCount} ukupno</span>}
      </h2>

      {/* FILTER PANEL */}
      <div className="joblist-panel">

        <div className="joblist-row">
          <input
            className="job-input"
            type="text"
            name="title"
            placeholder="Naziv pozicije"
            value={filters.title}
            onChange={handleChange}
            onKeyDown={handleKeyDown}
          />

          <input
            className="job-input"
            type="text"
            name="city"
            placeholder="Grad"
            value={filters.city}
            onChange={handleChange}
            onKeyDown={handleKeyDown}
          />

          <input
            className="job-input"
            type="text"
            name="country"
            placeholder="Država"
            value={filters.country}
            onChange={handleChange}
            onKeyDown={handleKeyDown}
          />
        </div>

        <div className="joblist-row">
          <select className="job-select" name="workType" value={filters.workType} onChange={handleChange}>
            {WORK_TYPE_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>

          <select className="job-select" name="seniorityLevel" value={filters.seniorityLevel} onChange={handleChange}>
            {SENIORITY_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>

          <select className="job-select" name="employmentType" value={filters.employmentType} onChange={handleChange}>
            {EMPLOYMENT_TYPE_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>

          <select className="job-select" name="workSchedule" value={filters.workSchedule} onChange={handleChange}>
            {WORK_SCHEDULE_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>

          <button className="btn-ghost" onClick={() => setShowAdvanced((v) => !v)}>
            {showAdvanced ? "Manje ▲" : "Više ▼"}
          </button>
        </div>

        {showAdvanced && (
          <div className="joblist-row">
            <input className="job-input small" type="number" name="minExperience" placeholder="Min iskustvo" value={filters.minExperience} onChange={handleChange}/>
            <input className="job-input small" type="number" name="maxExperience" placeholder="Max iskustvo" value={filters.maxExperience} onChange={handleChange}/>
            <input className="job-input small" type="number" name="salaryFrom" placeholder="Plata od" value={filters.salaryFrom} onChange={handleChange}/>
            <input className="job-input small" type="number" name="salaryTo" placeholder="Plata do" value={filters.salaryTo} onChange={handleChange}/>
            <input className="job-input small" type="text" name="currency" placeholder="Valuta" value={filters.currency} onChange={handleChange}/>
          </div>
        )}

        <div className="joblist-actions">
          <button className="btn-primary" onClick={applyFilters}>Pretraži</button>
          <button className="btn-secondary" onClick={resetFilters}>Resetuj</button>
        </div>

      </div>

      {/* STANJA */}
      {loading && <p className="job-empty">Učitavanje...</p>}
      {error && <p className="job-error">{error}</p>}
      {!error && applyError && <p className="job-error">{applyError}</p>}
      {!error && applyMessage && <p className="job-apply-message">{applyMessage}</p>}
      {!loading && jobs.length === 0 && !error && (
        <p className="job-empty">Trenutno nema aktivnih oglasa.</p>
      )}

      {/* KARTICE */}
      {!loading &&
        jobs.map((job) => {
          const jobId = Number(job.id);
          const isApplied = appliedAdvertisementIds.has(jobId);
          const isApplying = applyLoadingId === job.id;

          return (
          <div key={job.id} className="job-card-full">
            <div>
              <div className="jcf-header">
                <h4>{job.title}</h4>
                {job.position && <span className="jcf-position">{job.position}</span>}
              </div>

              {job.description && (
                <p className="jcf-description">
                  {job.description.length > 200
                    ? `${job.description.slice(0, 200)}...`
                    : job.description}
                </p>
              )}

              <div className="jcf-meta">
                <span className="jcf-meta-item">📍 {job.city}{job.country ? `, ${job.country}` : ""}</span>
                <span className="jcf-meta-item">{getWorkTypeLabel(job.workType)}</span>
                <span className="jcf-meta-item">{getSeniorityLabel(job.seniorityLevel)}</span>
                <span className="jcf-meta-item">{getEmploymentTypeLabel(job.employmentType)}</span>
              </div>

              {job.isSalaryVisible && job.salaryFrom > 0 && (
                <div className="jcf-salary">
                  💰 {job.salaryFrom?.toLocaleString("sr-RS")} – {job.salaryTo?.toLocaleString("sr-RS")} {job.currency}
                </div>
              )}

              <div className="jcf-expire">
                Objavljeno: {job.postedAt ? new Date(job.postedAt).toLocaleDateString("sr-RS") : "—"}
                {" · "}
                Važi do: {job.expiresAt ? new Date(job.expiresAt).toLocaleDateString("sr-RS") : "Nepoznato"}
              </div>
            </div>

            <div className="jcf-actions">
              <button
                className={`btn-apply${isApplied ? " btn-applied" : ""}`}
                type="button"
                onClick={() => handleApply(job)}
                disabled={isApplied || isApplying}
              >
                {isApplying ? "Prijava..." : isApplied ? "Prijavljeni ste" : "Prijavi se"}
              </button>
            </div>
          </div>
          );
        })}

      {/* PAGINACIJA */}
      {!loading && totalPages > 1 && (
        <div className="job-pagination">
          <button onClick={() => setPage((p) => p - 1)} disabled={page === 1}>
            ← Prethodna
          </button>
          <span>Strana {page} od {totalPages}</span>
          <button onClick={() => setPage((p) => p + 1)} disabled={page === totalPages}>
            Sledeća →
          </button>
        </div>
      )}
    </>
  );
}
