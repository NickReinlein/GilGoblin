import {Craft, Profit} from "../types/types";
import {convertCraftToProfit, convertMultipleCraftsToProfits} from "./CraftToProfitConverter";

describe('convertCraftToProfit', () => {
    const mockCraft: Craft = {
        recipeId: 301,
        worldId: 2,
        isHq: true,
        itemId: 101,
        itemInfo: {
            id: 101,
            iconId: 201,
            name: 'CraftedItem',
        },
        recipeInfo: {
            id: 301,
            resultQuantity: 2,
            canHq: true,
            targetItemId: 101,
            canQuickSynth: true
        },
        salePrice: 10,
        craftingCost: 3,
        profit: 7,
        updated: '2024-11-25T22:03:33.463499+00:00'
    };

    it('should convert Craft to Profit correctly', () => {
        const result: Profit = convertCraftToProfit(mockCraft);

        expect(result).toEqual({
            recipeId: 301,
            worldId: 2,
            itemId: 101,
            isHq: true,
            salePrice: 10,
            craftingCost: 3,
            profit: 7,
            resultQuantity: 2,
            name: 'CraftedItem',
            iconId: 201,
            updated: '2024-11-25T22:03:33.463499+00:00'
        });
    });

    it('should handle an empty Crafts gracefully', () => {
        const result = convertMultipleCraftsToProfits([]);

        expect(result).toEqual([]);
    });
});
