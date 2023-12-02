import React, { useState } from 'react';

interface TableRowProps {
    row: number;
    onFetchData: (id: number) => void;
}

const TableRow: React.FC<TableRowProps> = ({ row, onFetchData }) => {
    const [inputs, setInputs] = useState<string[]>(['', '', '']);

    const handleInputChange = (index: number, value: string) => {
        const newInputs = [...inputs];
        newInputs[index] = value;
        setInputs(newInputs);
    };

    return (
        <tr>
            <td>Label {row}</td>
    <td>
    <input
        type="text"
    value={inputs[0]}
    onChange={(e) => handleInputChange(0, e.target.value)}
    />
    </td>
    <td>
    <input
        type="text"
    value={inputs[1]}
    onChange={(e) => handleInputChange(1, e.target.value)}
    />
    </td>
    <td>
    <button onClick={() => onFetchData(row)}>Fetch Data</button>
    </td>
    </tr>
);
};

export default TableRow;
