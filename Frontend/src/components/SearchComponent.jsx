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
        console.log(selectedSheet);
        console.log(searchQuery);
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
                    console.log(res.data);
                    setSheetData(res.data);
                    setIsLoadingSheetData(false);
                });
        };

        GetSearchedData();
        // searchQuery ? GetSearchedData() : setIsLoadingSheetData(false);
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
