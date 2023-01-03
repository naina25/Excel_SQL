import React, { useState } from "react";
import SheetData from "./SheetData";
import SheetsDropdown from "./SheetsDropdown";

const Sheet = ({ source }) => {
	const [sheets, setSheets] = useState([]);
	const [isLoadingSheetNames, setIsLoadingSheetNames] = useState(true);
	const [isLoadingSheetData, setIsLoadingSheetData] = useState(false);
	const [selectedSheet, setSelectedSheet] = useState("");
	return (
		<div>
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
			{!isLoadingSheetNames && (
				<SheetData
					isLoadingSheetData={isLoadingSheetData}
					setIsLoadingSheetData={setIsLoadingSheetData}
					selectedSheet={selectedSheet}
					setSelectedSheet={setSelectedSheet}
					sheets={sheets}
				/>
			)}
		</div>
	);
};

export default Sheet;
