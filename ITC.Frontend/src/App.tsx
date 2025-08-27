import { Theme, presetGpnDefault } from "@consta/uikit/Theme";
import { Routers } from "./module";
import React from "react";
import { withNotifications } from "./store/notification";

const AppWithNotifications = withNotifications(<Routers />)

const App: React.FC = () => {
  const AppWithHOC = () => AppWithNotifications()
  
  return (<Theme preset={presetGpnDefault}>
    <AppWithHOC />
  </Theme>)
}

export default App;
