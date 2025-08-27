import React, { FC } from "react";
import { RegistryHeaderProps } from "./types";
import { Button } from "@consta/uikit/Button";
import { ChoiceGroup } from "@consta/uikit/ChoiceGroup";
import { useDispatch, useSelector } from "react-redux";
import { RegistrySlice } from "../../store";
import { TableFilterName, TableFilterType } from "../../types";
import { IconAdd } from "@consta/uikit/IconAdd";

type ItemType = {
  name: string,
  type: TableFilterType
}

const ITEMS: ItemType[] = [
  {
    name: TableFilterName.LIVE,
    type: 'LIVE'
  },
  {
    name: TableFilterName.FILE,
    type: 'FILE'
  }
]

export const RegistryHeader: FC<RegistryHeaderProps> = ({ onAddNewReport }) => {
  const dispatch = useDispatch()
  const mode = useSelector(RegistrySlice.selectors.mode)

  const isAddButtonVisible = mode === 'FILE'

  return (
    <div className="container-row justify-between align-center w-100 p-v-4">
      <ChoiceGroup
        value={ITEMS.find(item => item.type === mode)}
        onChange={({ value }) => { dispatch(RegistrySlice.actions.changeMode(value.type)) }}
        items={ITEMS}
        getItemLabel={(item) => item.name}
        size="s"
        view="ghost"
        name={""}
      />

      {isAddButtonVisible &&
        <Button
          label="Добавить"
          size="s"
          iconLeft={IconAdd}
          onClick={onAddNewReport}
        />
      }

    </div>
  );
};

