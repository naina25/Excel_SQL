import React from "react";
import { NavLink } from "react-router-dom";

const Sidebar = () => {
    return (
        <div className="sidebar-div">
            <div className="navlinks">
                <NavLink to="/">Home</NavLink>
                <NavLink to="/TablesList">Tables</NavLink>
                <NavLink to="/Report">Report</NavLink>
            </div>
        </div>
    );
};
export default Sidebar;
