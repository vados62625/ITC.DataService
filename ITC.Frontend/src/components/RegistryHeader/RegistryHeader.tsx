import React, { FC, useState } from "react";
import { RegistryHeaderProps } from "./types";
import { Button } from "@consta/uikit/Button";
import { IconUpload } from "@consta/uikit/IconUpload";
import { Text } from "@consta/uikit/Text";
import { ChoiceGroup } from "@consta/uikit/ChoiceGroup";
import { useDispatch, useSelector } from "react-redux";
import { RegistrySlice } from "../../store";
import { TableFilterName,  TableFilterType} from "../../types";
import { Delimiter } from "../Delimiter";

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
    name:  TableFilterName.FILE,
    type: 'FILE'
  }
]

export const RegistryHeader: FC<RegistryHeaderProps> = ({ onAddNewReport }) => {
  const dispatch = useDispatch()
  const mode = useSelector(RegistrySlice.selectors.mode)

  return (
    <div className="container-row justify-between align-center w-100 p-v-4">
      <Text size="2xl" weight="bold">
        Двигатели
      </Text>

      <div className="">
        <ChoiceGroup
          value={ITEMS.find(item=>item.type===mode)}
          onChange={({ value }) => { dispatch(RegistrySlice.actions.changeMode(value.type)) }}
          items={ITEMS}
          getItemLabel={(item) => item.name}
          size="s"
          view="ghost"
          name={""}
        />

        <Delimiter />

        <Button
          label="Анализ"
          size="s"
          iconRight={IconUpload}
          onClick={onAddNewReport}
        />
      </div>

    </div>
  );
};

