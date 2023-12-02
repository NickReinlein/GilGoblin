import React, { useState } from 'react';

const App = () => {
    const [list, setList] = useState([]);
    const [selectedValue, setSelectedValue] = useState('34');
    const baseUrl = `http://localhost:55448/`;
    const itemUrl = `item/`;

    const fetchData = async (inputValue: number) => {
        try {
            let url = `${baseUrl}${itemUrl}${inputValue}`;
            const response = await fetch(url);
            const data = await response.json();
            console.log(data.toString());
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

    const handleButtonClick = async () => {
        let inputNumber = parseInt((selectedValue || '').trim(), 10);
        if (inputNumber > 0)
            await fetchData(inputNumber);
    };

    const handleDropdownChange = (event: { target: { value: React.SetStateAction<string>; }; }) => {
        setSelectedValue(event.target.value);
    };



    return (
        <div>
            <select value={selectedValue} onChange={handleDropdownChange}>
                <option value="1639">1639</option>
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
