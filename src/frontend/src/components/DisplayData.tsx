import React from 'react';
import { Item } from '../types/types';

interface DisplayDataProps {
    data: Item[];
}

const DisplayData: React.FC<DisplayDataProps> = ({ data }) => {
    return (
        <div>
            <h2>Fetched Data:</h2>
            <ul>
                {data.map((item) => (
                    <li key={item.id}>
                        ID: {item.id},
                        Name: {item.name},
                        Description: {item.description}
                        IconId: {item.iconId}
                        Level: {item.level}
                        StackSize: {item.stackSize}
                        PriceMid: {item.priceMid}
                        PriceLow: {item.priceLow}
                        CanHq: {item.canHq ? `true` : `false`}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default DisplayData;
