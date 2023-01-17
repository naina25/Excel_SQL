import { GetSearchData } from "../../services/SearchComponentService/SearchComponentService";

export const GetSearchedData = async (
    searchQuery,
    selectedSheet,
    setIsLoadingSheetData,
    setSheetData
) => {
    const getUrl = searchQuery
        ? `/sheets/search/${selectedSheet}`
        : `/sheets/${selectedSheet}`;
    setIsLoadingSheetData(true);

    const params = searchQuery && { searchQuery: searchQuery };
    const searchedData = await GetSearchData(getUrl, params);

    if (searchedData) {
        setSheetData(searchedData);
        setIsLoadingSheetData(false);
    }
};
