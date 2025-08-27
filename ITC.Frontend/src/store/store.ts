import { configureStore } from "@reduxjs/toolkit";
import { combineReducers } from "redux";
import { RegistrySlice } from "./registry";
import { apiSlice } from "../api";
import { AuthSlice } from "./auth";


export const appReducer = combineReducers({
  auth: AuthSlice.reducer,
  registry: RegistrySlice.reducer,
  api: apiSlice.reducer
});

export function setupStore() {
  return configureStore({
    reducer: appReducer,
    middleware: getDefaultMiddleware =>
      getDefaultMiddleware({ serializableCheck: false })
        .prepend([
          // SignalRMiddleware.middleware,
          // NotificationMiddleware.middleware,
        ])
        .concat([apiSlice.middleware]),
  });
}

export const store = setupStore();

export type RootState = ReturnType<typeof appReducer>;
export type AppDispatch = typeof store.dispatch;
