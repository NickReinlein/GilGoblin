import React, { useState } from 'react';

const App = () => {
    const [list, setList] = useState([]);
    const [selectedValue, setSelectedValue] = useState('34');
    const baseUrl = `http://localhost:5432/`;

    const fetchData = async () => {
        try {
            const response = await fetch(`${baseUrl}${selectedValue}`);
            const data = await response.json();
            setList(data); // Update state with fetched data
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

    const handleButtonClick = () => {
        fetchData();
    };

    const handleDropdownChange = (event: { target: { value: React.SetStateAction<string>; }; }) => {
        setSelectedValue(event.target.value);
    };

    return (
        <div>
            <select value={selectedValue} onChange={handleDropdownChange}>
                <option value="34">34</option>
                {/* Add other dropdown options if needed */}
            </select>
            <button onClick={handleButtonClick}>Fetch Data</button>
            {list.length > 0 && (
                <div>
                    <h2>Fetched List:</h2>
                    <ul>
                        {list.map((item, index) => (
                            <li key={index}>{item}</li>
                            // Replace {item} with appropriate data from the fetched list
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};

export default App;
