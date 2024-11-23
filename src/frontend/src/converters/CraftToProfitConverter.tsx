import {Craft, Crafts, Profit, Profits} from '../types/types';

export function convertCraftToProfit(craft: Craft): Profit {
    return convertMultipleCraftsToProfits([craft])[0];
}

export function convertMultipleCraftsToProfits(crafts: Crafts): Profits {
    return !(crafts?.length > 0)
        ? ([])
        : crafts.map((craft) => {
            const {
                recipeId,
                worldId,
                isHq,
                itemId,
                salePrice,
                craftingCost,
                profit,
                updated,
                itemInfo,
                recipe
            } = craft;

            const resultQuantity = recipe?.resultQuantity ?? 0;
            const iconId = itemInfo?.iconId ?? 0;
            const name = itemInfo?.name ?? '';

            return {
                recipeId,
                worldId,
                isHq,
                itemId,
                salePrice,
                craftingCost,
                profit,
                resultQuantity,
                name,
                iconId,
                updated
            };
        });
}