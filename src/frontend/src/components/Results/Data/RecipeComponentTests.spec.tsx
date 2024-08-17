import React from 'react';
import {render, screen} from '@testing-library/react';
import RecipeComponent from './RecipeComponent';
import {Recipe} from '../../../types/types';
import ItemComponent from "./ItemComponent";

describe('RecipeComponent', () => {
    const recipeData: Recipe =
        {
            id: 3,
            craftType: 1,
            recipeLevelTable: 2,
            targetItemId: 1602,
            resultQuantity: 1,
            canHq: true,
            canQuickSynth: true,
            itemIngredient0TargetId: 111,
            amountIngredient0: 11,
            itemIngredient1TargetId: 222,
            amountIngredient1: 22,
            itemIngredient2TargetId: 0,
            amountIngredient2: 0,
            itemIngredient3TargetId: 0,
            amountIngredient3: 0,
            itemIngredient4TargetId: 0,
            amountIngredient4: 0,
            itemIngredient5TargetId: 0,
            amountIngredient5: 0,
            itemIngredient6TargetId: 0,
            amountIngredient6: 0,
            itemIngredient7TargetId: 0,
            amountIngredient7: 0,
            itemIngredient8TargetId: 0,
            amountIngredient8: 0,
            itemIngredient9TargetId: 333,
            amountIngredient9: 33
        };
    const expectedLabels =
        {
            "id": `Recipe Id: ${recipeData.id}`,
            "craftType": `Craft Type: ${recipeData.craftType}`,
            "recipeLevelTable" : `Recipe Level Table: ${recipeData.recipeLevelTable}`,
            "targetItemId" : `Target Item Id: ${recipeData.targetItemId}`,
            "resultQuantity" : `Result Quantity: ${recipeData.resultQuantity}`,
            "canHq" : "Can Hq: Yes",
            "canQuickSynth" : "Can QuickSynth: Yes"
        };

    Object.entries(expectedLabels)
        .forEach(([field, value]) => {
            it(`renders ${field} on screen`, () => {
                render(<RecipeComponent recipe={recipeData}/>);

                const fieldElement = screen.getByText(value);

                expect(fieldElement).toBeInTheDocument();
            });
        });
});
