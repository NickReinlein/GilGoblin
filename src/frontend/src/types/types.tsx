export {}

export interface Item {
    id: number;
    name: string;
    description: string;
    iconId: number;
    level: number;
    stackSize: number;
    priceMid: number;
    priceLow: number;
    canHq: boolean;
}

export interface BasePrice {
    id: number;
    name: string;
    description: string;
    iconId: number;
    level: number;
    stackSize: number;
    priceMid: number;
    priceLow: number;
    canHq: boolean;
    averageListingPrice: number;
    averageListingPriceNQ: number;
    averageListingPriceHQ: number;
    averageSold: number;
    averageSoldNQ: number;
    averageSoldHQ: number;
}

export interface Ingredient {
    recipeId: number;
    itemId: number;
    quantity: number;
}

export interface Price {
    worldId: number;
    itemId: number;
    lastUploadTime: number;
}

export interface RecipeValue {
    recipeId: number;
    worldId: number;
    updated: string;
}

export interface craft {
    itemId: number;
    worldId: number;
    itemInfo: Item;
    recipe: Recipe;
    averageListingPrice: number;
    averageSold: number;
    recipeCost: number;
    recipeProfitVsSold: number;
    recipeProfitVsListings: number;
    ingredients: Ingredient[];
    updated: string;
}

export interface Recipe {
    id: number;
    craftType: number;
    recipeLevelTable: number;
    targetItemId: number;
    resultQuantity: number;
    canHq: boolean;
    canQuickSynth: boolean;
    itemIngredient0TargetId: number;
    amountIngredient0: number;
    itemIngredient1TargetId: number;
    amountIngredient1: number;
    itemIngredient2TargetId: number;
    amountIngredient2: number;
    itemIngredient3TargetId: number;
    amountIngredient3: number;
    itemIngredient4TargetId: number;
    amountIngredient4: number;
    itemIngredient5TargetId: number;
    amountIngredient5: number;
    itemIngredient6TargetId: number;
    amountIngredient6: number;
    itemIngredient7TargetId: number;
    amountIngredient7: number;
    itemIngredient8TargetId: number;
    amountIngredient8: number;
    itemIngredient9TargetId: number;
    amountIngredient9: number;
}
