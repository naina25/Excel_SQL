import React, { useEffect } from "react";
import { GetSheetNames } from "./SheetsDropdown";
import "./SheetsDropdown.css";

const SheetsDropdown = ({
    isLoadingSheetNames,
    setIsLoadingSheetNames,
    selectedSheet,
    setSelectedSheet,
    sheets,
    setSheets,
    source,
}) => {
    useEffect(() => {
        GetSheetNames(
            setIsLoadingSheetNames,
            source,
            setSheets,
            setSelectedSheet
        );
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
