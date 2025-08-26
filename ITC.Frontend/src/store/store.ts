import { configureStore } from "@reduxjs/toolkit";
import type { TypedUseSelectorHook } from "react-redux";
import { useDispatch, useSelector } from "react-redux";
import { combineReducers } from "redux";
import { authReducer } from "./auth";
import { RegistrySlice } from "./registry";

export const appReducer = combineReducers({
  auth: authReducer,
  registry: RegistrySlice.reducer
});

export function setupStore() {
  return configureStore({
    reducer: appReducer,
    middleware: (getDefaultMiddleware) => {
      return getDefaultMiddleware({
        serializableCheck: false,
        immutableCheck: false,
      });
    },
  });
}

export const store = setupStore();

export type RootState = ReturnType<typeof appReducer>;
export type AppDispatch = typeof store.dispatch;
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
