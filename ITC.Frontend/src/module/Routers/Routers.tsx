import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Details, Registry } from "../../pages";
import React from "react";
import { RoutePaths } from "../../types";
import { Auth, Navbar, PrivateRoute } from "..";
import { useSelector } from "react-redux";
import { AuthSlice } from "../../store";

export const Routers: React.FC = () => {
  const isUserAuthenticated = useSelector(AuthSlice.selectors.isUserAuthenticatedSelector);

  return (
    <Router>
      {isUserAuthenticated === true && <Navbar />}
      <Routes>
        <Route
          path={`${RoutePaths.Details}/:id`}
          element={
            <Details />
          }
        />
        <Route
          path={RoutePaths.Registry}
          element={
            <Registry />

          }
        />
        <Route
          path={RoutePaths.Home}
          element={
            <Registry />
          }
        />
        <Route path={RoutePaths.All} element={<Registry />} />
      </Routes>
    </Router>
  );
};
