import { useState, useEffect, useCallback } from "react";
import { useAuth } from "../../context/AuthContext";
import { getMyNotifications, markNotificationAsRead } from "../../api/clientApi";
import NewApplicationModal from "./NewApplicationModal";
import ApplicationStatusModal from "./ApplicationStatusModal";
import "./Notifications.css";

const NOTIFICATION_TYPE = {
  WELCOME: 0,
  NEW_APPLICATION: 1,
  APPLICATION_ACCEPTED: 2,
  APPLICATION_REJECTED: 3,
};

function formatDate(dateStr) {
  if (!dateStr) return "";
  return new Date(dateStr).toLocaleString("sr-RS");
}

function typeLabel(type) {
  switch (type) {
    case 0: return "Dobrodošlica";
    case 1: return "Nova prijava";
    case 2: return "Prijava prihvaćena";
    case 3: return "Prijava odbijena";
    default: return "Obaveštenje";
  }
}

export default function Notifications() {
  const { user } = useAuth();
  const token = user?.accessToken;
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [detailNotification, setDetailNotification] = useState(null);

  const fetchNotifications = useCallback(async () => {
    if (!token) return;
    try {
      setLoading(true);
      setError(null);
      const data = await getMyNotifications(token);
      setNotifications(data);
    } catch (err) {
      setError(err.message || "Nije moguće učitati obaveštenja.");
    } finally {
      setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    fetchNotifications();
  }, [fetchNotifications]);

  const handleMarkAsRead = async (notificationId) => {
    if (!token) return;
    try {
      await markNotificationAsRead(notificationId, token);
      setNotifications((prev) =>
        prev.map((n) => (n.id === notificationId ? { ...n, isRead: true } : n))
      );
    } catch (err) {
      console.error("Greška pri označavanju obaveštenja:", err.message);
    }
  };

  const handleShowDetails = (e, notification) => {
    e.stopPropagation();
    if (!notification.isRead) {
      handleMarkAsRead(notification.id);
    }
    setDetailNotification(notification);
  };

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  return (
    <div className="notifications-container">
      <h2>
        Obaveštenja
        {unreadCount > 0 && (
          <span className="notifications-badge">{unreadCount} novo</span>
        )}
      </h2>

      {loading && <p className="notifications-loading">Učitavanje...</p>}
      {error && <p className="notifications-error">{error}</p>}

      {!loading && notifications.length === 0 && !error && (
        <p className="notifications-empty">Nemate obaveštenja.</p>
      )}

      <ul className="notifications-list">
        {notifications.map((n) => (
          <li
            key={n.id}
            className={`notification-item${n.isRead ? " read" : " unread"}`}
            onClick={() => !n.isRead && handleMarkAsRead(n.id)}
            title={n.isRead ? "" : "Kliknite da označite kao pročitano"}
            style={{ cursor: n.isRead ? "default" : "pointer" }}
          >
            <div className="notification-header">
              <span className="notification-type">{typeLabel(n.type)}</span>
              <span className="notification-date">{formatDate(n.createdAt)}</span>
              {!n.isRead && <span className="notification-dot" />}
            </div>
            <p className="notification-title">{n.title}</p>
            <p className="notification-message">{n.message}</p>

            {n.type === NOTIFICATION_TYPE.NEW_APPLICATION && n.candidateId && n.advertisementId && (
              <button
                type="button"
                className="notification-action-btn"
                onClick={(e) => handleShowDetails(e, n)}
              >
                Prikaži prijavu
              </button>
            )}

            {(n.type === NOTIFICATION_TYPE.APPLICATION_ACCEPTED ||
              n.type === NOTIFICATION_TYPE.APPLICATION_REJECTED) &&
              n.companyId &&
              n.advertisementId && (
                <button
                  type="button"
                  className="notification-action-btn"
                  onClick={(e) => handleShowDetails(e, n)}
                >
                  Detaljnije
                </button>
              )}
          </li>
        ))}
      </ul>

      {detailNotification?.type === NOTIFICATION_TYPE.NEW_APPLICATION && (
        <NewApplicationModal
          candidateId={detailNotification.candidateId}
          advertisementId={detailNotification.advertisementId}
          onClose={() => setDetailNotification(null)}
        />
      )}

      {(detailNotification?.type === NOTIFICATION_TYPE.APPLICATION_ACCEPTED ||
        detailNotification?.type === NOTIFICATION_TYPE.APPLICATION_REJECTED) && (
        <ApplicationStatusModal
          companyId={detailNotification.companyId}
          advertisementId={detailNotification.advertisementId}
          onClose={() => setDetailNotification(null)}
        />
      )}
    </div>
  );
}
