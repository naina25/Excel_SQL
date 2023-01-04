import axios from "axios";
import React, { useState } from "react";
import { useEffect } from "react";
import PieChart from "./PieChart";

const ReportPage = ({ selectedSheet }) => {
	const [colNames, setColNames] = useState();
	const [firstSelectCol, setFirstSelectCol] = useState();
	const [distinctArr, setDistinctArr] = useState();
	const [distinctVal, setDistinctVal] = useState();
	const [secondSelectCol, setSecondSelectCol] = useState();

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
		colNames && colNames.length && setFirstSelectCol(colNames[1]);
		colNames && colNames.length && setSecondSelectCol(colNames[2]);
	}, [colNames]);

	useEffect(() => {
		const GetDistinctEntries = () => {
			console.log(firstSelectCol);
			axios
				.get(
					`https://localhost:7108/api/ExcelData/sqltables/${selectedSheet}/${firstSelectCol}`
				)
				.then((res) => {
					setDistinctArr(res.data);
					setDistinctVal(res.data[0]);
				});
		};
		firstSelectCol && GetDistinctEntries();
	}, [colNames, firstSelectCol]);
	return (
		<div>
			GROUP BY:{" "}
			{colNames && colNames.length && (
				<>
					<select
						value={firstSelectCol}
						onChange={(e) => {
							setFirstSelectCol(e.target.value);
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
					{distinctVal && (
						<select
							value={distinctVal[firstSelectCol]}
							onChange={(e) => {
								setDistinctVal(e.target.value);
							}}
						>
							{distinctArr &&
								distinctArr.map((val, index) => {
									return (
										<option key={index}>
											{val[firstSelectCol]}
										</option>
									);
								})}
						</select>
					)}
					<select
						value={secondSelectCol}
						onChange={(e) => {
							setSecondSelectCol(e.target.value);
						}}
					>
						{colNames &&
							colNames.map((val, index) => {
								return (
									index !== 0 &&
									val !== firstSelectCol && (
										<option key={index}>{val}</option>
									)
								);
							})}
					</select>
				</>
			)}
			{secondSelectCol && distinctVal && (
				<div>
					<PieChart
						selectedSheet={selectedSheet}
						firstSelectCol={firstSelectCol}
						secondSelectCol={secondSelectCol}
						distinctVal={distinctVal}
					/>
				</div>
			)}
		</div>
	);
};

export default ReportPage;
