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

    const {id: recipeId, resultQuantity, canHq} = recipe;
    const {iconId, name} = itemInfo;

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
