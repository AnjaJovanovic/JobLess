import { createContext, useCallback, useContext, useEffect, useState } from "react";
import { useAuth } from "./AuthContext";
import {
  getMyNotifications,
  markNotificationAsRead as apiMarkNotificationAsRead,
} from "../api/clientApi";

const NotificationContext = createContext();

const POLL_INTERVAL_MS = 15000;

export function NotificationProvider({ children }) {
  const { user } = useAuth();
  const token = user?.accessToken;
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const refresh = useCallback(async () => {
    if (!token) {
      setNotifications([]);
      setUnreadCount(0);
      return;
    }
    try {
      setLoading(true);
      setError(null);
      const data = await getMyNotifications(token);
      setNotifications(data);
      setUnreadCount(data.filter((n) => !n.isRead).length);
    } catch (err) {
      setError(err.message || "Nije moguće učitati obaveštenja.");
    } finally {
      setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    refresh();
  }, [refresh]);

  // "Real-time" efekat: bez SignalR-a/websocket-a, badge sa brojem
  // nepročitanih se osvežava periodičnim pozivom postojećeg REST endpointa,
  // pa se ažurira sam i bez ulaska korisnika u Obaveštenja.
  useEffect(() => {
    if (!token) return undefined;

    const intervalId = setInterval(() => {
      getMyNotifications(token)
        .then((data) => {
          setNotifications(data);
          setUnreadCount(data.filter((n) => !n.isRead).length);
        })
        .catch(() => {
          // tiha greška - sledeći ciklus pollinga ce probati ponovo
        });
    }, POLL_INTERVAL_MS);

    return () => clearInterval(intervalId);
  }, [token]);

  const markAsRead = useCallback(
    async (notificationId) => {
      if (!token) return;

      const target = notifications.find((n) => n.id === notificationId);
      if (!target || target.isRead) return;

      await apiMarkNotificationAsRead(notificationId, token);
      setNotifications((prev) =>
        prev.map((n) => (n.id === notificationId ? { ...n, isRead: true } : n))
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    },
    [token, notifications]
  );

  return (
    <NotificationContext.Provider
      value={{ notifications, unreadCount, loading, error, refresh, markAsRead }}
    >
      {children}
    </NotificationContext.Provider>
  );
}

export function useNotifications() {
  return useContext(NotificationContext);
}
