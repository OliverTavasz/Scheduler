import { useState } from "react";
import Button from "react-bootstrap/Button";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import Stack from "react-bootstrap/Stack";
import TypeMap from "./TypeMap"
function Scheduler() {

    const currentDate = new Date();
    const formattedDate = currentDate.toISOString().slice(0, 10);

    const [date, setDate] = useState(formattedDate);
    const [SessionList, setSessionList] = useState([]);
    
    const [newSession, setNewSession] = useState({
        name: "",
        type: 0,
        date: "",
        startHour: 0,
        endHour: 0,
        hosts: "",
        guests: ""
    });

    const UpdateDate = (e) => {
        setDate(e.target.value);
    };

    const SubmitDate = (e) => {
        e.preventDefault();
        fetch('http://localhost:5000/Schedule/Get/' + date)
            .then(response => response.json())
            .then(data => {
                console.log(data);
                if (data != null && data.length != 0) {
                    setSessionList(data);
                }
            })
            .catch(error => console.error('Error fetching data:', error));
    };

    const UpdateNewSession = (e) => {
        const { name, value } = e.target;
        setNewSession({
            ...newSession,
            [name]: value
        });
    };

    const SubmitNewSession = async (e) => {
        e.preventDefault();

        const params = new URLSearchParams();
        params.append("name", newSession.name);
        params.append("type", Number(newSession.type));
        params.append("dateOnly", newSession.date);
        params.append("startHour", Number(newSession.startHour));
        params.append("endHour", Number(newSession.endHour));
        params.append("hosts", newSession.hosts || "");
        params.append("guests", newSession.guests || "");

        try {
            const response = await fetch('http://localhost:5000/Session/Add' + '?' + params.toString(), {
                method: 'POST'
            });
            if (response.ok) {
                const result = await response.json();
                console.log('Data submitted successfully:', result);
            } else {
                console.error('Error submitting data:', response.statusText);
            }
        } catch (error) {
            console.error('Unexpected error:', error);
        }

        SubmitDate(e);
    };

    return (
        <div>
            <h1>Scheduler</h1>
            <div>
                <h3>{date}</h3>
                <form onSubmit={SubmitDate}>
                    <label>
                        Date <input type="text" value={date} onChange={UpdateDate} />
                    </label>
                    <button type="submit">Submit</button>
                </form>
                <table className="sessionList sessionListL">
                    <thead>
                        <tr>
                            <th className="gridItem">Time</th>
                            <th className="gridItem">Name</th>
                            <th className="gridItem">Type</th>
                            <th className="gridItem">Hosts</th>
                            <th className="gridItem">Guests</th>
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
                                    <td className="gridItem">{item.Hosts}</td>
                                    <td className="gridItem">{item.Guests}</td>
                                </tr>
                            ) : (
                                <tr key={index}>
                                    <td>-</td>
                                    <td>-</td>
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
            <div>
                <form onSubmit={SubmitNewSession}>
                    <Stack>
                        <label>Name<input type="text" name="name" onChange={UpdateNewSession} /></label>
                        <label>Type<input type="number" min="0" max="4" step="1" name="type" onChange={UpdateNewSession} /></label>
                        <label>Date<input type="text" name="date" onChange={UpdateNewSession} /></label>
                        <label>From hour<input type="number" min="0" max="23" step="1" name="startHour" onChange={UpdateNewSession} /></label>
                        <label>To hour<input type="number" min={newSession.startHour} max="23" step="1" name="endHour" onChange={UpdateNewSession} /></label>
                        <label>Hosts<input type="text" name="hosts" onChange={UpdateNewSession} /></label>
                        <label>Guests<input type="text" name="guests" onChange={UpdateNewSession} /></label>
                        <button type="submit">Add new session</button>
                    </Stack>
                </form>
            </div>
        </div>
    );
}

export default Scheduler;