import axios from "axios";
import React, { useState } from "react";
import { useEffect } from "react";
import PieChart from "./PieChart";

const ReportPage = ({ selectedSheet }) => {
	const [colNames, setColNames] = useState();
	const [firstSelectCol, setFirstSelectCol] = useState();
	const [distinctArr, setDistinctArr] = useState();
	const [secondSelectCol, setSecondSelectCol] = useState();
	const [selectedArr, setSelectedArr] = useState();

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
		selectedSheet && getColumnNames();
	}, [selectedSheet]);

	useEffect(() => {
		setSelectedArr();
		colNames && colNames.length && setFirstSelectCol(colNames[1]);
		colNames && colNames.length && setSecondSelectCol(colNames[2]);
	}, [colNames]);

	const GetDistinctEntries = (sheet, col, setDistinctArr, setSelectedArr) => {
		axios
			.get(
				`https://localhost:7108/api/ExcelData/sqltables/${sheet}/${col}`
			)
			.then((res) => {
				setDistinctArr(res.data);
				setSelectedArr &&
					setSelectedArr([
						res.data[0][col] !== ""
							? res.data[0][col]
							: res.data[1][col],
					]);
			});
	};

	useEffect(() => {
		firstSelectCol &&
			GetDistinctEntries(
				selectedSheet,
				firstSelectCol,
				setDistinctArr,
				setSelectedArr
			);
	}, [firstSelectCol]);

	const SelectCheckbox = (e, val) => {
		if (e.ctrlKey)
			if (selectedArr.indexOf(val) !== -1) {
				console.log(val);
				setSelectedArr(() => selectedArr.filter((el) => el !== val));
			} else {
				setSelectedArr([...selectedArr, val]);
			}
		else {
			setSelectedArr([val]);
		}
	};

	return (
		<div className="reportPage-div">
			{colNames && colNames.length && (
				<div className="dropdown-div">
					<select
						className="report-dropdown"
						value={firstSelectCol}
						onChange={(e) => {
							setSelectedArr();
							setFirstSelectCol(e.target.value);
						}}
					>
						{colNames &&
							colNames[0] &&
							colNames.map((val, index) => {
								return (
									index !== 0 && (
										<option key={index}>{val}</option>
									)
								);
							})}
					</select>

					{selectedArr && (
						<select
							className="report-dropdown"
							// defaultValue={distinctVal[firstSelectCol]}
							defaultValue={[
								selectedArr &&
									selectedArr.length &&
									selectedArr,
							]}
							multiple
						>
							{distinctArr &&
								distinctArr.length > 0 &&
								distinctArr.map((val, index) => {
									return (
										val[firstSelectCol] !== "" && (
											<option
												key={index}
												onClick={(e) =>
													SelectCheckbox(
														e,
														val[firstSelectCol]
													)
												}
											>
												{val[firstSelectCol]}
											</option>
										)
									);
								})}
						</select>
					)}
					<select
						className="report-dropdown"
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
				</div>
			)}
			{secondSelectCol && selectedArr && (
				<div>
					<PieChart
						selectedSheet={selectedSheet}
						firstSelectCol={firstSelectCol}
						secondSelectCol={secondSelectCol}
						selectedArr={selectedArr}
						setSelectedArr={setSelectedArr}
						GetDistinctEntries={GetDistinctEntries}
					/>
				</div>
			)}
		</div>
	);
};

export default ReportPage;
