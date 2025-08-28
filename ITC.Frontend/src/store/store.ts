import { configureStore } from "@reduxjs/toolkit";
import { combineReducers, createStore } from "redux";
import { RegistrySlice } from "./registry";
import { apiSlice } from "../api";
import { AuthSlice } from "./auth";
import { NotificationMiddleware, NotificationSlice } from "./notification";
import { SignalRMiddleware, SignalRSlice } from "./signalR";


export const appReducer = combineReducers({
  auth: AuthSlice.reducer,
  registry: RegistrySlice.reducer,
  api: apiSlice.reducer,
  notification: NotificationSlice.reducer,
  signalR: SignalRSlice.reducer,
});

export function setupStore() {
  return configureStore({
    reducer: appReducer,
    middleware: getDefaultMiddleware =>
      getDefaultMiddleware({ serializableCheck: false })
        .prepend([
          SignalRMiddleware.middleware,
          NotificationMiddleware.middleware,
        ])
        .concat([apiSlice.middleware]),
  });
}

export const store = setupStore();

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch;
