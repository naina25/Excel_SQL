import { GetSheets } from "../../services/SheetsDropdownService/SheetsDropdownService";

export const GetSheetNames = async (
    setIsLoadingSheetNames,
    source,
    setSheets,
    setSelectedSheet
) => {
    setIsLoadingSheetNames && setIsLoadingSheetNames(true);
    const sheets = await GetSheets(`/${source}`);
    if (sheets) {
        setSheets(sheets);
        setSelectedSheet(() => (sheets[0].Table ? sheets[0].Table : sheets[0]));
        setIsLoadingSheetNames && setIsLoadingSheetNames(false);
    }
};
