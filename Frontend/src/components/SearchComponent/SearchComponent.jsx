import { faMagnifyingGlass } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React, { useEffect, useState } from "react";
import { GetSearchedData } from "./SearchComponent";
import "./SearchComponent.css";

const SearchComponent = ({
    selectedSheet,
    setSheetData,
    setIsLoadingSheetData,
}) => {
    const [searchQuery, setSearchQuery] = useState("");

    useEffect(() => {
        GetSearchedData(
            searchQuery,
            selectedSheet,
            setIsLoadingSheetData,
            setSheetData
        );
    }, [searchQuery]);

    return (
        <div className="search-div">
            <FontAwesomeIcon icon={faMagnifyingGlass} />
            <input
                onChange={(e) => setSearchQuery(e.target.value)}
                type="text"
                placeholder="Type to Search..."
            ></input>
        </div>
    );
};

export default SearchComponent;
