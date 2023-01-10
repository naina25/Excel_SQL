import axios from "axios";
import React, { useEffect, useState } from "react";
import Chart from "chart.js/auto";

const PieChart = ({
	selectedSheet,
	firstSelectCol,
	secondSelectCol,
	selectedArr,
	GetDistinctEntries,
}) => {
	const [chartData, setChartData] = useState();
	const [distinctArr, setDistinctArr] = useState();
	const [datasetState, setDatasetState] = useState([]);

	useEffect(() => {
		setDistinctArr();
		GetDistinctEntries(selectedSheet, secondSelectCol, setDistinctArr);
	}, [firstSelectCol, selectedArr, secondSelectCol]);

	useEffect(() => {
		setDatasetState([]);
		if (
			distinctArr &&
			chartData &&
			distinctArr.length &&
			chartData.length
		) {
			for (let i = 0; i < distinctArr.length; i++) {
				setDatasetState((prev) => [
					...prev,
					{
						label: distinctArr[i][secondSelectCol],
						data:
							chartData &&
							chartData.map((entry) => {
								if (typeof entry === "string") {
									const entryObj = JSON.parse(entry);
									for (let j = 0; j < entryObj.length; j++) {
										if (
											entryObj[j][
												secondSelectCol
											].trim() ==
											distinctArr[i][
												secondSelectCol
											].trim()
										) {
											return entryObj[j].ValCount;
										} else if (
											j === entryObj.length - 1 &&
											entryObj[j][
												secondSelectCol
											].trim() !=
												distinctArr[i][
													secondSelectCol
												].trim()
										) {
											return 0;
										}
									}
								} else {
									return entry.ValCount;
								}
							}),
						hoverOffset: 15,
					},
				]);
			}
		}
	}, [distinctArr, chartData]);

	useEffect(() => {
		const GetChartData = () => {
			chartData && setChartData();
			const url =
				selectedArr.length > 1
					? `https://localhost:7108/api/ExcelData/sqltables/${selectedSheet}/Barchart`
					: `https://localhost:7108/api/ExcelData/sqltables/${selectedSheet}/chart`;
			axios
				.get(url, {
					params: {
						firstCol: firstSelectCol,
						secondCol: secondSelectCol,
						selectedVal:
							selectedArr.length > 1
								? selectedArr
								: selectedArr[0],
					},
				})
				.then((res) => {
					setChartData(JSON.parse(res.data));
				})
				.catch(() => setChartData());
		};
		selectedArr[0] && GetChartData();
	}, [firstSelectCol, secondSelectCol, selectedArr, distinctArr]);

	useEffect(() => {
		const ctx = document.getElementById("myChart");

		const chart = new Chart(ctx, {
			type: selectedArr.length > 1 ? "bar" : "pie",
			data: {
				labels:
					selectedArr && selectedArr.length > 1
						? selectedArr.map((selectedVal) => {
								return selectedVal;
						  })
						: chartData &&
						  chartData.map((entry) => {
								return entry[secondSelectCol];
						  }),
				datasets:
					selectedArr && selectedArr.length > 1
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
											var letters =
												"0123456789ABCDEF".split("");
											var color = "#";
											for (var i = 0; i < 6; i++) {
												color +=
													letters[
														Math.floor(
															Math.random() * 16
														)
													];
											}
											return color;
										}),
									hoverOffset: 15,
								},
						  ],
			},
			options: selectedArr &&
				selectedArr.length > 1 && {
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
				},
		});

		return () => {
			chart.destroy();
		};
	}, [datasetState]);

	return <canvas id="myChart"></canvas>;
};

export default PieChart;
