import { useState } from "react";
import Button from "react-bootstrap/Button";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import Stack from "react-bootstrap/Stack";
function Scheduler() {
    const [date, setDate] = useState('2025-10-30');
    const [data, setData] = useState([]);
    const [fetchString, setFetchString] = useState('http://localhost:5000/Schedule/Get');

    const [newSession, setNewSession] = useState({
        date: '',
        hours: '',
        type: 0,
        hosts: '',
        guests: ''
    });

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

    const handleDeleteSession = async (event) => {
        event.preventDefault();

        const fulldate = event.target.elements.date.value + " " + event.target.elements.hour.value + ":00";
        const urlSafeString = encodeURIComponent(fulldate);

        try {
            const response = await fetch('http://localhost:5000/Session/Remove' + '?date=' + urlSafeString, {
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

        reloadSessions();
    };


    const reloadSessions = () => {
        setFetchString('http://localhost:5000/Schedule/Get/' + date);

        setData([]);

        fetch(fetchString)
            .then(response => response.json())
            .then(data => {
                console.log(data);
                if (data != null) {

                    setData(data.Sessions);

                }
            })
            .catch(error => console.error('Error fetching data:', error));
        console.log(data);
    }


    const handleDateChange = (event) => {
        setDate(event.target.value);
    };

    const onDateChanged = (event) => {
        event.preventDefault();
        reloadSessions();
    };

    const handleNewSessionChange = (event) => {
        const { name, value } = event.target;
        setNewSession({
            ...newSession,
            [name]: value
        });
    };

    const handleNewSession = async (event) => {
        event.preventDefault();

        const params = new URLSearchParams();
        params.append('date', newSession.date);
        params.append('hours', newSession.hours);
        params.append('type', Number(newSession.type));
        params.append('hosts', newSession.hosts || '');
        params.append('guests', newSession.guests || '');

        console.log(params.toString());

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

        reloadSessions();
    };


    return (
        <div>
            <h1>Scheduler</h1>
            <Row>
                <Col>
                    <form onSubmit={onDateChanged}>
                        <label>Date
                            <input
                                type="text"
                                value={date}
                                onChange={handleDateChange}
                            />
                        </label>
                        <Button type="Submit">Get</Button>
                    </form>
                </Col>
                <Col>
                    <form onSubmit={handleNewSession}>
                        <Stack>
                            <div>
                                <label>Date<input type="text" name="date" value={newSession.Date} onChange={handleNewSessionChange} required /></label>
                            </div>

                            <div>
                                <label>Hours<input type="text" name="hours" value={newSession.Hours} onChange={handleNewSessionChange} required /></label>
                            </div>

                            <div>
                                <label>Type<input type="number" name="type" value={newSession.Type} onChange={handleNewSessionChange} required /></label>
                            </div>

                            <div>
                                <label>Hosts<input type="text" name="hosts" value={newSession.Hosts} onChange={handleNewSessionChange} /></label>
                            </div>

                            <div>
                                <label>Guests<input type="text" name="guests" value={newSession.Guests} onChange={handleNewSessionChange} /></label>
                            </div>

                            <div>
                                <Button type="Submit">Add Session</Button>
                            </div>
                        </Stack>

                    </form>
                </Col>
            </Row>
            <table>
                <thead>
                    <tr>
                        <th>Hour</th>
                        <th>Type</th>
                        <th>Hosts</th>
                        <th>Guests</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) =>
                    (
                        item ? (
                            <tr key={index}>
                                <td>{item.Hour}:00</td>
                                <td>{typeMap(item.Type)}</td>
                                <td>{item.Hosts}</td>
                                <td>{item.Guests}</td>
                                <td>
                                    <form onSubmit={handleDeleteSession}>
                                        <input type="hidden" name="date" value={item.date} />
                                        <input type="hidden" name="hour" value={item.Hour} />
                                        <Button type="Submit">Delete</Button>
                                    </form>
                                </td>
                            </tr>
                        ) : (
                            <tr key={index}>
                                <td></td>
                                <td></td>
                                <td></td>
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

export default Scheduler;