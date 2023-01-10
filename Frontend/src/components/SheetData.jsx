import React, { useState, useEffect } from "react";
import axios from "axios";
import PopupMessage from "./PopupMessage";
import UpArrow from "../assets/UpArrow.png";
import DownArrow from "../assets/DownArrow.png";
import { Col, Container, Row } from "react-bootstrap";

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
																					editData
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
