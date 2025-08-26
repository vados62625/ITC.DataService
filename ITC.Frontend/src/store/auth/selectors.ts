import { RootState } from "../store";

export const authSelector = (state: RootState) => state.auth;

export const isUserAuthenticatedSelector = (state: RootState) =>
  !!state.auth.currentUser;
