export interface World {
    id: number;
    name?: string;
}

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
    id: number;
    itemId: number;
    worldId: number;
    isHq: boolean;
    updated: string;
    minListing?: PriceDataPointPoco | null;
    recentPurchase?: PriceDataPointPoco | null;
    averageSalePrice?: PriceDataPointPoco | null;
    dailySaleVelocity?: DailySaleVelocityPoco | null;
}

export interface PriceDataPointPoco {
    id: number;
    itemId: number;
    worldId: number;
    isHq: boolean;
    worldDataPoint?: PriceDataPoco | null;
    dcDataPoint?: PriceDataPoco | null;
    regionDataPoint?: PriceDataPoco | null;
}

export interface PriceDataPoco {
    id: number;
    priceType: string;
    price: number;
    worldId: number;
    timestamp: number;
}

export interface DailySaleVelocityPoco {
    id: number;
    itemId: number;
    worldId: number;
    isHq: boolean;
    world: number;
    dc: number;
    region: number;
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
    recipeId: number;
    worldId: number;
    isHq: boolean;
    itemId: number;
    itemInfo?: Item;
    recipeInfo?: Recipe;
    salePrice: number;
    craftingCost: number;
    profit: number;
    updated: string;
    [key: string]: any;
}

export interface Profit {
    recipeId: number;
    worldId: number;
    isHq: boolean;
    itemId: number;
    salePrice: number;
    craftingCost: number;
    profit: number;
    resultQuantity: number;
    name?: string | null;
    iconId?: number;
    updated: string;
    [key: string]: any;
}

export interface Profits extends Array<Profit> {
}

export interface Crafts extends Array<Craft> {
}