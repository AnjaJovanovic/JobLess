import { Outlet } from "react-router-dom";
import Header from "../header/Header";
import Footer from "../footer/Footer";

export default function Layout() {
  return (
    <>

      <main style={{ minHeight: "100vh" }}>
        <Outlet />
      </main>

    </>
  );
}
