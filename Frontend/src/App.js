import "./App.css";
import Sheet from "./components/Sheet";
import {
    BrowserRouter as Router,
    NavLink,
    Route,
    Routes,
} from "react-router-dom";
// import TablesList from "./components/TablesList";

function App() {
    return (
        <div className="App">
            <Router>
                <NavLink to={"/TablesList"}>Tables</NavLink>
                <NavLink to={"/Report"}>Report</NavLink>
                <Routes>
                    <Route
                        exact
                        path="/"
                        element={<Sheet source="sheets" isReport={false} />}
                    />
                    <Route
                        path="/TablesList"
                        element={<Sheet source="sqltables" isReport={false} />}
                    />
                    <Route
                        path="/Report"
                        element={<Sheet source="sqltables" isReport={true} />}
                    />
                </Routes>
            </Router>
        </div>
    );
}

export default App;
