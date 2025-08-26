import React from "react";

import style from "./styles.css";
import type { DelimiterProps } from "./types";

export const Delimiter: React.FC<DelimiterProps> = (
  { className } = { className: "" }
) => {
  return <div className={`${style.delimiter} ${className}`} />;
};
