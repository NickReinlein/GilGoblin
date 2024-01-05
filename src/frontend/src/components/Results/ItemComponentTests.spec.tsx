import React from 'react';
import {render, screen} from '@testing-library/react';
import ItemComponent from './ItemComponent';
import {Item} from '../../types/types';

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
            canHq: true,
        };

    it('renders correctly with ItemSummary', () => {
        render(<ItemComponent item={itemData} />);
        const itemIdElement = screen.getByText(`Item Id: ${itemData.id}`);
        expect(itemIdElement).toBeInTheDocument();
    });
});