import React, { useEffect } from "react";
import axios from "axios";

const SheetsDropdown = ({
	isLoadingSheetNames,
	setIsLoadingSheetNames,
	setSheetData,
	setIsLoadingSheetData,
	selectedSheet,
	setSelectedSheet,
	sheets,
	setSheets,
	source,
}) => {
	useEffect(() => {
		const getSheets = () => {
			setIsLoadingSheetNames && setIsLoadingSheetNames(true);
			axios
				.get(`https://localhost:7108/api/ExcelData/${source}`)
				.then((res) => {
					setSheets(res.data);
					setSelectedSheet(() =>
						res.data[0].Table ? res.data[0].Table : res.data[0]
					);
					setIsLoadingSheetNames && setIsLoadingSheetNames(false);
				});
		};
		getSheets();
	}, [source]);

	return (
		<div>
			{!isLoadingSheetNames ? (
				<select
					className="sheet-dropdown"
					value={selectedSheet}
					onChange={(e) => setSelectedSheet(e.target.value)}
					disabled={sheets[0] ? false : true}
				>
					{sheets.map((sheet, index) => {
						return (
							<option className="sheet-options" key={index}>
								{sheet.Table ? sheet.Table : sheet}
							</option>
						);
					})}
				</select>
			) : (
				<h3>Loading Sheet Names...</h3>
			)}
		</div>
	);
};

export default SheetsDropdown;
