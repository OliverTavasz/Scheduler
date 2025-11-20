import './App.css'
import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Scheduler from './components/Scheduler';
import Main from './components/Main';
import { useState } from 'react';
function App() {

    

    return (
        <Router>
            <Routes>
                <Route path="/" element={<Main/> } />
                <Route path="/scheduler" element={<Scheduler />} />
            </Routes>
        </Router>
    );
}

export default App