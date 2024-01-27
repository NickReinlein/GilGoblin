import React from 'react';
import {render, screen} from '@testing-library/react';
import RecipeComponent from './RecipeComponent';
import {Recipe} from '../../../types/types';

describe('RecipeComponent', () => {
    const RecipeData: Recipe =
        {
            "id": 3,
            "craftType": 1,
            "recipeLevelTable": 2,
            "targetItemId": 1602,
            "resultQuantity": 1,
            "canHq": true,
            "canQuickSynth": true,
            "itemIngredient0TargetId": 5056,
            "amountIngredient0": 1,
            "itemIngredient1TargetId": 5361,
            "amountIngredient1": 1,
            "itemIngredient2TargetId": 5432,
            "amountIngredient2": 1,
            "itemIngredient3TargetId": 0,
            "amountIngredient3": 0,
            "itemIngredient4TargetId": 0,
            "amountIngredient4": 0,
            "itemIngredient5TargetId": 0,
            "amountIngredient5": 0,
            "itemIngredient6TargetId": 0,
            "amountIngredient6": 0,
            "itemIngredient7TargetId": 0,
            "amountIngredient7": 0,
            "itemIngredient8TargetId": 2,
            "amountIngredient8": 1,
            "itemIngredient9TargetId": 5,
            "amountIngredient9": 1
        };

    describe('Base Fields', () => {
        const baseFieldLabels =
            {
                "id": `Recipe Id: ${RecipeData.id}`,
                "craftType": `Craft Type: ${RecipeData.craftType}`,
                "recipeLevelTable": `Recipe Level Table: ${RecipeData.recipeLevelTable}`,
                "targetItemId": `Target Item Id: ${RecipeData.targetItemId}`,
                "resultQuantity": `Result Quantity: ${RecipeData.resultQuantity}`,
                "canHq": `Can HQ: ${RecipeData.canHq}`,
                "canQuickSynth": `Can QuickSynth: ${RecipeData.canQuickSynth}`,
            };

        Object.entries(baseFieldLabels)
            .forEach(([field, value]) => {
                it(`renders ${field} on screen`, () => {
                    render(<RecipeComponent recipe={RecipeData}/>);

                    const fieldElement = screen.getByText(value);

                    expect(fieldElement).toBeInTheDocument();
                });
            });
    });
    describe('Ingredients', () => {
        Object.entries(RecipeData)
            .filter(([field]) =>
                field.includes("itemIngredient") ||
                field.includes("amountIngredient"))
            .forEach(([field, value]) => {
                it(`renders ingredient ${field} on screen`, () => {
                    render(<RecipeComponent recipe={RecipeData}/>);
                    let elementText = `${field.charAt(0).toUpperCase() + field.slice(1)}: ${value}`;

                    const fieldElement = screen.getByText(elementText);

                    expect(fieldElement).toBeInTheDocument();
                });
            });
    });
});
