export interface Item {
    id: number;
    name?: string;
    description?: string;
    iconId?: number;
    level?: number;
    stackSize?: number;
    priceMid?: number;
    priceLow?: number;
    canHq?: boolean;
}

export interface Price {
    worldId: number,
    itemId: number,
    lastUploadTime?: number,
    averageListingPrice: number;
    averageListingPriceNQ?: number;
    averageListingPriceHQ?: number;
    averageSold: number;
    averageSoldNQ?: number;
    averageSoldHQ?: number;
}

export interface Recipe {
    id: number;
    craftType?: number;
    recipeLevelTable?: number;
    targetItemId: number;
    resultQuantity: number;
    canHq?: boolean;
    canQuickSynth?: boolean;
    itemIngredient0TargetId?: number;
    amountIngredient0?: number;
    itemIngredient1TargetId?: number;
    amountIngredient1?: number;
    itemIngredient2TargetId?: number;
    amountIngredient2?: number;
    itemIngredient3TargetId?: number;
    amountIngredient3?: number;
    itemIngredient4TargetId?: number;
    amountIngredient4?: number;
    itemIngredient5TargetId?: number;
    amountIngredient5?: number;
    itemIngredient6TargetId?: number;
    amountIngredient6?: number;
    itemIngredient7TargetId?: number;
    amountIngredient7?: number;
    itemIngredient8TargetId?: number;
    amountIngredient8?: number;
    itemIngredient9TargetId?: number;
    amountIngredient9?: number;
}

export interface Ingredient {
    recipeId: number;
    itemId: number;
    quantity: number;
}

export interface Craft {
    itemId: number;
    worldId: number;
    itemInfo?: Item;
    recipe?: Recipe;
    averageListingPrice: number;
    averageSold: number;
    recipeCost: number;
    recipeProfitVsSold: number;
    recipeProfitVsListings: number;
    ingredients?: Ingredient[];
    updated: string;
}

export interface Profit {
    itemId: number;
    worldId: number;
    recipeId: number;
    profitSold: number;
    profitListings: number;
    cost: number;
    averageListing: number;
    averageSold: number;
    craftType?: number;
    resultQuantity: number;
    name?: string | null;
    iconId?: number;
    canHq?: boolean;
    ingredients?: Ingredient[];
    updated: string;
}

export interface Profits extends Array<Profit>{}
export interface Crafts extends Array<Craft>{}