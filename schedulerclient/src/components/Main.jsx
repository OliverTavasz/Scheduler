import { useState } from "react";
import { useEffect } from "react";
import TypeMap from "./TypeMap";
function Main() {
    const [SessionList, setSessionList] = useState([]);
    const [pIndex, setpIndex] = useState(-1);

    const currentDate = new Date();
    const formattedDate = currentDate.toISOString().slice(0, 10);
    let sessionIntervals;
    const [progress, setProgress] = useState(0);

    useEffect(() => {
        fetch('http://localhost:5000/Schedule/Get/' + formattedDate)
            .then(response => response.json())
            .then(data => {
                console.log(data);
                if (data != null && data.length != 0) {
                    sessionIntervals = [0];
                    let objects = [{ Hour: data[0].Hour.toString(), Name: data[0].Name, Type: data[0].Type }];

                    for (let i = 1; i < data.length; i++) {
                        let currentItem = { Hour: data[i].Hour, Name: data[i].Name, Type: data[i].Type };
                        let previousItem = { Hour: data[i - 1].Hour, Name: data[i - 1].Name, Type: data[i - 1].Type };

                        if (currentItem.Name !== previousItem.Name && currentItem.Type !== previousItem.Type) {
                            objects[objects.length - 1].Hour += ":00 - " + currentItem.Hour.toString() + ":00";
                            sessionIntervals.push(currentItem.Hour);
                            objects.push(currentItem);
                        }
                    }
                    sessionIntervals.push(24);
                    objects[objects.length - 1].Hour += ":00 - 24:00";
                    
                    setSessionList(objects);

                    var today = new Date();
                    var hour = today.getHours();
                    var sessionIndex = 0;

                    for (let i = 1; i < sessionIntervals.length - 1; i++) {
                        if (hour < sessionIntervals[i + 1] && hour >= sessionIntervals[i]) {
                            sessionIndex = i;
                            break;
                        }
                    }

                    let customDate = new Date(
                        today.getFullYear(),
                        today.getMonth(),
                        today.getDate(),
                        sessionIntervals[sessionIndex],
                        0,
                        0,
                        0
                    );

                    let fullseconds = (sessionIntervals[sessionIndex + 1] - sessionIntervals[sessionIndex]) * 3600;
                    let seconds = Math.floor((today - customDate) / 1000);
                    let prog = (seconds / fullseconds) * 100;
                    setProgress(prog);
                    setpIndex(sessionIndex);
                }
            })
            .catch(error => console.error('Error fetching data:', error));
    }, [formattedDate]);

    useEffect(() => {
        const interval = setInterval(() => {
            var today = new Date();
            var hour = today.getHours();
            var sessionIndex = 0;

            for (let i = 1; i < sessionIntervals.length - 1; i++) {
                if (hour < sessionIntervals[i + 1] && hour >= sessionIntervals[i]) {
                    sessionIndex = i;
                    break;
                }
            }

            let customDate = new Date(
                today.getFullYear(),
                today.getMonth(),
                today.getDate(),
                sessionIntervals[sessionIndex],
                0,
                0,
                0
            );

            let fullseconds = (sessionIntervals[sessionIndex + 1] - sessionIntervals[sessionIndex]) * 3600;
            let seconds = Math.floor((today - customDate) / 1000);
            let prog = (seconds / fullseconds) * 100;
            setProgress(prog);
            setpIndex(sessionIndex);
        }, 10000);

        return () => clearInterval(interval);
    }, []);

    return (
        <div className="mainPage">
            <h1>Radio Station</h1>
            <div>
                <h3>{formattedDate}</h3>
                <table className="sessionList">
                    <thead>
                        <tr>
                            <th className="gridItem">Time</th>
                            <th className="gridItem">Name</th>
                            <th className="gridItem">Type</th>
                        </tr>
                    </thead>
                    <tbody>
                        {SessionList.map((item, index) =>
                        (
                            item ? (
                                <tr key={index}>
                                    <td className="gridItem">{item.Hour}</td>
                                    <td className="gridItem">{item.Name}</td>
                                    <td className="gridItem">{TypeMap(item.Type)}</td>
                                </tr>
                            ) : (
                                <tr key={index}>
                                    <td>-</td>
                                    <td>-</td>
                                    <td>-</td>
                                </tr>
                            )
                        )
                        )}
                    </tbody>
                </table>
            </div>
            <div className="barContainer">
                {SessionList.length > 0 && pIndex !== -1 ? (<p>Playing now: {SessionList[pIndex].Name}</p>) : (<p>Loading...</p>)}
                <div className="progressBar">
                    <div className="bar" style={{ width: `${progress}%` }}></div>
                </div>
            </div>
        </div>
    );
}

export default Main;