import React from 'react';
import {render, screen} from '@testing-library/react';
import PriceComponent from './PriceComponent';
import {Price} from '../../types/types';

describe('PriceComponent', () => {
    const PriceData: Price =
        {
            "averageListingPrice": 10500,
            "averageListingPriceNQ": 0,
            "averageListingPriceHQ": 10500,
            "averageSold": 14470.15,
            "averageSoldNQ": 7356,
            "averageSoldHQ": 21584.3,
            "worldId": 34,
            "itemId": 1639,
            "lastUploadTime": 1698281137868
        };

    const expectedLabels =
        {
            "averageListingPrice": `Average Listing Price: ${PriceData.averageListingPrice}`,
            "averageListingPriceNQ": `Average Listing Price NQ: ${PriceData.averageListingPriceNQ}`,
            "averageListingPriceHQ": `Average Listing Price HQ: ${PriceData.averageListingPriceHQ}`,
            "averageSold": `Average Sold: ${PriceData.averageSold}`,
            "averageSoldNQ": `Average Sold NQ: ${PriceData.averageSoldNQ}`,
            "averageSoldHQ": `Average Sold HQ: ${PriceData.averageSoldHQ}`,
            "worldId": `Price for Item Id: ${PriceData.itemId} in World: ${PriceData.worldId}`,
            "itemId": `Price for Item Id: ${PriceData.itemId} in World: ${PriceData.worldId}`,
            "lastUploadTime": `Last Upload Time: ${PriceData.lastUploadTime}`
        };

    Object.entries(expectedLabels)
        .forEach(([field, value]) => {
            it(`renders ${field} on screen`, () => {
                render(<PriceComponent price={PriceData}/>);

                const fieldElement = screen.getByText(value);

                expect(fieldElement).toBeInTheDocument();
            });
        });
});