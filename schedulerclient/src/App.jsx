import './App.css'
import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Scheduler from './components/Scheduler';
import Main from './components/Main';
import { useState } from 'react';
import axios from 'axios';

function App() {

    //const [token, setToken] = useState();
    const [login, setLogin] = useState({username: "", password: ""});
    function UpdateLogin(e) {
        const { name, value } = e.target;
        setLogin({
            ...login,
            [name]: value
        });
    }
    async function SubmitLogin(e) {
        e.preventDefault();
        let username = login.username;
        let password = login.password;

        try {
            const response = await axios.post('http://localhost:5000/login', {
                username,
                password
            });
            localStorage.setItem("token", response.data.token);
            alert("Login successful!");
        } catch (error) {
            alert("Login failed: " + error);
        }
    }

    return (
        <div>
            <form className="loginForm" onSubmit={SubmitLogin}>
                <label>username<input type="text" name="username" onChange={UpdateLogin} /></label>
                <label>password<input type="password" name="password" onChange={UpdateLogin} /></label>
                <button type="submit">Login</button>
            </form>
            <Router>
                <Routes>
                    <Route path="/" element={<Main />} />
                    <Route path="/scheduler" element={<Scheduler />} />
                </Routes>
            </Router>
        </div>
    );
}

export default App