import React from 'react';
import { Craft, Ingredient } from '../../types/types';
import RecipesComponent from './RecipeComponent';
import ItemComponent from './ItemComponent';

interface CraftProps {
    craft: Craft;
}

const CraftComponent: React.FC<CraftProps> = ({ craft }) => {
    return (
        <div>
            <h2>Craft Summary for Item ID {craft?.itemId} in world {craft?.worldId}</h2>
            <table>
                <tbody>
                <tr>
                    <td>Average Listing Price:</td>
                    <td>{craft?.averageListingPrice}</td>
                </tr>
                <tr>
                    <td>Average Sold:</td>
                    <td>{craft?.averageSold}</td>
                </tr>
                <tr>
                    <td>Recipe Cost:</td>
                    <td>{craft?.recipeCost}</td>
                </tr>
                <tr>
                    <td>Recipe Result Quantity:</td>
                    <td>{craft?.recipe?.resultQuantity}</td>
                </tr>
                <tr>
                    <td>Recipe Profit vs Sold:</td>
                    <td>{craft?.recipeProfitVsSold}</td>
                </tr>
                <tr>
                    <td>Recipe Profit vs Listings:</td>
                    <td>{craft?.recipeProfitVsListings}</td>
                </tr>
                <tr>
                    <td>Last Updated:</td>
                    <td>{craft?.updated}</td>
                </tr>
                </tbody>
            </table>
            <h3>
                <u>Ingredients</u>
            </h3>
            <table>
                <tbody>
                {craft?.ingredients?.map((ingredient: Ingredient, index: number) => (
                    <tr key={index}>
                        <td>Item ID:</td>
                        <td>{ingredient?.itemId}</td>
                        <td>Quantity:</td>
                        <td>{ingredient?.quantity}</td>
                    </tr>
                ))}
                </tbody>
            </table>
            <RecipesComponent recipe={craft?.recipe} />
            <ItemComponent item={craft?.itemInfo} />
        </div>
    );
};

export default CraftComponent;
