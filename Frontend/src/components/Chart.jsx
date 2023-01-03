import axios from "axios";
import React, { useState } from "react";
import { useEffect } from "react";

const Chart = ({ selectedSheet, sheetData }) => {
	const [colNames, setColNames] = useState();
	const [selectedCol, setSelectedCol] = useState();

	useEffect(() => {
		const getColumnNames = () => {
			axios
				.get(
					`https://localhost:7108/api/ExcelData/sqltables/${selectedSheet}`
				)
				.then((res) => {
					setColNames(() => res.data.map((val) => val.name));
				});
		};
		getColumnNames();
	}, [selectedSheet]);

	useEffect(() => {
		colNames && colNames.length && setSelectedCol(colNames[1]);
	}, [colNames]);

	useEffect(() => {
		console.log(selectedCol);
	}, [selectedCol]);
	return (
		<div>
			GROUP BY:{" "}
			{colNames && colNames.length && (
				<select
					onChange={(e) => {
						setSelectedCol(e.target.value);
					}}
				>
					{colNames &&
						colNames.map((val, index) => {
							return (
								index !== 0 && (
									<option key={index}>{val}</option>
								)
							);
						})}
				</select>
			)}
		</div>
	);
};

export default Chart;
