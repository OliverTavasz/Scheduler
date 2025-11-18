import { useState } from "react";
import SessionList from './SessionList';
function Home() {

    const [progress, setProgress] = useState(0);

    const handleChange = (event) => {
        setProgress(event.target.value);
    };

    return (
        <div>
            <SessionList listdate="2025-05-05" />
            <input
                type="number"
                value={progress}
                onChange={handleChange}
                min="0"
                max="100"
            />
            <div className="barcontainer">
                <div className="bar" style={{ width: `${progress}%` }}></div>
            </div>
        </div>
    );
}

export default Home;