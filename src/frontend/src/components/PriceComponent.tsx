import React from 'react';
import { Price } from '../types/types';

interface PriceProps {
    price: Price;
}

const PriceComponent: React.FC<PriceProps> = ({ price }) => {
    return (
        <div>
            <h2>Item ID: {price.itemId}</h2>
            <p>World ID: {price.worldId}</p>
            <p>Last Upload Time: {price.lastUploadTime}</p>
        </div>
    );
};

export default PriceComponent;

