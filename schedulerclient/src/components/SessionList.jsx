import { useState } from "react";
import { useEffect } from "react";
function SessionList({ listdate }) {
    const [sessions, setSessions] = useState([]);
    useEffect(() => {
        const fetchSessions = () => {
            fetch('http://localhost:5000/Schedule/Get/' + listdate)
                .then(response => response.json())
                .then(data => { console.log(data); if (data != null) { setSessions(data.Sessions); } })
                .catch(error => console.error('Error fetching data:', error));
        }
        fetchSessions();
        
    }, []);
    

    const typeMap = (type) => {
        switch (type) {
            case 0:
                return "Music";
            case 1:
                return "Prerecorded";
            case 2:
                return "Live";
            default:
                return "Unkown";
        }
    }

    return (
        <div className="SessionTable">
            <h3>{listdate}</h3>
            <table>
                <thead>
                    <tr>
                        <td>Time</td>
                        <td>Type</td>
                    </tr>
                </thead>
                <tbody className="SessionList">
                    {sessions.map((item, index) =>
                    (
                        item ? (
                            <tr key={index}>
                                <td>{item.Hour}:00</td>
                                <td>{typeMap(item.Type)}</td>
                            </tr>
                        ) : (
                            <tr key={index}>
                                <td></td>
                                <td></td>
                            </tr>
                        )
                    )
                    )}
                </tbody>
            </table>
        </div>
       
    );
}

export default SessionList;