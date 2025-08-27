import React from "react"
import { Modal } from "@consta/uikit/Modal";
import { useDispatch, useSelector } from "react-redux";
import { RegistrySlice } from "../../../store";
import { EngineApi } from "../../../apiRTK";
import { AddNewReportForm } from "../../../components";

export const AddModal = () => {
    const dispatch = useDispatch()
    const [addEngine] = EngineApi.useAddMutation()

    const { isOpen } = useSelector(RegistrySlice.selectors.addModal)

    const onAddNewReport = (reportName: string | null, file: File | null) => {
        addEngine({ name: reportName, file })
    };
    const onCloseModal = () => {
        dispatch(RegistrySlice.actions.changeAddModal({ isOpen: false }))
    };

    return (
        <Modal
            isOpen={isOpen}
            hasOverlay
            onClickOutside={onCloseModal}
            onEsc={onCloseModal}
        >
            <AddNewReportForm
                onAddNewReport={onAddNewReport}
                onCloseModal={onCloseModal}
            />
        </Modal>
    )
}