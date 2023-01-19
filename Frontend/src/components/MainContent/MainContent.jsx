import React, { useState } from "react";
import ReportPage from "../ReportPage/ReportPage.jsx";
import SearchComponent from "../SearchComponent/SearchComponent.jsx";
import SheetData from "../SheetData/SheetData.jsx";
import SheetsDropdown from "../SheetsDropdown/SheetsDropdown.jsx";
import "./MainContent.css";

const MainContent = ({ source, isReport }) => {
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

export default MainContent;
