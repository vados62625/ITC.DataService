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

export const authSlice = createSlice({
  name: "authSlice",
  initialState: initialState,
  reducers: {
    replaceCurrentUser,
  },
});

export const authReducer = authSlice.reducer;
