import React from 'react';
import { Price } from '../types/types';

interface PriceProps {
    price: Price;
}

const PriceComponent: React.FC<PriceProps> = ({ price }) => {
    return (
        <div>
            <h2>Price for Item Id: {price.itemId}</h2>
            <p>World Id: {price.worldId}</p>
            <p>Last Upload Time: {price.lastUploadTime}</p>
        </div>
    );
};

export default PriceComponent;

