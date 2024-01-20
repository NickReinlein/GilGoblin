import {Craft, Profit} from "../types/types";
import {convertCraftToProfit} from "./CraftToProfitConverter";

describe('convertCraftToProfit', () => {
    const mockCraft: Craft = {
        itemId: 1,
        worldId: 2,
        averageListingPrice: 10,
        averageSold: 8,
        recipeCost: 5,
        recipeProfitVsSold: 3,
        recipeProfitVsListings: 2,
        updated: '2022-01-01',
        itemInfo: {
            id: 101,
            iconId: 201,
            name: 'CraftedItem',
        },
        recipe: {
            id: 301,
            resultQuantity: 2,
            canHq: true,
            targetItemId: 1
        },
        ingredients: [],
    };

    it('should convert Craft to Profit correctly', () => {
        const result: Profit = convertCraftToProfit(mockCraft);

        expect(result).toEqual({
            itemId: 1,
            worldId: 2,
            recipeId: 301,
            recipeProfitVsSold: 3,
            recipeProfitVsListings: 2,
            recipeCost: 5,
            averageListingPrice: 10,
            averageSold: 8,
            resultQuantity: 2,
            name: 'CraftedItem',
            iconId: 201,
            canHq: true,
            ingredients: [],
            updated: '2022-01-01',
        });
    });
});
