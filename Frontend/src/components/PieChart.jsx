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

		chartData &&
			new Chart(ctx, {
				type: "pie",
				data: {
					labels: [
						"Red",
						"Blue",
						"Yellow",
						"Green",
						"Purple",
						"Orange",
					],
					datasets: [
						{
							label: "# of Votes",
							data: [12, 19, 3, 5, 2, 3],
							backgroundColor: [
								"Red",
								"Blue",
								"Yellow",
								"Green",
								"Purple",
								"Orange",
							],
							borderColor: [
								"Red",
								"Blue",
								"Yellow",
								"Green",
								"Purple",
								"Orange",
							],
							borderWidth: 1,
						},
					],
				},
			});
	}, [chartData]);

	return <canvas id="myChart">PieChart</canvas>;
};

export default PieChart;
