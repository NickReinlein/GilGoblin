import React from 'react';
import {render, screen} from '@testing-library/react';
import {Crafts} from "../../../types/types";
import ProfitTableComponent, {timeAgo} from "./ProfitTableComponent";

describe('ProfitTableComponent', () => {
    test('renders a message when crafts are empty', () => {
        render(<ProfitTableComponent crafts={[]}/>);

        expect(screen.getByText("Press the search button to search for the World's best recipes to craft")).toBeInTheDocument();
    });

    test('renders a table with the correct number of columns', () => {
        render(<ProfitTableComponent crafts={mockCrafts}/>);

        const columns = screen.getAllByRole('columnheader');

        expect(columns.length).toBe(11);
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
        rows.forEach((row) => {
            const craft = mockCrafts.find((craft) => row.textContent?.includes(craft.itemInfo?.name || ''));
            expect(craft).toBeTruthy();
            expect(craft?.itemInfo).toBeTruthy();
            expect(row).toHaveTextContent(craft!.itemInfo!.name!.toString());
        });
    });

    const mockCrafts: Crafts = [
        {
            "recipeId": 100,
            "worldId": 34,
            "isHq": true,
            "itemId": 2551,
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
            "recipeInfo": {
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
            "salePrice": 10,
            "craftingCost": 3,
            "profit": 7,
            "name": "Plumed Iron Hatchet",
            "iconId": 38105,
            "updated": "2024-01-02T23:10:32.7760275+00:00"
        },
        {
            "recipeId": 2041,
            "worldId": 34,
            "isHq": false,
            "itemId": 7986,
            "itemInfo": {
                "id": 7986,
                "name": "Riviera Cushion",
                "description": "A plush cushion upholstered in the riviera fashion.",
                "iconId": 52526,
                "level": 55,
                "stackSize": 1,
                "priceMid": 17424,
                "priceLow": 90,
                "canHq": true
            },
            "recipeInfo": {
                "id": 2041,
                "craftType": 5,
                "recipeLevelTable": 70,
                "targetItemId": 7986,
                "resultQuantity": 33,
                "canHq": true,
                "canQuickSynth": true,
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
            "salePrice": 10,
            "craftingCost": 3,
            "profit": 7,
            "name": "Riviera Cushion",
            "iconId": 38657,
            "updated": "2024-01-02T23:10:40.4266199+00:00"
        }
    ];
});

describe('timeAgo function', () => {
    test.each([
        [30, '30 seconds ago'],
        [90, '1 minutes ago'],
        [5400, '1 hours ago'],
        [86400, '1 days ago'],
        [172800, '2 days ago'],
        [259200, '3 days ago'],
    ])('converts %d seconds into "%s"', (input, expected) => {
        expect(timeAgo(input)).toBe(expected);
    });
});

const columnHeaderToFieldMapping = (header: string) => {
    switch (header) {
        case 'Recipe Id':
            return 'recipeId';
        case 'World Id':
            return 'worldId';
        case 'Is HQ':
            return 'isHq';
        case 'Item Id':
            return 'itemId';
        case 'Name':
            return 'name'
        case 'Sale Price':
            return 'salePrice'
        case 'Crafting Cost':
            return 'craftingCost';
        case 'Profit':
            return 'profit'
        case 'Qty':
            return 'resultQuantity';
        case 'Icon Id':
            return 'iconId'
        case 'Age':
            return 'updated';
        default:
            return 'missing';
    }
}