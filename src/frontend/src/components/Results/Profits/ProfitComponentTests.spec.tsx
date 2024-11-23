import React from 'react';
import {render, screen} from '@testing-library/react';
import ProfitComponent from './ProfitComponent';
import {Profit} from "../../../types/types";

const mockProfit : Profit = {
    recipeId: 789,
    worldId: 456,
    isHq: true,
    itemId: 123,
    salePrice: 50,
    craftingCost: 30,
    profit: 20,
    craftType: 1,
    resultQuantity: 2,
    name: 'Test Item',
    iconId: 1,
    updated: '2024-01-01'
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
        expect(screen.getByText(`${mockProfit.isHq}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.itemId}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.name}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.salePrice}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.craftingCost}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.profit}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.resultQuantity}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.updated}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.craftType}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.iconId}`)).toBeInTheDocument();
    });
});
