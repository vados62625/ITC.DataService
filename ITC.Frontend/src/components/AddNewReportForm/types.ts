export type AddNewReportFormProps = {
  onAddNewReport: (reportName: string | null, file: File | null) => void;
  onCloseModal: () => void;
};
