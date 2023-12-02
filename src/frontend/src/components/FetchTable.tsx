import React, { useState } from 'react';
import TableRow from './TableRow';
import DisplayData from './DisplayData';
import { Item } from '../types/types';

const FetchTable: React.FC = () => {
    const [data, setData] = useState<Item[]>([]);
    const baseUrl = `http://localhost:55448/`;
    const itemUrl = `item/`;

    const fetchData = async (id: number) => {
        try {
            let url = `${baseUrl}${itemUrl}${id}`;
            const response = await fetch(url);
            const responseData: Item = await response.json();
            setData((prevData) => [...prevData, responseData]);
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

    return (
        <div>
            <table>
                <thead>
                <tr>
                    <th>Label</th>
                    <th>Input 1</th>
                    <th>Input 2</th>
                    <th>Action</th>
                </tr>
                </thead>
                <tbody>
                {[1, 2, 3, 4, 5].map((row) => (
                    <TableRow key={row} row={row} onFetchData={fetchData} />
                ))}
                </tbody>
            </table>

            {data.length > 0 && <DisplayData data={data} />}
        </div>
    );
};

export default FetchTable;
