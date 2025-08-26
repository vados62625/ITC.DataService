import React, { FC } from "react";
import { IconAdd } from "@consta/uikit/IconAdd";
import { ResponsesEmptyBox } from "@consta/uikit/ResponsesEmptyBox";
import { Button } from "@consta/uikit/Button";
import { RegistryPlaceholderProps } from "./types";

export const RegistryPlaceholder: FC<RegistryPlaceholderProps> = ({
  onOpenModal,
}) => {
  return (
    <div className="container-column justify-center align-center w-100 h-100">
      <ResponsesEmptyBox
        actions={
          <Button
            label="Новый отчет"
            size="s"
            iconRight={IconAdd}
            onClick={onOpenModal}
          />
        }
      />
    </div>
  );
};
