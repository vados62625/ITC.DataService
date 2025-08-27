import React, { FC, useState } from "react";
import { AddNewReportFormProps } from "./types";
import { Button } from "@consta/uikit/Button";
import { IconClose } from "@consta/uikit/IconClose";
import { IconAttach } from "@consta/uikit/IconAttach";
import { Text } from "@consta/uikit/Text";
import { TextField } from "@consta/uikit/TextField";
import { DragNDropField } from "@consta/uikit/DragNDropField";
import { Attachment } from "@consta/uikit/Attachment";

import css from "./style.css";
import { humanFileSize } from "../../utils";

export const AddNewReportForm: FC<AddNewReportFormProps> = ({
  onAddNewReport,
  onCloseModal,
}) => {
  const [engineNumber, setEngineNumber] = useState<string | null>(null);
  const [file, setFile] = React.useState<File | null>(null);

  const onChangeEngineNumber = ({ value }: { value: string | null }) =>
    setEngineNumber(value);

  const onDropFiles = (files: File[]): void => {
    setFile(files[0]);
  };

  const onDeleteFile = () => {
    setFile(null);
  };

  const isSaveDisabled = !engineNumber || !file;

  return (
    <div className={`container-column ${css.addNewReportFormModal} p-8`}>
      <div className="container-row justify-between p-b-8">
        <Text size="xl" weight="bold">
          Загрузка данных
        </Text>
        <Button
          size="s"
          onlyIcon={true}
          iconLeft={IconClose}
          view="clear"
          onClick={onCloseModal}
        />
      </div>
      <div className="w-100 container-column">
        <TextField
          className="m-b-6"
          onChange={onChangeEngineNumber}
          value={engineNumber}
          type="text"
          placeholder="Введите номер двигателя"
          label="Номер двигателя"
          required
        />
         {/* <TextField
          className="m-b-6"
          onChange={onChangeEngineNumber}
          value={engineNumber}
          type="text"
          placeholder="Введите номер двигателя"
          label="Номер двигателя"
          required
        /> */}
        <DragNDropField
          className="m-b-6"
          onDropFiles={onDropFiles}
          accept=".csv"
        >
          {({ openFileDialog }) => (
            <>
              <Text>Перетащите файл сюда или нажмите на кнопку ниже</Text>
              <Text view="ghost" font="mono">
                Поддерживаемые форматы: CSV
              </Text>
              <Button
                className="m-t-6"
                onClick={openFileDialog}
                view="ghost"
                iconLeft={IconAttach}
                label="Выбрать файл"
              />
            </>
          )}
        </DragNDropField>
        <div className={css.fileContainer}>
          {file && (
            <Attachment
              key={file.name}
              fileName={file.name}
              fileExtension={"csv"}
              fileDescription={`${humanFileSize(
                file.size
              )} ${new Date().toLocaleString()}`}
              withAction
              onButtonClick={onDeleteFile}
              buttonIcon={IconClose}
            />
          )}
        </div>
      </div>

      <Button
        className="m-t-4"
        label="Сохранить"
        size="m"
        onClick={() => {
          onAddNewReport(engineNumber, file);
          onCloseModal();
        }}
        disabled={isSaveDisabled}
      />
    </div>
  );
};
