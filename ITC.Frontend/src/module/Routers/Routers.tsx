import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Details, Registry } from "../../pages";
import React from "react";
import { RoutePaths } from "../../types";
import { Auth, Navbar, PrivateRoute } from "..";
import { PageNotFoundPlaceholder } from "../../components";
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
            <PrivateRoute>
              <Details />
            </PrivateRoute>
          }
        />
        <Route
          path={RoutePaths.Registry}
          element={
            <PrivateRoute>
              <Registry />
            </PrivateRoute>
          }
        />
        <Route
          path={RoutePaths.Home}
          element={
            <PrivateRoute>
              <Registry />
            </PrivateRoute>
          }
        />
        <Route
          path={RoutePaths.All}
          element={
            <PrivateRoute>
              <PageNotFoundPlaceholder />
            </PrivateRoute>
          }
        />
        <Route path={RoutePaths.Auth} element={<Auth />} />
      </Routes>
    </Router>
  );
};
