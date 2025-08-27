import {
  RegistryHeader,
} from "../../components";
import { AddModal, EnginesTable, RemoveModal } from "../../module";
import styles from './style.css'
import { Pagination } from "antd";
import React from "react";
import { useDispatch } from "react-redux";
import { RegistrySlice } from "../../store";

export const Registry = () => {
  const dispatch = useDispatch()

  const onOpenModal = () => {
    dispatch(RegistrySlice.actions.changeAddModal({ isOpen: true }))
  };

  return (
    <div className={styles.container}>
      <div className={styles.content}>
        <RegistryHeader onAddNewReport={onOpenModal} />
        <EnginesTable />
        <Pagination defaultPageSize={10} total={100} className="p-t-4" />
        <AddModal />
        <RemoveModal />
      </div>
    </div>
  );
};
