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
            console.log("dropdown");
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

    // useEffect(() => {
    //     const getSheetData = () => {
    //         setIsLoadingSheetData(true);
    //         const sheetUrl = `https://localhost:7108/api/ExcelData/sheets/${selectedSheet}`;
    //         axios
    //             .get(sheetUrl)
    //             .then((res) => {
    //                 setSheetData(res.data);
    //                 setIsLoadingSheetData(false);
    //             })
    //             .catch((err) => {
    //                 return console.log(err);
    //             });
    //     };
    //     selectedSheet
    //         ? getSheetData()
    //         : sheets[0] && setSelectedSheet(sheets[0]);
    // }, [
    //     sheets,
    //     selectedSheet,
    //     setSheetData,
    //     setSelectedSheet,
    //     setIsLoadingSheetData,
    // ]);

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
                        // console.log(sheets);
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
