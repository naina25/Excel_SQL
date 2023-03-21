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

export const SelectCheckbox = (
    e,
    val,
    selectedArr,
    setSelectedArr,
    setChartType
) => {
    if (e.ctrlKey)
        if (selectedArr.indexOf(val) !== -1) {
            setSelectedArr(() => selectedArr.filter((el) => el !== val));
        } else {
            selectedArr.length === 1 && setChartType("bar");
            setSelectedArr([...selectedArr, val]);
        }
    else {
        setSelectedArr([val]);
    }
};

export const getSelectValues = (
    select,
    selectedArr,
    setSelectedArr,
    setChartType
) => {
    var result = [];
    var options = select && select.options;
    var opt;

    for (var i = 0, iLen = options.length; i < iLen; i++) {
        opt = options[i];

        if (opt.selected) {
            result.push(opt.value || opt.text);
        }
    }
    setSelectedArr(result);
    selectedArr.length > 0 && setChartType("bar");
    //return result;
};
