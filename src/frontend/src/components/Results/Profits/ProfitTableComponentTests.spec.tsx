import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import {Craft, Crafts} from "../../../types/types";
import ProfitTableComponent from "./ProfitTableComponent";

describe('ProfitTableComponent', () => {
    test('renders a message when crafts are empty', () => {
        render(<ProfitTableComponent crafts={[]}/>);

        expect(screen.getByText("Press the search button to search for the World's best recipes to craft")).toBeInTheDocument();
    });

    test('renders a table with the correct number of columns', () => {
        render(<ProfitTableComponent crafts={mockCrafts}/>);

        const columns = screen.getAllByRole('columnheader');

        expect(columns.length).toBe(9);
    });

    test('maps the first column header to index field', () => {
        const mapped = columnHeaderToFieldMapping('#');

        expect(mapped).toBe('index')
    });

    test('maps the age header to timestamp updated field', () => {
        const mapped = columnHeaderToFieldMapping('Age');

        expect(mapped).toBe('updated')
    });

    test.each(['1', 'Aged', 'Avg. Soul', ''])
    ('maps an invalid header, %p, to missing field',
        (header) => {
            const mapped = columnHeaderToFieldMapping(header);

            expect(mapped).toBe('missing')
        });

    test('renders table rows with the correct data', () => {
        render(<ProfitTableComponent crafts={mockCrafts}/>);

        const rows = screen.getAllByRole('row').slice(1); // remove header

        expect(rows.length).toBe(mockCrafts.length);
        rows.forEach((row, index) => {
            const craft = mockCrafts.find((craft) => row.textContent?.includes(craft.itemInfo?.name || ''));
            expect(craft).toBeTruthy();
            expect(craft?.itemInfo).toBeTruthy();
            expect(row).toHaveTextContent(craft!.itemInfo!.name!.toString());
        });
    });

    test('renders table sorted by sold profit by default', () => {
        render(<ProfitTableComponent crafts={mockCrafts}/>);

        const rows = screen.getAllByRole('row').slice(1); // remove header

        expect(rows[0]).toHaveTextContent(mockCrafts[1].itemInfo!.name!.toString());
        expect(rows[1]).toHaveTextContent(mockCrafts[0].itemInfo!.name!.toString());
        expect(mockCrafts[1].recipeProfitVsSold).toBeGreaterThan(mockCrafts[0].recipeProfitVsSold);
    });

    test('renders table sorted by listing profit when clicked', () => {
        render(<ProfitTableComponent crafts={mockCrafts}/>);

        const listingProfitColumn = screen.getByRole('columnheader', {name: 'Listings Profit'});
        fireEvent.click(listingProfitColumn);

        const rows = screen.getAllByRole('row').slice(1); // remove header

        expect(rows[0]).toHaveTextContent(mockCrafts[1].itemInfo!.name!.toString());
        expect(rows[1]).toHaveTextContent(mockCrafts[0].itemInfo!.name!.toString());
        expect(mockCrafts[1].recipeProfitVsSold).toBeGreaterThan(mockCrafts[0].recipeProfitVsSold);
    });

    const mockCrafts: Crafts = [
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
                "resultQuantity": 11,
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
            "recipeProfitVsListings": 210057,
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
                "resultQuantity": 33,
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
});

const columnHeaderToFieldMapping = (header: string) => {
    switch (header) {
        case '#':
            return 'index';
        case 'Name':
            return 'name'
        case 'Sold Profit':
            return 'profitSold';
        case 'Listings Profit':
            return 'profitListings';
        case 'Avg. Sold':
            return 'averageSold';
        case 'Avg. Listing':
            return 'averageListing';
        case 'Cost':
            return 'cost';
        case 'Qty':
            return 'resultQuantity';
        case 'Age':
            return 'updated';
        default:
            return 'missing';
    }
}