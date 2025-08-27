import { useState } from "react";
import {
  AddNewReportForm,
  RegistryHeader,
} from "../../components";
import { Modal } from "@consta/uikit/Modal";
import { EnginesTable } from "../../module";
import styles from './style.css'
import { Pagination } from "antd";
import React from "react";
import { EngineApi } from "../../apiRTK";

export const Registry = () => {
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const [addEngine] = EngineApi.useAddMutation()
  const onAddNewReport = (reportName: string | null, file: File | null) => {
    addEngine({name: reportName, file})
  };

  const onOpenModal = () => {
    setIsModalOpen(true);
  };

  const onCloseModal = () => {
    setIsModalOpen(false);
  };

  return (
    <div className={styles.container}>

      <div className={styles.content}>
        <RegistryHeader onAddNewReport={onOpenModal} />
        <EnginesTable />
        <Pagination defaultPageSize={10} total={100} className="p-t-4"/>

        <Modal
          isOpen={isModalOpen}
          hasOverlay
          onClickOutside={onCloseModal}
          onEsc={onCloseModal}
        >
          <AddNewReportForm
            onAddNewReport={onAddNewReport}
            onCloseModal={onCloseModal}
          />
        </Modal>
      </div>
    </div>
  );
};
