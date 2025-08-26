import React from "react";

export type ComponentWithTooltipProps = {
  children: React.ReactNode;
  direction?:
    | "downCenter"
    | "upCenter"
    | "downRight"
    | "downLeft"
    | "upRight"
    | "upLeft"
    | "leftUp"
    | "leftCenter"
    | "leftDown"
    | "rightUp"
    | "rightCenter"
    | "rightDown"
    | "downStartLeft"
    | "upStartLeft"
    | "downStartRight"
    | "upStartRight"
    | "leftStartUp"
    | "leftStartDown"
    | "rightStartUp"
    | "rightStartDown"
    | undefined;
  leftOffset?: number;
};
