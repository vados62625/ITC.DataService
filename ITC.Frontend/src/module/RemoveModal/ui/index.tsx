import { IconClose } from "@consta/uikit/IconClose"
import React from "react"
import { Modal } from "@consta/uikit/Modal";
import { Text } from "@consta/uikit/Text";
import styles from './styles.css'
import { useDispatch, useSelector } from "react-redux";
import { RegistrySlice } from "../../../store";
import { Button } from "@consta/uikit/Button";
import { EngineApi } from "../../../apiRTK";

export const RemoveModal = () => {
    const dispatch = useDispatch()

    const [deleteEngine, { isLoading: isDeleteLoading }] = EngineApi.useDeleteMutation()

    const { isOpen, selectedEngine } = useSelector(RegistrySlice.selectors.removeModal)

    const onCloseRemoveModal = () => {
        dispatch(RegistrySlice.actions.changeRemoveModal({ isOpen: false }))
    };

    const onClickActionBtn = () => {
        deleteEngine(selectedEngine?.id ?? '').finally(() => {
            onCloseRemoveModal()
        })
    }
    return (
        <Modal
            isOpen={isOpen}
            hasOverlay
            onClickOutside={onCloseRemoveModal}
            onEsc={onCloseRemoveModal}
        >
            <div className={styles.modalContainer}>
                <div className={styles.modalHeader}>
                    <Text size="xl" weight="medium" view="primary">Удаление данных о двигателе</Text>

                    <Button
                        onlyIcon={true}
                        view="clear"
                        size="s"
                        iconLeft={IconClose}
                        onClick={onCloseRemoveModal}
                    />
                </div>

                <div className={styles.modalContent}>
                    <Text view="secondary" size="s">
                        Данные о двигателе {selectedEngine?.name} будут удалены без возможности восстановления.
                        Продолжить?
                    </Text>
                </div>

                <div className={styles.modalFooter}>
                    <Button
                        label={"Отмена"}
                        view="ghost"
                        width="default"
                        size="s"
                        onClick={onCloseRemoveModal}
                    />

                    <Button
                        label={'Удалить'}
                        view="primary"
                        size="s"
                        width="default"
                        loading={isDeleteLoading}
                        onClick={onClickActionBtn}
                    />
                </div>
            </div>
        </Modal>
    )
}