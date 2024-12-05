import {Craft, Crafts, Profit, Profits} from '../types/types';

export function convertCraftToProfit(craft: Craft): Profit {
    return convertMultipleCraftsToProfits([craft])[0];
}

export function convertMultipleCraftsToProfits(crafts: Crafts): Profits {
    return crafts.length == 0
        ? []
        : crafts.map((craft) => {
            const {
                recipeId,
                worldId,
                isHq,
                itemId,
                itemInfo,
                recipeInfo,
                salePrice,
                craftingCost,
                profit,
                updated,
            } = craft;

            const resultQuantity = recipeInfo?.resultQuantity ?? 0;
            const name = itemInfo?.name ?? null;
            const iconId = itemInfo?.iconId ?? undefined;

            return {
                recipeId,
                worldId,
                isHq,
                itemId,
                resultQuantity,
                salePrice,
                craftingCost,
                profit,
                name,
                iconId,
                updated
            };
        });
}