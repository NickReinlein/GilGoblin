import {Craft, Profit} from '../types/types';

export function convertCraftToProfit(craft: Craft): Profit {
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
        recipeProfitVsSold,
        recipeProfitVsListings,
        recipeCost,
        averageListingPrice,
        averageSold,
        resultQuantity,
        name,
        iconId,
        canHq,
        ingredients,
        updated,
    };
}
