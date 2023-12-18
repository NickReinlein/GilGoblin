import React from 'react';
import {render} from '@testing-library/react';
import CraftComponent from '../../src/components/Results/CraftComponent';
import {craft} from "../../src/types/types";

describe('CraftComponent', () => {
    const mockCraftSummary = {
        itemId: 2,
        worldId: 99,
        averageListingPrice: 200,
        averageSold: 75,
        recipeCost: 40,
        recipeProfitVsSold: 35,
        recipeProfitVsListings: 25,
        ingredients: [
            {itemId: 789, quantity: 3},
            {itemId: 987, quantity: 8},
        ],
        itemInfo:
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
            }
    };

    it('renders correctly with craftSummary', () => {
        const {getByText} = render(<CraftComponent craft={mockCraftSummary}/>);
        expect(getByText('Craft Summary for Item ID 2, in world 99')).toBeInTheDocument();
        expect(getByText('Average Listing Price: 200')).toBeInTheDocument();
        expect(getByText('Average Sold: 75')).toBeInTheDocument();
    });

    it('renders ingredient details correctly', () => {
        const {getByText} = render(<CraftComponent craft={mockCraftSummary}/>);
        expect(getByText('Item ID: 789, Quantity: 3')).toBeInTheDocument();
        expect(getByText('Item ID: 987, Quantity: 8')).toBeInTheDocument();
    });
});