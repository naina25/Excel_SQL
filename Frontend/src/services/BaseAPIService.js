import axios from "axios";

const baseUrl = "https://localhost:7108/api/ExcelData";

export const GetApi = async (apiURL, params) => {
    const response = await axios.get(
        baseUrl + apiURL,
        params && {
            params: { ...params },
        }
    );
    return response;
};

export const PutApi = async (apiURL, data) => {
    const response = await axios.put(baseUrl + apiURL, data, {
        headers: { "content-type": "application/json" },
    });
    return response;
};
