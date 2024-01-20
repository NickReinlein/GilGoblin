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
    updated: '2022-01-01',
};

describe('ProfitComponent', () => {
    test('renders profit data correctly', () => {
        render(<ProfitComponent profit={mockProfit} index={0}/>);

        expect(screen.getByText(`1`)).toBeInTheDocument();
        expect(screen.getByText(`Item Id: ${mockProfit.itemId}`)).toBeInTheDocument();
        expect(screen.getByText(`Name: ${mockProfit.name}`)).toBeInTheDocument();
        expect(screen.getByText(`CanHq: ${mockProfit.canHq}`)).toBeInTheDocument();
        expect(screen.getByText(`World Id: ${mockProfit.worldId}`)).toBeInTheDocument();
        expect(screen.getByText(`Recipe Id: ${mockProfit.recipeId}`)).toBeInTheDocument();
        expect(screen.getByText(`Average Listing Price: ${mockProfit.averageListingPrice}`)).toBeInTheDocument();
        expect(screen.getByText(`Average Sold: ${mockProfit.averageSold}`)).toBeInTheDocument();
        expect(screen.getByText(`Recipe Cost: ${mockProfit.recipeCost}`)).toBeInTheDocument();
        expect(screen.getByText(`Recipe Result Quantity: ${mockProfit.resultQuantity}`)).toBeInTheDocument();
        expect(screen.getByText(`Recipe Profit vs Sold: ${mockProfit.recipeProfitVsSold}`)).toBeInTheDocument();
        expect(screen.getByText(`Recipe Profit vs Listings: ${mockProfit.recipeProfitVsListings}`)).toBeInTheDocument();
        expect(screen.getByText(`Last Updated: ${mockProfit.updated}`)).toBeInTheDocument();
    });
});
