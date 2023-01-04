import React, { useState } from "react";
import ReportPage from "./ReportPage";
import SheetData from "./SheetData";
import SheetsDropdown from "./SheetsDropdown";

const Sheet = ({ source, isReport }) => {
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
            {!isReport ? (
                !isLoadingSheetNames && (
                    <SheetData
                        isLoadingSheetData={isLoadingSheetData}
                        setIsLoadingSheetData={setIsLoadingSheetData}
                        selectedSheet={selectedSheet}
                        setSelectedSheet={setSelectedSheet}
                        sheets={sheets}
                    />
                )
            ) : (
                <ReportPage selectedSheet={selectedSheet} />
            )}
        </div>
    );
};

export default Sheet;
