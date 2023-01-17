import {
    GetSheetData,
    GetSortedTableData,
    PutData,
} from "../../services/SheetDataService/SheetDataService";

export const putRow = async (
    editData,
    selectedSheet,
    setHasDataUpdated,
    hasDataUpdated
) => {
    const jsonStr = JSON.stringify(editData).replaceAll('"', '\\"');

    const response = await PutData(
        `/sheets/${selectedSheet}/edit`,
        `${jsonStr}`
    );

    if (response) {
        console.log("Updated Successfully");
        setHasDataUpdated(!hasDataUpdated);
    }
};

export const GetSortedData = async (
    selectedSheet,
    col,
    sortOrder,
    setIsLoadingSheetData,
    setOrder,
    setSheetData
) => {
    setIsLoadingSheetData && setIsLoadingSheetData(true);
    setOrder(sortOrder);

    const params = { column: col, order: sortOrder };
    console.log(params);
    const getSortedData = await GetSortedTableData(
        `/sheets/sort/${selectedSheet}`,
        params
    );

    if (getSortedData) {
        setSheetData(() => getSortedData);
        setIsLoadingSheetData && setIsLoadingSheetData(false);
    }
};

export const getSheetData = async (
    isLoadingSheetData,
    setIsLoadingSheetData,
    selectedSheet,
    setSheetData
) => {
    isLoadingSheetData !== undefined && setIsLoadingSheetData(true);
    const getSheetsResponse = await GetSheetData(`/sheets/${selectedSheet}`);

    if (getSheetsResponse) {
        setSheetData && setSheetData(getSheetsResponse);
        setIsLoadingSheetData && setIsLoadingSheetData(false);
    }
};
