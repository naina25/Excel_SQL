import "./App.css";
import MainContent from "./components/MainContent/MainContent";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Navbar from "./components/Navbar/Navbar";

function App() {
    return (
        <div className="App">
            <Router>
                <Navbar />
                <Routes>
                    <Route
                        exact
                        path="/"
                        element={
                            <MainContent source="sheets" isReport={false} />
                        }
                    />
                    <Route
                        path="/TablesList"
                        element={
                            <MainContent source="sqltables" isReport={false} />
                        }
                    />
                    <Route
                        path="/Report"
                        element={
                            <MainContent source="sqltables" isReport={true} />
                        }
                    />
                </Routes>
            </Router>
        </div>
    );
}

export default App;
