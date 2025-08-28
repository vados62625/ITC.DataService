import { Theme, presetGpnDefault } from "@consta/uikit/Theme";
import { PrivateRoute, Routers } from "./module";
import React from "react";
import { withNotifications } from "./store/notification";
import { SignalRProvider } from "./signalR";

const AppWithNotifications = withNotifications(<Routers />)

const App: React.FC = () => {
  const AppWithHOC = () => AppWithNotifications()

  return (<Theme preset={presetGpnDefault}>
    <PrivateRoute>
      <SignalRProvider>
        <AppWithHOC />
      </SignalRProvider>
    </PrivateRoute>
  </Theme>)
}

export default App;
