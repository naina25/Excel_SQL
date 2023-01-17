import { GetApi } from "../BaseAPIService";

export const GetSearchData = async (url, params) => {
    try {
        let response = await GetApi(url, params).then((res) => {
            return res.data;
        });
        return response;
    } catch (error) {
        console.log("Error : " + error);
    }
};
