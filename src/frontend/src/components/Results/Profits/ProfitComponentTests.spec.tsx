import React from 'react';
import {render, screen} from '@testing-library/react';
import ProfitComponent from './ProfitComponent';

const mockProfit = {
    recipeId: 301,
    worldId: 231,
    isHq: true,
    itemId: 101,
    salePrice: 40,
    craftingCost: 31,
    profit: 9,
    resultQuantity: 2,
    name: 'CraftedItem',
    iconId: 3341,
    updated: '2024-11-25T22:03:33.463499+00:00'
};

describe('ProfitComponent', () => {
    beforeAll(() => {
        console.error = jest.fn();
        console.log = jest.fn();
    });

    test('renders profit data correctly', () => {
        render(<ProfitComponent profit={mockProfit} index={0}/>);

        expect(screen.getByText(`${mockProfit.recipeId}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.worldId}`)).toBeInTheDocument();
        // expect(screen.getByText(`${mockProfit.isHq}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.itemId}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.salePrice}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.craftingCost}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.profit}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.resultQuantity}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.name}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.iconId}`)).toBeInTheDocument();
        // expect(screen.getByText(`${mockProfit.updated}`)).toBeInTheDocument();
    });
});
