import React, { useState } from "react";
import ReportPage from "./ReportPage";
import SearchComponent from "./SearchComponent";
import SheetData from "./SheetData";
import SheetsDropdown from "./SheetsDropdown";

const Sheet = ({ source, isReport }) => {
	const [sheets, setSheets] = useState([]);
	const [isLoadingSheetNames, setIsLoadingSheetNames] = useState(true);
	const [isLoadingSheetData, setIsLoadingSheetData] = useState(false);
	const [selectedSheet, setSelectedSheet] = useState("");
	const [sheetData, setSheetData] = useState();

	return (
		<div className="content-div">
			<div className="sheet-div">
				<SheetsDropdown
					isLoadingSheetNames={isLoadingSheetNames}
					setIsLoadingSheetNames={setIsLoadingSheetNames}
					setIsLoadingSheetData={setIsLoadingSheetData}
					selectedSheet={selectedSheet}
					setSelectedSheet={setSelectedSheet}
					sheets={sheets}
					setSheets={setSheets}
					source={source}
				/>
				{!isReport && !isLoadingSheetNames && (
					<SearchComponent
						selectedSheet={selectedSheet}
						setSheetData={setSheetData}
						isLoadingSheetData={isLoadingSheetData}
						setIsLoadingSheetData={setIsLoadingSheetData}
					/>
				)}
			</div>

			{!isReport ? (
				!isLoadingSheetNames && (
					<SheetData
						isLoadingSheetData={isLoadingSheetData}
						setIsLoadingSheetData={setIsLoadingSheetData}
						selectedSheet={selectedSheet}
						setSelectedSheet={setSelectedSheet}
						sheets={sheets}
						sheetData={sheetData}
						setSheetData={setSheetData}
					/>
				)
			) : (
				<ReportPage selectedSheet={selectedSheet} />
			)}
		</div>
	);
};

export default Sheet;
