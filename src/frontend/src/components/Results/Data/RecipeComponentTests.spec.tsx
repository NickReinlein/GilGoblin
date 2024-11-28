import React from 'react';
import {render, screen} from '@testing-library/react';
import RecipeComponent from './RecipeComponent';
import {Recipe} from '../../../types/types';

describe('RecipeComponent', () => {
    const recipe: Recipe = {
        id: 3,
        craftType: 1,
        recipeLevelTable: 2,
        targetItemId: 1602,
        resultQuantity: 1,
        canHq: true,
        canQuickSynth: true,
        amountIngredient0: 5,
        itemIngredient0TargetId: 101,
        amountIngredient1: 3,
        itemIngredient1TargetId: 102,
        amountIngredient2: undefined,
        itemIngredient2TargetId: undefined
    };

    const expectedLabels = {
        id: `Recipe Id: ${recipe.id}`,
        craftType: `Craft Type: ${recipe.craftType}`,
        targetItemId: `Target Item Id: ${recipe.targetItemId}`,
        resultQuantity: `Result Quantity: ${recipe.resultQuantity}`,
        canHq: `Can Hq: ${recipe?.canHq ? 'Yes' : 'No'}`,
        canQuickSynth: `Can QuickSynth: ${recipe.canQuickSynth ? 'Yes' : 'No'}`
    };

    Object.entries(expectedLabels).forEach(([field, value]) => {
        it(`renders ${field} on screen`, async () => {
            render(<RecipeComponent recipe={recipe}/>);

            const fieldElement = await screen.findByText(value);

            expect(fieldElement).toBeInTheDocument();
        });
    });

    it('renders the "Ingredients" section', () => {
        render(<RecipeComponent recipe={recipe}/>);

        const ingredientsTitle = screen.getByText(/Ingredients/i);
        expect(ingredientsTitle).toBeInTheDocument();

        const quantityItemText = screen.getByText(/5 x 101/i);
        expect(quantityItemText).toBeInTheDocument();

        const secondIngredientText = screen.getByText(/3 x 102/i);
        expect(secondIngredientText).toBeInTheDocument();

        // Invalid ingredients should not appear
        const missingIngredientText = screen.queryByText(/null/i);
        expect(missingIngredientText).not.toBeInTheDocument();
    });

    it('renders "Missing" for null or undefined recipe', () => {
        render(<RecipeComponent recipe={null}/>);

        const missingText = screen.getByText(/Recipe Id: Missing/i);
        expect(missingText).toBeInTheDocument();
    });
});