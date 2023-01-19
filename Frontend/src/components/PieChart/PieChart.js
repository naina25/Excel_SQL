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

export const SetDataState = async (
    setDatasetState,
    distinctArr,
    chartData,
    chartType,
    secondSelectCol,
    selectedArr
) => {
    setDatasetState([]);
    if (distinctArr && chartData && distinctArr.length && chartData.length) {
        if (chartType === "bar") {
            SetBarChartData(
                distinctArr,
                setDatasetState,
                secondSelectCol,
                chartData
            );
        } else if (chartType === "radar" && selectedArr.length > 1) {
            if (chartData) {
                SetRadarChartData(
                    chartData,
                    setDatasetState,
                    selectedArr,
                    distinctArr,
                    secondSelectCol
                );
            }
        }
    }
};

export const GetLabelsForChart = (
    selectedArr,
    chartType,
    distinctArr,
    secondSelectCol,
    chartData
) => {
    return selectedArr && selectedArr.length > 1
        ? chartType === "bar"
            ? selectedArr.map((selectedVal) => {
                  return selectedVal;
              })
            : chartType === "radar" &&
              distinctArr &&
              distinctArr.length &&
              distinctArr.map((val) => {
                  return val[secondSelectCol];
              })
        : chartData &&
              chartData.map((entry) => {
                  return entry[secondSelectCol];
              });
};

export const GetDataSet = (
    selectedArr,
    datasetState,
    secondSelectCol,
    chartData
) => {
    return selectedArr && selectedArr.length > 1
        ? datasetState
        : [
              {
                  label: `${secondSelectCol} - ${selectedArr[0]}`,
                  data:
                      chartData &&
                      chartData.map((entry) => {
                          return entry.ValCount;
                      }),
                  backgroundColor:
                      chartData &&
                      chartData.map(() => {
                          var letters = "0123456789ABCDEF".split("");
                          var color = "#";
                          for (var i = 0; i < 6; i++) {
                              color += letters[Math.floor(Math.random() * 16)];
                          }
                          return color;
                      }),
                  hoverOffset: 15,
              },
          ];
};

export const GetOptionsForChart = (
    selectedArr,
    chartType,
    secondSelectCol,
    firstSelectCol
) => {
    return selectedArr && selectedArr.length > 1 && chartType === "bar"
        ? {
              scales: {
                  y: {
                      title: {
                          display: true,
                          text: `# of ${secondSelectCol}s`,
                      },
                  },
                  x: {
                      title: {
                          display: true,
                          text: `${firstSelectCol}s`,
                      },
                  },
              },
          }
        : chartType === "radar" && {
              elements: {
                  line: {
                      borderWidth: 3,
                  },
              },
          };
};

const SetBarChartData = (
    distinctArr,
    setDatasetState,
    secondSelectCol,
    chartData
) => {
    for (let i = 0; i < distinctArr.length; i++) {
        setDatasetState((prev) => [
            ...prev,
            {
                label: distinctArr[i][secondSelectCol],
                data:
                    chartData &&
                    chartData.map((entry) => {
                        if (typeof entry == "string") {
                            const entryObj = JSON.parse(entry);
                            for (let j = 0; j < entryObj.length; j++) {
                                if (
                                    entryObj[j][secondSelectCol].trim() ===
                                    distinctArr[i][secondSelectCol].trim()
                                ) {
                                    return entryObj[j].ValCount;
                                } else if (
                                    j === entryObj.length - 1 &&
                                    entryObj[j][secondSelectCol].trim() !==
                                        distinctArr[i][secondSelectCol].trim()
                                ) {
                                    return 0;
                                }
                            }
                        }
                    }),
                hoverOffset: 15,
            },
        ]);
    }
};

const SetRadarChartData = (
    chartData,
    setDatasetState,
    selectedArr,
    distinctArr,
    secondSelectCol
) => {
    for (let i = 0; i < chartData.length; i++) {
        const entryObj = JSON.parse(chartData[i]);
        setDatasetState((prev) => [
            ...prev,
            {
                label: selectedArr[i],
                data:
                    distinctArr &&
                    distinctArr.map((distinctCol) => {
                        for (let k = 0; k < entryObj.length; k++) {
                            if (
                                distinctCol[secondSelectCol].trim() ===
                                entryObj[k][secondSelectCol].trim()
                            ) {
                                return entryObj[k].ValCount;
                            } else if (
                                distinctCol[secondSelectCol].trim() !==
                                    entryObj[k][secondSelectCol].trim() &&
                                k === entryObj.length - 1
                            ) {
                                return 0;
                            }
                        }
                    }),
                hoverOffset: 15,
            },
        ]);
    }
};
