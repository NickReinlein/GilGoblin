import React from 'react';
import '../../styles/CraftComponent.css';
import {Craft, Ingredient} from '../../types/types';
import RecipesComponent from './RecipeComponent';
import ItemComponent from './ItemComponent';
import TimestampToAgeConverter from "../../converters/TimestampToAgeConverter";

interface CraftProps {
    craft: Craft;
}

const CraftComponent: React.FC<CraftProps> = ({craft}) => {
    return (
        <div className="craft">
            <p>Craft Summary for Item ID {craft?.itemId} in world {craft?.worldId}</p>
            <table>
                <tbody>
                <tr>
                    <td>Average Listing Price:</td>
                    <td>{craft?.averageListingPrice?.toLocaleString()}</td>
                </tr>
                <tr>
                    <td>Average Sold:</td>
                    <td>{craft?.averageSold?.toLocaleString()}</td>
                </tr>
                <tr>
                    <td>Recipe Cost:</td>
                    <td>{craft?.recipeCost?.toLocaleString()}</td>
                </tr>
                <tr>
                    <td>Recipe Result Quantity:</td>
                    <td>{craft?.recipe?.resultQuantity?.toLocaleString()}</td>
                </tr>
                <tr>
                    <td>Recipe Profit vs Sold:</td>
                    <td>{craft?.recipeProfitVsSold?.toLocaleString()}</td>
                </tr>
                <tr>
                    <td>Recipe Profit vs Listings:</td>
                    <td>{craft?.recipeProfitVsListings?.toLocaleString()}</td>
                </tr>
                <tr>
                    <td>Age:</td>
                    <td><TimestampToAgeConverter timestamp={craft?.updated}/></td>
                </tr>
                </tbody>
            </table>
            <h3>
                <u>Ingredients</u>
            </h3>
            <div className="ingredients">
                <table>
                    <thead>
                        <tr>
                            <th>Qty</th>
                            <th>Item Id</th>
                        </tr>
                    </thead>
                    <tbody>
                    {craft?.ingredients?.map((ingredient: Ingredient, index: number) => (
                        <tr key={index}>
                            <td>{ingredient?.quantity}</td>
                            <td>{ingredient?.itemId}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
            <h3>
                <u>Recipe</u>
            </h3>
            <div className="recipes">
                <RecipesComponent recipe={craft?.recipe}/>
            </div>
            <h3>
                <u>Item</u>
            </h3>
            <div className="iteminfo">
                <ItemComponent item={craft?.itemInfo}/>
            </div>
        </div>
    );
};

export default CraftComponent;
