import React from 'react';
import {Recipe} from '../../../types/types';

interface RecipeProps {
    recipe: Recipe | null | undefined;
}

const RecipeComponent: React.FC<RecipeProps> = ({recipe}) => {
    return (!recipe)
        ? <div>Missing Recipe</div>
        : (
            <div>
                <p>Recipe Id: {recipe?.id || 'Missing'}</p>
                <p>Craft Type: {recipe?.craftType}</p>
                <p>Recipe Level Table: {recipe?.recipeLevelTable}</p>
                <p>Target Item Id: {recipe?.targetItemId}</p>
                <p>Result Quantity: {recipe?.resultQuantity}</p>
                <p>Can HQ: {recipe?.canHq ? 'true' : 'false'}</p>
                <p>Can QuickSynth: {recipe?.canQuickSynth ? 'true' : 'false'}</p>
                <p>ItemIngredient0TargetId: {recipe?.itemIngredient0TargetId}</p>
                <p>AmountIngredient0: {recipe?.amountIngredient0}</p>
                <p>ItemIngredient1TargetId: {recipe?.itemIngredient1TargetId}</p>
                <p>AmountIngredient1: {recipe?.amountIngredient1}</p>
                <p>ItemIngredient2TargetId: {recipe?.itemIngredient2TargetId}</p>
                <p>AmountIngredient2: {recipe?.amountIngredient2}</p>
                <p>ItemIngredient3TargetId: {recipe?.itemIngredient3TargetId}</p>
                <p>AmountIngredient3: {recipe?.amountIngredient3}</p>
                <p>ItemIngredient4TargetId: {recipe?.itemIngredient4TargetId}</p>
                <p>AmountIngredient4: {recipe?.amountIngredient4}</p>
                <p>ItemIngredient5TargetId: {recipe?.itemIngredient5TargetId}</p>
                <p>AmountIngredient5: {recipe?.amountIngredient5}</p>
                <p>ItemIngredient6TargetId: {recipe?.itemIngredient6TargetId}</p>
                <p>AmountIngredient6: {recipe?.amountIngredient6}</p>
                <p>ItemIngredient7TargetId: {recipe?.itemIngredient7TargetId}</p>
                <p>AmountIngredient7: {recipe?.amountIngredient7}</p>
                <p>ItemIngredient8TargetId: {recipe?.itemIngredient8TargetId}</p>
                <p>AmountIngredient8: {recipe?.amountIngredient8}</p>
                <p>ItemIngredient9TargetId: {recipe?.itemIngredient9TargetId}</p>
                <p>AmountIngredient9: {recipe?.amountIngredient9}</p>
            </div>
        );
};

export default RecipeComponent;
