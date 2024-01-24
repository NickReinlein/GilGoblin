import React from 'react';
import {render, screen} from '@testing-library/react';
import ProfitComponent from './ProfitComponent';

const mockProfit = {
    itemId: 123,
    name: 'Test Item',
    canHq: true,
    worldId: 456,
    recipeId: 789,
    averageListingPrice: 50,
    averageSold: 30,
    recipeCost: 20,
    resultQuantity: 5,
    recipeProfitVsSold: 10,
    recipeProfitVsListings: 25,
    updated: '2024-01-01',
};

describe('ProfitComponent', () => {
    test('renders profit data correctly', () => {
        render(<ProfitComponent profit={mockProfit} index={0}/>);

        expect(screen.getByText(`1`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.name}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.averageListingPrice}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.averageSold}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.recipeCost}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.resultQuantity}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.recipeProfitVsSold}`)).toBeInTheDocument();
        expect(screen.getByText(`${mockProfit.recipeProfitVsListings}`)).toBeInTheDocument();
        expect(screen.getByTestId('age')).toBeInTheDocument();
    });
});
