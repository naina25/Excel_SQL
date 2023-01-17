import React, { useState, useEffect } from "react";
import PopupMessage from "../PopupMessage/PopupMessage";
import UpArrow from "../../assets/UpArrow.png";
import DownArrow from "../../assets/DownArrow.png";
import { Col, Container, Row } from "react-bootstrap";
import "./SheetData.css";
import { getSheetData, GetSortedData, putRow } from "./SheetData";

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

    useEffect(() => {
        selectedSheet
            ? getSheetData(
                  isLoadingSheetData,
                  setIsLoadingSheetData,
                  selectedSheet,
                  setSheetData
              )
            : sheets[0] && setSelectedSheet(sheets[0]);
    }, [selectedSheet]);

    useEffect(() => {
        hasDataUpdated &&
            setTimeout(() => {
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
                        <Container>
                            <Row
                                xs={2}
                                sm={4}
                                md={5}
                                lg={6}
                                xl={7}
                                xxl={9}
                                className="table-head flex-nowrap"
                            >
                                {sheetData &&
                                    sheetData.length &&
                                    Object.keys(sheetData[0]).map(
                                        (keyName, keyIndex) => {
                                            return (
                                                keyIndex !== 0 && (
                                                    <Col
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
                                                                    : "ASC",
                                                                setIsLoadingSheetData,
                                                                setOrder,
                                                                setSheetData
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
                                                    </Col>
                                                )
                                            );
                                        }
                                    )}
                            </Row>
                            {sheetData &&
                                Object.entries(sheetData).map(
                                    ([, value], index) => {
                                        return (
                                            <Row
                                                xs={2}
                                                sm={4}
                                                md={5}
                                                lg={6}
                                                xl={7}
                                                xxl={9}
                                                key={index}
                                                className={`${
                                                    index % 2 === 0 && "row-bg "
                                                }table-row flex-nowrap`}
                                            >
                                                {Object.keys(value).map(
                                                    (valueKey, ind) => {
                                                        return (
                                                            ind !== 0 && (
                                                                <Col
                                                                    key={ind}
                                                                    className="table-td"
                                                                >
                                                                    <textarea
                                                                        rows="1"
                                                                        defaultValue={
                                                                            value[
                                                                                valueKey
                                                                            ]
                                                                        }
                                                                        onKeyDown={(
                                                                            e
                                                                        ) => {
                                                                            if (
                                                                                e.key ===
                                                                                "Enter"
                                                                            ) {
                                                                                e.preventDefault();
                                                                            }
                                                                        }}
                                                                        onKeyUp={(
                                                                            e
                                                                        ) => {
                                                                            if (
                                                                                e.key ===
                                                                                "Enter"
                                                                            ) {
                                                                                // e.preventDefault();
                                                                                e.target.blur();
                                                                                putRow(
                                                                                    editData,
                                                                                    selectedSheet,
                                                                                    setHasDataUpdated,
                                                                                    hasDataUpdated
                                                                                );
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
                                                                </Col>
                                                            )
                                                        );
                                                    }
                                                )}
                                            </Row>
                                        );
                                    }
                                )}
                        </Container>
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
