import React from 'react';
import {render, screen} from '@testing-library/react';
import ProfitComponent from './ProfitComponent';

const mockProfit = {
    itemId: 123,
    name: 'Test Item',
    canHq: true,
    worldId: 456,
    recipeId: 789,
    averageListing: 50,
    averageSold: 30,
    cost: 20,
    resultQuantity: 5,
    profitSold: 10,
    profitListings: 25,
    updated: '2024-01-01',
};

describe('ProfitComponent', () => {
    beforeAll(() => {
        console.error = jest.fn();
        console.log = jest.fn();
    });

    test('renders profit data correctly', () => {
        render(<ProfitComponent profit={mockProfit} index={0}/>);

        expect(screen.getByText(`1`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.name}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.averageListing}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.averageSold}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.cost}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.resultQuantity}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.profitSold}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.profitListings}`)).toBeInTheDocument();
        expect(screen.getByTestId('age')).toBeInTheDocument();
    });
});
