import React, { FC, useState } from "react";
import { RegistryHeaderProps } from "./types";
import { Button } from "@consta/uikit/Button";
import { IconAdd } from "@consta/uikit/IconAdd";
import { Text } from "@consta/uikit/Text";
import { ChoiceGroup } from "@consta/uikit/ChoiceGroup";
import { useDispatch, useSelector } from "react-redux";
import { RegistrySlice } from "../../store";
import { TableType } from "../../types";
import { Delimiter } from "../Delimiter";

type ItemType = {
  name: string,
  type: TableType
}

const ITEMS: ItemType[] = [
  {
    name: 'Лайв',
    type: 'Live'
  },
  {
    name: "Загруженные",
    type: 'File'
  }
]

export const RegistryHeader: FC<RegistryHeaderProps> = ({ onAddNewReport }) => {
  const dispatch = useDispatch()
  const mode = useSelector(RegistrySlice.selectors.mode)

  return (
    <div className="container-row justify-between align-center w-100 p-v-4">
      <Text size="2xl" weight="bold">
        Отчеты
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
          label="Новый отчет"
          size="s"
          iconRight={IconAdd}
          onClick={onAddNewReport}
        />
      </div>

    </div>
  );
};

