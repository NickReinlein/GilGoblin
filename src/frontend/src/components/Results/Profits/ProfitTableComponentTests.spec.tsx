import React from 'react';
import {render, screen} from '@testing-library/react';
import ProfitTableComponent from './ProfitTableComponent';

const mockCrafts = [
    {
        "itemId": 2551,
        "worldId": 34,
        "itemInfo": {
            "id": 2551,
            "name": "Plumed Iron Hatchet",
            "description": "A Plumed Iron Hatchet",
            "iconId": 38105,
            "level": 28,
            "stackSize": 1,
            "priceMid": 2388,
            "priceLow": 37,
            "canHq": true
        },
        "recipe": {
            "id": 100,
            "craftType": 1,
            "recipeLevelTable": 29,
            "targetItemId": 2551,
            "resultQuantity": 684,
            "canHq": true,
            "canQuickSynth": true,
            "itemIngredient0TargetId": 5057,
            "amountIngredient0": 1,
            "itemIngredient1TargetId": 5371,
            "amountIngredient1": 1,
            "itemIngredient2TargetId": 5359,
            "amountIngredient2": 1,
            "itemIngredient3TargetId": 0,
            "amountIngredient3": 0,
            "itemIngredient4TargetId": 0,
            "amountIngredient4": 0,
            "itemIngredient5TargetId": 0,
            "amountIngredient5": 0,
            "itemIngredient6TargetId": 0,
            "amountIngredient6": 0,
            "itemIngredient7TargetId": 0,
            "amountIngredient7": 0,
            "itemIngredient8TargetId": 2,
            "amountIngredient8": 3,
            "itemIngredient9TargetId": 5,
            "amountIngredient9": 2
        },
        "averageListingPrice": 22575,
        "averageSold": 18909,
        "recipeCost": 1518,
        "recipeProfitVsSold": 17391,
        "recipeProfitVsListings": 21057,
        "ingredients": [
            {
                "recipeId": 100,
                "itemId": 5057,
                "quantity": 1
            },
            {
                "recipeId": 100,
                "itemId": 5371,
                "quantity": 1
            },
            {
                "recipeId": 100,
                "itemId": 5359,
                "quantity": 1
            },
            {
                "recipeId": 100,
                "itemId": 222,
                "quantity": 3
            },
            {
                "recipeId": 100,
                "itemId": 567,
                "quantity": 2
            }
        ],
        "updated": "2024-01-02T23:10:32.7760275+00:00"
    },
    {
        "itemId": 7986,
        "worldId": 34,
        "itemInfo": {
            "id": 7986,
            "name": "Riviera Cushion",
            "description": "A plush cushion upholstered in the riviera fashion.",
            "iconId": 52526,
            "level": 55,
            "stackSize": 1,
            "priceMid": 17424,
            "priceLow": 90,
            "canHq": false
        },
        "recipe": {
            "id": 2041,
            "craftType": 5,
            "recipeLevelTable": 70,
            "targetItemId": 7986,
            "resultQuantity": 326,
            "canHq": false,
            "canQuickSynth": false,
            "itemIngredient0TargetId": 8024,
            "amountIngredient0": 3,
            "itemIngredient1TargetId": 5328,
            "amountIngredient1": 2,
            "itemIngredient2TargetId": 5338,
            "amountIngredient2": 3,
            "itemIngredient3TargetId": 8149,
            "amountIngredient3": 3,
            "itemIngredient4TargetId": 0,
            "amountIngredient4": 0,
            "itemIngredient5TargetId": 0,
            "amountIngredient5": 0,
            "itemIngredient6TargetId": 0,
            "amountIngredient6": 0,
            "itemIngredient7TargetId": 0,
            "amountIngredient7": 0,
            "itemIngredient8TargetId": 18,
            "amountIngredient8": 1,
            "itemIngredient9TargetId": 16,
            "amountIngredient9": 1
        },
        "averageListingPrice": 83989,
        "averageSold": 58897,
        "recipeCost": 28405,
        "recipeProfitVsSold": 30492,
        "recipeProfitVsListings": 55584,
        "ingredients": [
            {
                "recipeId": 2041,
                "itemId": 8024,
                "quantity": 3
            },
            {
                "recipeId": 2041,
                "itemId": 5328,
                "quantity": 2
            },
            {
                "recipeId": 2041,
                "itemId": 5338,
                "quantity": 3
            },
            {
                "recipeId": 2041,
                "itemId": 8149,
                "quantity": 3
            },
            {
                "recipeId": 2041,
                "itemId": 18,
                "quantity": 1
            },
            {
                "recipeId": 2041,
                "itemId": 16,
                "quantity": 1
            }
        ],
        "updated": "2024-01-02T23:10:40.4266199+00:00"
    }
];

describe('ProfitTableComponent', () => {
    test('renders a message when crafts are empty', () => {
        render(<ProfitTableComponent crafts={[]}/>);

        expect(screen.getByText("Press the search button to search for the World's best recipes to craft")).toBeInTheDocument();
    });

    test('renders the profit table headers', () => {
        render(<ProfitTableComponent crafts={mockCrafts}/>);

        expect(screen.getByText('#')).toBeInTheDocument();
        expect(screen.getByText('Name')).toBeInTheDocument();
        expect(screen.getByText('Sold Profit')).toBeInTheDocument();
        expect(screen.getByText('Listings Profit')).toBeInTheDocument();
        expect(screen.getByText('Avg. Sold')).toBeInTheDocument();
        expect(screen.getByText('Avg. Listing')).toBeInTheDocument();
        expect(screen.getByText('Cost')).toBeInTheDocument();
        expect(screen.getByText('Qty')).toBeInTheDocument();
        expect(screen.getByText('Age')).toBeInTheDocument();
    });

    test('renders the profit table body with each craft', () => {
        render(<ProfitTableComponent crafts={mockCrafts}/>);

        mockCrafts.forEach((craft, index) => {
            expect(screen.getByText(`${craft.itemInfo.name}`)).toBeInTheDocument();
            expect(screen.getByText(`${craft.averageListingPrice.toLocaleString()}`)).toBeInTheDocument();
            expect(screen.getByText(`${craft.averageSold.toLocaleString()}`)).toBeInTheDocument();
            expect(screen.getByText(`${craft.recipeCost.toLocaleString()}`)).toBeInTheDocument();
            expect(screen.getByText(`${craft.recipeProfitVsSold.toLocaleString()}`)).toBeInTheDocument();
            expect(screen.getByText(`${craft.recipeProfitVsListings.toLocaleString()}`)).toBeInTheDocument();
        });
    });
});
