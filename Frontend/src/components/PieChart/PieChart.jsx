import React, { useEffect, useState } from "react";
import Chart from "chart.js/auto";
import "./PieChart.css";
import {
    GetChartData,
    GetDataSet,
    GetLabelsForChart,
    GetOptionsForChart,
    SetDataState,
} from "./PieChart";

const PieChart = ({
    selectedSheet,
    firstSelectCol,
    secondSelectCol,
    selectedArr,
    GetDistinctEntries,
    chartType,
}) => {
    const [chartData, setChartData] = useState();
    const [distinctArr, setDistinctArr] = useState();
    const [datasetState, setDatasetState] = useState([]);

    useEffect(() => {
        setDistinctArr();
        GetDistinctEntries(selectedSheet, secondSelectCol, setDistinctArr);
    }, [firstSelectCol, selectedArr, secondSelectCol]);

    useEffect(() => {
        SetDataState(
            setDatasetState,
            distinctArr,
            chartData,
            chartType,
            secondSelectCol,
            selectedArr
        );
    }, [distinctArr, chartData, chartType]);

    useEffect(() => {
        selectedArr[0] &&
            GetChartData(
                chartData,
                setChartData,
                selectedArr,
                selectedSheet,
                firstSelectCol,
                secondSelectCol
            );
    }, [firstSelectCol, secondSelectCol, selectedArr, distinctArr]);

    useEffect(() => {
        const ctx = document.getElementById("myChart");
        const chart = new Chart(ctx, {
            type: chartType,
            data: {
                labels: GetLabelsForChart(
                    selectedArr,
                    chartType,
                    distinctArr,
                    secondSelectCol,
                    chartData
                ),
                datasets: GetDataSet(
                    selectedArr,
                    datasetState,
                    secondSelectCol,
                    chartData
                ),
            },
            options: GetOptionsForChart(
                selectedArr,
                chartType,
                secondSelectCol,
                firstSelectCol
            ),
        });

        return () => {
            chart.destroy();
        };
    }, [datasetState, chartType]);

    return <canvas id="myChart"></canvas>;
};

export default PieChart;
