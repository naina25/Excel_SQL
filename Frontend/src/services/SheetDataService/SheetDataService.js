import { GetApi, PutApi } from "../BaseAPIService";

export const GetSheetData = async (url) => {
    try {
        let response = await GetApi(url).then((res) => {
            return res.data;
        });
        return response;
    } catch (error) {
        console.log("Error : " + error);
    }
};

export const GetSortedTableData = async (url, params) => {
    try {
        console.log(params);
        let response = await GetApi(url, params).then((res) => {
            return res.data;
        });
        return response;
    } catch (error) {
        console.log("Error : " + error);
    }
};

export const PutData = async (url, data) => {
    try {
        let response = await PutApi(url, data).then((res) => {
            return res.data;
        });
        return response;
    } catch (error) {
        console.log("Error : " + error);
    }
};
