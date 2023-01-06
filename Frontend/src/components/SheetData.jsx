import React, { useState, useEffect } from "react";
import axios from "axios";
import PopupMessage from "./PopupMessage";
import UpArrow from "../assets/UpArrow.png";
import DownArrow from "../assets/DownArrow.png";

const SheetData = ({
    isLoadingSheetData,
    setIsLoadingSheetData,
    selectedSheet,
    setSelectedSheet,
    sheets,
    sheetData,
    setSheetData,
}) => {
    const [editData, setEditData] = useState();
    const [hasDataUpdated, setHasDataUpdated] = useState(false);
    const [sortCol, setSortCol] = useState();
    const [order, setOrder] = useState();

    const putRow = (editData) => {
        const jsonStr = JSON.stringify(editData).replaceAll('"', '\\"');
        axios
            .put(
                `https://localhost:7108/api/ExcelData/sheets/${selectedSheet}/edit`,
                `${jsonStr}`,
                { headers: { "content-type": "application/json" } }
            )
            .then((res) => {
                console.log("Updated Successfully");
                setHasDataUpdated(!hasDataUpdated);
            });
    };

    const GetSortedData = (selectedSheet, col, sortOrder) => {
        setIsLoadingSheetData && setIsLoadingSheetData(true);
        setOrder(sortOrder);
        axios
            .get(
                `https://localhost:7108/api/ExcelData/sheets/sort/${selectedSheet}`,
                { params: { column: col, order: sortOrder } }
            )
            .then((res) => {
                setSheetData(() => res.data);
                setIsLoadingSheetData && setIsLoadingSheetData(false);
            });
    };

    useEffect(() => {
        const getSheetData = () => {
            isLoadingSheetData !== undefined && setIsLoadingSheetData(true);
            const sheetUrl = `https://localhost:7108/api/ExcelData/sheets/${selectedSheet}`;
            axios
                .get(sheetUrl)
                .then((res) => {
                    console.log(res.data);
                    setSheetData && setSheetData(res.data);
                    setIsLoadingSheetData && setIsLoadingSheetData(false);
                })
                .catch((err) => {
                    return console.log(err);
                });
        };
        selectedSheet
            ? getSheetData()
            : sheets[0] && setSelectedSheet(sheets[0]);
    }, [selectedSheet]);

    useEffect(() => {
        hasDataUpdated &&
            setTimeout(() => {
                console.log("hey");
                setHasDataUpdated(!hasDataUpdated);
            }, 3000);
    }, [hasDataUpdated]);

    return (
        <>
            <div className={hasDataUpdated ? "showPopup" : "hidePopup"}>
                <PopupMessage />
            </div>
            <div className="sheet-table-div">
                {!isLoadingSheetData ? (
                    sheetData && sheetData.length > 0 ? (
                        <table className="xl-table">
                            <thead>
                                <tr className="table-head">
                                    {sheetData &&
                                        sheetData.length &&
                                        Object.keys(sheetData[0]).map(function (
                                            keyName,
                                            keyIndex
                                        ) {
                                            return (
                                                keyIndex !== 0 && (
                                                    <th
                                                        key={keyIndex}
                                                        className="table-td"
                                                        onClick={() => {
                                                            setSortCol(keyName);
                                                            GetSortedData(
                                                                selectedSheet,
                                                                keyName,
                                                                sortCol !==
                                                                    keyName
                                                                    ? "ASC"
                                                                    : order ===
                                                                      "ASC"
                                                                    ? "DESC"
                                                                    : "ASC"
                                                            );
                                                        }}
                                                    >
                                                        {keyName}
                                                        {sortCol ===
                                                            keyName && (
                                                            <span className="sort-arrows">
                                                                {order ===
                                                                "ASC" ? (
                                                                    <img
                                                                        src={
                                                                            UpArrow
                                                                        }
                                                                        alt="Ascending"
                                                                    />
                                                                ) : (
                                                                    <img
                                                                        src={
                                                                            DownArrow
                                                                        }
                                                                        alt="Descending"
                                                                    />
                                                                )}
                                                            </span>
                                                        )}
                                                    </th>
                                                )
                                            );
                                        })}
                                </tr>
                            </thead>
                            <tbody>
                                {sheetData &&
                                    Object.entries(sheetData).map(
                                        ([key, value], index) => {
                                            return (
                                                <tr
                                                    key={index}
                                                    className={`${
                                                        index % 2 === 0 &&
                                                        "row-bg "
                                                    }table-row`}
                                                >
                                                    {Object.keys(value).map(
                                                        (valueKey, ind) => {
                                                            return (
                                                                ind !== 0 && (
                                                                    <td
                                                                        key={
                                                                            ind
                                                                        }
                                                                        className="table-td"
                                                                    >
                                                                        <textarea
                                                                            rows="4"
                                                                            defaultValue={
                                                                                value[
                                                                                    valueKey
                                                                                ]
                                                                            }
                                                                            onKeyUp={(
                                                                                e
                                                                            ) => {
                                                                                if (
                                                                                    e.key ===
                                                                                    "Enter"
                                                                                ) {
                                                                                    putRow(
                                                                                        editData
                                                                                    );
                                                                                    e.preventDefault();
                                                                                    e.target.blur();
                                                                                } else {
                                                                                    setEditData(
                                                                                        () => {
                                                                                            return {
                                                                                                ...value,
                                                                                                [valueKey]:
                                                                                                    e
                                                                                                        .target
                                                                                                        .value,
                                                                                            };
                                                                                        }
                                                                                    );
                                                                                }
                                                                            }}
                                                                        />
                                                                    </td>
                                                                )
                                                            );
                                                        }
                                                    )}
                                                </tr>
                                            );
                                        }
                                    )}
                            </tbody>
                        </table>
                    ) : (
                        <h4>No data</h4>
                    )
                ) : (
                    <h3>Loading Sheet Data...</h3>
                )}
            </div>
        </>
    );
};

export default SheetData;
