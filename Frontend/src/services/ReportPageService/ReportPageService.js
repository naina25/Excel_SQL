import { GetApi } from "../BaseAPIService";

export const GetData = async (url) => {
    try {
        let response = await GetApi(url).then((res) => {
            return res.data;
        });
        return response;
    } catch (error) {
        console.log("Error : " + error);
    }
};
