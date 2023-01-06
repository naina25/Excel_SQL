import axios from "axios";
import React, { useEffect, useState } from "react";
import SearchIcon from "../assets/searchIcon.png";

const SearchComponent = ({
    selectedSheet,
    setSheetData,
    setIsLoadingSheetData,
}) => {
    const [searchQuery, setSearchQuery] = useState("");

    useEffect(() => {
        const GetSearchedData = () => {
            const getUrl = searchQuery
                ? `https://localhost:7108/api/ExcelData/sheets/search/${selectedSheet}`
                : `https://localhost:7108/api/ExcelData/sheets/${selectedSheet}`;
            setIsLoadingSheetData(true);
            axios
                .get(getUrl, {
                    params: searchQuery && { searchQuery: searchQuery },
                })
                .then((res) => {
                    setSheetData(res.data);
                    setIsLoadingSheetData(false);
                });
        };

        GetSearchedData();
    }, [searchQuery]);

    return (
        <div className="search-div">
            <div className="searchIcon">
                <img src={SearchIcon} alt="" />
            </div>
            <input
                onChange={(e) => setSearchQuery(e.target.value)}
                type="text"
                placeholder="Type to Search..."
            ></input>
        </div>
    );
};

export default SearchComponent;
