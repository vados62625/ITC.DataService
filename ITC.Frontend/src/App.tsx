import { Theme, presetGpnDefault } from "@consta/uikit/Theme";
import { Routers } from "./module";
import React from "react";

const App: React.FC = () => (
  <Theme preset={presetGpnDefault}>
      <Routers />
  </Theme>
);

export default App;
