import React from 'react';
import { CraftSummary, Ingredient } from '../types/types';

interface CraftProps {
    craftSummary: CraftSummary;
}

const CraftComponent: React.FC<CraftProps> = ({ craftSummary }) => {
    return (
        <div>
            <h2>Craft Summary for Item ID: {craftSummary.itemId}</h2>
            <p>World ID: {craftSummary.worldId}</p>
            <p>Average Listing Price: {craftSummary.averageListingPrice}</p>
            <p>Average Sold: {craftSummary.averageSold}</p>
            <p>Recipe Cost: {craftSummary.recipeCost}</p>
            <p>Recipe Profit vs Sold: {craftSummary.recipeProfitVsSold}</p>
            <p>Recipe Profit vs Listings: {craftSummary.recipeProfitVsListings}</p>

            <h3>Ingredients:</h3>
            <ul>
                {craftSummary.ingredients?.map((ingredient: Ingredient, index: number) => (
                    <li key={index}>
                        Item ID: {ingredient.itemId}, Quantity: {ingredient.quantity}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default CraftComponent;
