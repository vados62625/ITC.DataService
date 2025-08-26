import { Tooltip } from "@consta/uikit/Tooltip";
import React, { useRef, useState } from "react";

import css from "./style.css";
import type { ComponentWithTooltipProps } from "./types";

/**
 * HighOrderComponent, возвращающий компонент обернутый в Tooltip
 * @param WrappedComponent компонент, который необходимо обернуть в Tooltip
 * @returns обернутый в Tooltip компонент
 */
export function withTooltip<T>(WrappedComponent: React.ComponentType<T>) {
  const Component: React.FC<T & ComponentWithTooltipProps> = (props) => {
    const [showTooltip, setShowTooltip] = useState(false);
    const ref = useRef(null);

    const onShowTooltip = () => {
      setShowTooltip(true);
    };

    const onHideTooltip = () => {
      setShowTooltip(false);
    };

    return (
      <div>
        <div ref={ref} onMouseOver={onShowTooltip} onMouseLeave={onHideTooltip}>
          <WrappedComponent {...props} />
        </div>

        {showTooltip && (
          <Tooltip
            className={css.tooltip}
            anchorRef={ref}
            direction={props.direction ?? "upCenter"}
            size="l"
            placeholder={undefined}
            onPointerEnterCapture={undefined}
            onPointerLeaveCapture={undefined}
          >
            {props.children}
          </Tooltip>
        )}
      </div>
    );
  };

  return Component;
}
