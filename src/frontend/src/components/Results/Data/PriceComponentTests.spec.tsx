import React from 'react';
import {render, screen} from '@testing-library/react';
import PriceComponent, {formatDate} from './PriceComponent';
import {Price} from '../../../types/types';

describe('PriceComponent', () => {
    const PriceData: Price =
        {
            "worldId": 34,
            "itemId": 1639,
            "isHq": false,
            "lastUploadTime": 1719292894930
        };

    const expectedLabels =
        {
            "worldId": `World: ${PriceData.worldId}`,
            "itemId": `Item Id: ${PriceData.itemId}`,
            "lastUploadTime": `${formatDate(new Date(PriceData.lastUploadTime ?? 0))}`
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
