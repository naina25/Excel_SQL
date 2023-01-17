import { GetDataForChart } from "../../services/PieChartService/PieChartService";

export const GetChartData = async (
    chartData,
    setChartData,
    selectedArr,
    selectedSheet,
    firstSelectCol,
    secondSelectCol
) => {
    chartData && setChartData();
    const url =
        selectedArr.length > 1
            ? `/sqltables/${selectedSheet}/Barchart`
            : `/sqltables/${selectedSheet}/chart`;
    const params = {
        firstCol: firstSelectCol,
        secondCol: secondSelectCol,
        selectedVal: selectedArr.length > 1 ? selectedArr : selectedArr[0],
    };
    const getData = await GetDataForChart(url, params);

    getData && setChartData(getData);
};
