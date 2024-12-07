import {Craft, Crafts, Profit, Profits} from '../types/types';

export function convertCraftToProfit(craft: Craft): Profit {
    return convertMultipleCraftsToProfits([craft])[0];
}

export function convertMultipleCraftsToProfits(crafts: Crafts): Profits {
    return crafts.length === 0
        ? []
        : crafts.map((craft) => {
            const {
                recipeId,
                worldId,
                isHq,
                itemId,
                itemInfo,
                recipe,
                salePrice,
                craftingCost,
                profit,
                updated,
            } = craft;

            const resultQuantity = recipe?.resultQuantity ?? 0;
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