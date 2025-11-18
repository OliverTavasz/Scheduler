import './App.css'
import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Scheduler from './components/Scheduler';
import Home from './components/Home';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Home/> } />
                <Route path="/scheduler" element={<Scheduler />} />
            </Routes>
        </Router>
    );
}

export default App