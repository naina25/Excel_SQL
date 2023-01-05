import axios from "axios";
import React, { useEffect, useState } from "react";
import Chart from "chart.js/auto";

const PieChart = ({
    selectedSheet,
    firstSelectCol,
    secondSelectCol,
    distinctVal,
}) => {
    const [chartData, setChartData] = useState();
    useEffect(() => {
        const selectDistinctVal = distinctVal[firstSelectCol]
            ? distinctVal[firstSelectCol]
            : distinctVal;
        const GetChartData = () => {
            console.log(firstSelectCol);
            console.log(secondSelectCol);
            console.log(selectDistinctVal);
            axios
                .get(
                    `https://localhost:7108/api/ExcelData/sqltables/${selectedSheet}/chart`,
                    {
                        params: {
                            firstCol: firstSelectCol,
                            secondCol: secondSelectCol,
                            selectedVal: selectDistinctVal,
                        },
                    }
                )
                .then((res) => {
                    console.log(res.data);
                    setChartData(res.data);
                })
                .catch(() => setChartData());
        };
        GetChartData();
    }, [firstSelectCol, secondSelectCol, distinctVal]);

    useEffect(() => {
        const ctx = document.getElementById("myChart");

        const chart = new Chart(ctx, {
            type: "pie",
            data: {
                labels:
                    chartData &&
                    chartData.map((entry, index) => {
                        return entry[secondSelectCol];
                    }),
                datasets: [
                    {
                        label: `${secondSelectCol} - ${distinctVal}`,
                        data:
                            chartData &&
                            chartData.map((entry, index) => {
                                return entry.ValCount;
                            }),
                        backgroundColor:
                            chartData &&
                            chartData.map((entry, index) => {
                                var letters = "0123456789ABCDEF".split("");
                                var color = "#";
                                for (var i = 0; i < 6; i++) {
                                    color +=
                                        letters[Math.floor(Math.random() * 16)];
                                }
                                return color;
                            }),
                        hoverOffset: 15,
                    },
                ],
            },
        });

        return () => {
            chart.destroy();
        };
    }, [chartData]);

    return <canvas id="myChart"></canvas>;
};

export default PieChart;
