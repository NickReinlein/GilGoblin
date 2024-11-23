import {Craft, Profit} from "../types/types";
import {convertCraftToProfit} from "./CraftToProfitConverter";

describe('convertCraftToProfit', () => {
    const mockCraft: Craft = {
        recipeId: 301,
        worldId: 2,
        isHq: true,
        itemId: 1,
        salePrice: 10,
        craftingCost: 8,
        profit: 5,
        updated: '2024-01-01',
        itemInfo: {
            id: 101,
            iconId: 201,
            name: 'CraftedItem',
            canHq: true,
            description: 'Description',
            level: 10,
            stackSize: 1,
            priceMid: 10,
            priceLow: 5
        },
        recipe: {
            id: 301,
            resultQuantity: 2,
            canHq: true,
            targetItemId: 1,
            canQuickSynth: true
        },
    };

    it('should convert Craft to Profit correctly', () => {
        const result: Profit = convertCraftToProfit(mockCraft);

        expect(result).toEqual({
            recipeId: mockCraft.recipeId,
            worldId: mockCraft.worldId,
            isHq: mockCraft.isHq,
            itemId: mockCraft.itemId,
            salePrice: mockCraft.salePrice,
            craftingCost: mockCraft.craftingCost,
            profit: mockCraft.profit,
            resultQuantity: mockCraft.recipe?.resultQuantity,
            name: mockCraft.itemInfo?.name,
            iconId: mockCraft.itemInfo?.iconId,
            updated: mockCraft.updated
        });
    });
});
