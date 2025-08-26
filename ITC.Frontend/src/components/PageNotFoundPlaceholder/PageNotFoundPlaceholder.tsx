import React from "react";
import { Responses404 } from "@consta/uikit/Responses404";
import { Button } from "@consta/uikit/Button";
import { useNavigate } from "react-router-dom";
import { RoutePaths } from "../../types";

export const PageNotFoundPlaceholder = () => {
  const navigate = useNavigate();

  return (
    <div className="container-column justify-center align-center w-100 h-100">
      <Responses404
        actions={
          <Button
            size="m"
            view="ghost"
            label="На главную"
            onClick={() => {
              navigate(RoutePaths.Registry);
            }}
          />
        }
      />
    </div>
  );
};
