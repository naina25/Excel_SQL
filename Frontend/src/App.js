import "./App.css";
import Sheet from "./components/Sheet";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
// import TablesList from "./components/TablesList";

function App() {
    return (
        <div className="App">
            <Router>
                <Routes>
                    <Route exact path="/" element={<Sheet source="sheets" />} />
                    <Route
                        path="/TablesList"
                        element={<Sheet source="sqlsheets" />}
                    />
                </Routes>
            </Router>
        </div>
    );
}

export default App;
