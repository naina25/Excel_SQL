import { GetData } from "../../services/ReportPageService/ReportPageService";

export const getColumnNames = async (selectedSheet, setColNames) => {
    const columnNames = await GetData(`/sqltables/${selectedSheet}`);

    columnNames && setColNames(() => columnNames.map((val) => val.name));
};

export const GetDistinctEntries = async (
    sheet,
    col,
    setDistinctArr,
    setSelectedArr
) => {
    const distinctEntries = await GetData(`/sqltables/${sheet}/${col}`);

    typeof distinctEntries === "object" && setDistinctArr(distinctEntries);
    setSelectedArr &&
        setSelectedArr([
            distinctEntries[0][col] !== ""
                ? distinctEntries[0][col]
                : distinctEntries[1][col],
        ]);
};

export const SelectCheckbox = (e, val, selectedArr, setSelectedArr) => {
    if (e.ctrlKey)
        if (selectedArr.indexOf(val) !== -1) {
            setSelectedArr(() => selectedArr.filter((el) => el !== val));
        } else {
            setSelectedArr([...selectedArr, val]);
        }
    else {
        setSelectedArr([val]);
    }
};
