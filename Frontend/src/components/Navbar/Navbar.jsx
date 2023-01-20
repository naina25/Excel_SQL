import { faBars, faXmark } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React, { useState } from "react";
import { NavLink } from "react-router-dom";
import SearchComponent from "../SearchComponent/SearchComponent.jsx";
import "./Navbar.css";

const Navbar = () => {
    const [isHamOpen, setIsHamOpen] = useState(false);

    const handleClick = () => {
        setIsHamOpen(!isHamOpen);
    };
    return (
        <div className="navbar-div">
            <div className={`left-nav${isHamOpen ? " hamStyles" : ""}`}>
                <div onClick={handleClick} className="hamburger">
                    {!isHamOpen ? (
                        <FontAwesomeIcon icon={faBars} />
                    ) : (
                        <FontAwesomeIcon icon={faXmark} />
                    )}
                </div>
                <div className={`left-nav-group${isHamOpen ? " openHam" : ""}`}>
                    <div className="nav-brand">Story Tracker</div>
                    <div className={`navlinks${isHamOpen ? " openHam" : ""}`}>
                        <NavLink to="/">Home</NavLink>
                        <NavLink to="/TablesList">Tables</NavLink>
                        <NavLink to="/Report">Report</NavLink>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Navbar;
