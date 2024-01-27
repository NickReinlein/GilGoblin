import React from 'react';
import {render, screen} from '@testing-library/react';
import ItemComponent from './ItemComponent';
import {Item} from '../../../types/types';

describe('ItemComponent', () => {
    const itemData: Item =
        {
            id: 2,
            name: 'test',
            description: 'testDescription',
            iconId: 22,
            level: 33,
            stackSize: 1,
            priceMid: 101,
            priceLow: 223,
            canHq: true
        };
    Object.entries(itemData)
        .forEach(([field, value]) => {
            it(`renders ${field} on screen`, () => {
                render(<ItemComponent item={itemData}/>);
                let elementText = field === 'id'
                    ? `Item Id: ${itemData.id}`
                    : `${field.charAt(0).toUpperCase() + field.slice(1)}: ${value}`;

                const fieldElement = screen.getByText(elementText);

                expect(fieldElement).toBeInTheDocument();
            });
        });
});
