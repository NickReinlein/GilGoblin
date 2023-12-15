import React from 'react';
import { Recipe } from '../../types/types';

interface RecipeProps {
    recipe: Recipe;
}

const RecipeComponent: React.FC<RecipeProps> = ({ recipe }) => {
    return (
        <div>
            <h2>Recipe Id: {recipe.id}</h2>
            <p>Craft Type: {recipe.craftType}</p>
            <p>Recipe Level Table: {recipe.recipeLevelTable}</p>
            <p>Target Item Id: {recipe.targetItemId}</p>
            <p>Result Quantity: {recipe.resultQuantity}</p>
            <p>Can HQ: {recipe.canHq ? 'true' : 'false'}</p>
        </div>
    );
};

export default RecipeComponent;
