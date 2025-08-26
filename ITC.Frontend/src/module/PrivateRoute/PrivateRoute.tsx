import type { FC, ReactElement } from "react";
import React from "react";
import { Navigate } from "react-router-dom";

import { useAppSelector, isUserAuthenticatedSelector } from "../../store";
import { RoutePaths } from "../../types";

export const PrivateRoute: FC<{ children: ReactElement }> = ({ children }) => {
  const isUserAuthenticated = useAppSelector(isUserAuthenticatedSelector);

  return isUserAuthenticated ? children : <Navigate to={RoutePaths.Auth} />;
};
