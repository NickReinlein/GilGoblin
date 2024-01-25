import {Craft, Crafts, Profit, Profits} from '../types/types';

export function convertCraftToProfit(craft: Craft): Profit {
    return convertMultipleCraftsToProfits([craft])[0];
}

export function convertMultipleCraftsToProfits(crafts: Crafts): Profits {
    return !(crafts?.length > 0)
        ? ([])
        : crafts.map((craft) => {
            const {
                itemId,
                worldId,
                averageListingPrice,
                averageSold,
                recipeCost,
                recipeProfitVsSold,
                recipeProfitVsListings,
                updated,
                itemInfo,
                recipe,
                ingredients,
            } = craft;

            const recipeId = recipe?.id ?? 0;
            const resultQuantity = recipe?.resultQuantity ?? 0;
            const canHq = recipe?.canHq ?? false;

            const iconId = itemInfo?.iconId ?? 0;
            const name = itemInfo?.name ?? '';

            return {
                itemId,
                worldId,
                recipeId,
                profitSold: recipeProfitVsSold,
                profitListings: recipeProfitVsListings,
                cost: recipeCost,
                averageListing: averageListingPrice,
                averageSold,
                resultQuantity,
                name,
                iconId,
                canHq,
                ingredients,
                updated,
            };
        });
}