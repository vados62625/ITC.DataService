import type { FC, ReactElement } from "react";
import React, { useEffect } from "react";
import { Navigate } from "react-router-dom";

import { RoutePaths } from "../../types";
import { useDispatch, useSelector } from "react-redux";
import { AuthSlice } from "../../store";
import { Auth } from "../Auth";

export const PrivateRoute: FC<{ children: ReactElement }> = ({ children }) => {
  const isUserAuthenticated = useSelector(AuthSlice.selectors.isUserAuthenticatedSelector);
  const dispatch = useDispatch()
  
  const token = localStorage.getItem('token')

  useEffect(() => {
    const myfunc = async () => {
      if(!token){
        return
      }
      const userResponse = await fetch('http://89.108.73.166:5017/api/v1/Users/AuthorizedContext', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      const userData = await userResponse.json();
      dispatch(AuthSlice.actions.replaceCurrentUser({ id: userData.id, userName: userData.name }))
    }

    myfunc()
  }, [])

  if(!token){
    return <Auth />
  }

  // return isUserAuthenticated ? children : <Navigate to={RoutePaths.Auth} />;
  return isUserAuthenticated ? children : <Auth />;
};
