import React from 'react';
import {Price} from '../../types/types';

interface PriceProps {
    price: Price;
}

const PriceComponent: React.FC<PriceProps> = ({price}) => {
    return (
        <div>
            <h2>Price for Item Id: {price?.itemId} in World: {price?.worldId}</h2>
            <p>Last Upload Time: {price?.lastUploadTime}</p>
            <p>Average Listing Price: {price?.averageListingPrice}</p>
            <p>Average Listing Price NQ: {price?.averageListingPriceNQ}</p>
            <p>Average Listing Price HQ: {price?.averageListingPriceHQ}</p>
            <p>Average Sold: {price?.averageSold}</p>
            <p>Average Sold NQ: {price?.averageSoldNQ}</p>
            <p>Average Sold HQ: {price?.averageSoldHQ}</p>
        </div>
    );
};

export default PriceComponent;

