import React from 'react';
import { Craft, Ingredient } from '../../types/types';

interface CraftProps {
    craft: Craft;
}

const CraftComponent: React.FC<CraftProps> = ({ craft }) => {
    return (
        <div>
            <h2>Craft Summary for Item ID {craft.itemId}, in world {craft.worldId}</h2>
            <p>Average Listing Price: {craft.averageListingPrice}</p>
            <p>Average Sold: {craft.averageSold}</p>
            <p>Recipe Cost: {craft.recipeCost}</p>
            <p>Recipe Profit vs Sold: {craft.recipeProfitVsSold}</p>
            <p>Recipe Profit vs Listings: {craft.recipeProfitVsListings}</p>

            <h3>Ingredients:</h3>
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
