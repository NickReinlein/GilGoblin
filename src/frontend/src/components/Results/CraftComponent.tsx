import React from 'react';
import {Craft, Ingredient} from '../../types/types';
import RecipesComponent from "./RecipeComponent";
import ItemComponent from "./ItemComponent";

interface CraftProps {
    craft: Craft;
}

const CraftComponent: React.FC<CraftProps> = ({craft}) => {
    return (
        <div>
            <h2>Craft Summary for Item ID {craft.itemId}, in world {craft.worldId}</h2>
            <p>Average Listing Price: {craft.averageListingPrice}</p>
            <p>Average Sold: {craft.averageSold}</p>
            <p>Recipe Cost: {craft.recipeCost}</p>
            <p>Recipe Profit vs Sold: {craft.recipeProfitVsSold}</p>
            <p>Recipe Profit vs Listings: {craft.recipeProfitVsListings}</p>
            <p>Last Updated: {craft.updated}</p>
            <h3>Recipe</h3>
            <RecipesComponent recipe={craft.recipe}/>
            {/*<ItemComponent item={craft.itemInfo}/>*/}

            <h3>Ingredients</h3>
            <ul>
                {craft.ingredients?.map((ingredient: Ingredient, index: number) => (
                    <li key={index}>
                        Item ID: {ingredient.itemId}, Quantity: {ingredient.quantity}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default CraftComponent;
