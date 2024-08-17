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

    const expectedLabels =
        {
            "id": `Id ${itemData.id}`,
            "name": `${itemData.name}`,
            "description": `${itemData.description}`,
            "iconId": `Icon Id: ${itemData.iconId}`,
            "level": `Level: ${itemData.level}`,
            "StackSize": `Stack size: ${itemData.stackSize}`,
            "priceMid": `PriceMid: ${itemData.priceMid}`,
            "priceLow": `PriceLow: ${itemData.priceLow}`,
            "canHq": `CanHq: Yes`
        }

    Object.entries(expectedLabels)
        .forEach(([field, value]) => {
            it(`renders ${field} on screen`, () => {
                render(<ItemComponent item={itemData}/>);

                const fieldElement = screen.getByText(value);

                expect(fieldElement).toBeInTheDocument();
            });
        });
});
