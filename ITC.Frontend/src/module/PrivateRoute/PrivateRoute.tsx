import type { FC, ReactElement } from "react";
import React from "react";
import { Navigate } from "react-router-dom";

import { RoutePaths } from "../../types";
import { useSelector } from "react-redux";
import { AuthSlice } from "../../store";

export const PrivateRoute: FC<{ children: ReactElement }> = ({ children }) => {
  const isUserAuthenticated = useSelector(AuthSlice.selectors.isUserAuthenticatedSelector);
 
  return isUserAuthenticated ? children : <Navigate to={RoutePaths.Auth} />;
};
