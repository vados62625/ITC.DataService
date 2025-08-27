import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { initialState } from "./initialState";
import { initialAuthState } from "./types";
import { User } from "../../types";

/**
 * Заменяет текущего пользователя
 * @param state текущее состояние
 * @param action текущий пользователь
 */
const replaceCurrentUser = (
  state: initialAuthState,
  action: PayloadAction<User | null>
) => {
  state.currentUser = action.payload;
};

export const AuthSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    replaceCurrentUser
  },
  selectors: {
    authSelector: state => state,
    isUserAuthenticatedSelector: state => !!state.currentUser,
  },
})