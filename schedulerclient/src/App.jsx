import './App.css'
import { useState } from "react";
import Button from "react-bootstrap/Button";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";

function App() {
    const [date, setDate] = useState('2025-10-30');
    const [data, setData] = useState([]);
    const [fetchString, setFetchString] = useState('http://localhost:5000/Schedule/Get');

    //useEffect(() => {
    //    fetch(fetchString)
    //        .then(response => response.json())
    //        .then(data => setData(data.Sessions))
    //        .catch(error => console.error('Error fetching data:', error));
    //});

    const handleChange = (event) => {
        setDate(event.target.value);
    };

    const onDateChanged = (event) => {
        event.preventDefault();
        setFetchString('http://localhost:5000/Schedule/Get/' + date);

        fetch(fetchString)
            .then(response => response.json())
            .then(data => {
                console.log(data);
                if (data != null) {
                    
                        setData(data.Sessions);
                    
                }
            })
            .catch(error => console.error('Error fetching data:', error));
    }

    return (
        <div>
            <h1>Scheduler</h1>

            <form onSubmit={onDateChanged}>
                <Row>
                    <Col>
                        <label>Date
                            <input
                                type="text"
                                value={date}
                                onChange={handleChange}
                            />

                        </label>
                        
                    </Col>
                    <Col>
                        <Button type="Submit">Submit</Button>
                    </Col>
                </Row>
            </form>

            <table>
                <thead>
                    <tr>
                        <th>Hour</th>
                        <th>Type</th>
                        <th>Hosts</th>
                        <th>Guests</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) =>
                    (
                        item ?
                            (
                                <tr key={index}>
                                    <td>{item.Hour}:00</td>
                                    <td>{item.Type}</td>
                                    <td>{item.Hosts}</td>
                                    <td>{item.Guests}</td>
                                </tr>
                            ) : (
                                <tr key={index}>
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

export default App
