import React, { useState } from "react";
import { useEffect } from "react";
import PieChart from "../PieChart/PieChart.jsx";
import {
    getColumnNames,
    GetDistinctEntries,
    SelectCheckbox,
} from "./ReportPage";
import "./ReportPage.css";

const ReportPage = ({ selectedSheet }) => {
    const [colNames, setColNames] = useState();
    const [firstSelectCol, setFirstSelectCol] = useState();
    const [distinctArr, setDistinctArr] = useState();
    const [secondSelectCol, setSecondSelectCol] = useState();
    const [selectedArr, setSelectedArr] = useState();

    useEffect(() => {
        selectedSheet && getColumnNames(selectedSheet, setColNames);
        selectedSheet && setFirstSelectCol();
        selectedSheet && setSecondSelectCol();
    }, [selectedSheet]);

    useEffect(() => {
        setSelectedArr();
        colNames && colNames.length && setFirstSelectCol(colNames[1]);
        colNames && colNames.length && setSecondSelectCol(colNames[2]);
    }, [colNames]);

    useEffect(() => {
        firstSelectCol &&
            GetDistinctEntries(
                selectedSheet,
                firstSelectCol,
                setDistinctArr,
                setSelectedArr
            );
    }, [firstSelectCol]);

    return (
        <div className="reportPage-div">
            {colNames && colNames.length && (
                <div className="dropdown-div">
                    <label for="col1">
                        Select Column 1 for visual data representation
                    </label>
                    <select
                        id="col1"
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
                    <label for="col2">
                        Select entry/entries for above selected column
                    </label>

                    {selectedArr && (
                        <select
                            id="col2"
                            className="report-dropdown"
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
                                                        val[firstSelectCol],
                                                        selectedArr,
                                                        setSelectedArr
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
                    <label for="col3">
                        Select Column 2 for visual data representation
                    </label>

                    <select
                        id="col3"
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
