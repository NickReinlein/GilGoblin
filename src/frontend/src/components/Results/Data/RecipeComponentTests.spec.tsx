import React from 'react';
import {render, screen} from '@testing-library/react';
import RecipeComponent from './RecipeComponent';
import {Recipe} from '../../../types/types';

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
        };

    const expectedLabels =
        {
            "id": `Recipe Id: ${recipeData.id}`,
            "craftType": `Craft Type: ${recipeData.craftType}`,
            "recipeLevelTable": `Recipe Level Table: ${recipeData.recipeLevelTable}`,
            "targetItemId": `Target Item Id: ${recipeData.targetItemId}`,
            "resultQuantity": `Result Quantity: ${recipeData.resultQuantity}`,
            "canHq": "Can Hq: Yes",
            "canQuickSynth": "Can QuickSynth: Yes"
        };

    Object.entries(expectedLabels)
        .forEach(([field, value]) => {
            it(`renders ${field} on screen`, () => {
                render(<RecipeComponent recipe={recipeData}/>);

                const fieldElement = screen.getByText(value);

                expect(fieldElement).toBeInTheDocument();
            });
        });

    it('renders ingredient components', async () => {
        render(<RecipeComponent recipe={recipeData} />);

        const ingredientComponents = await screen.findAllByText('Ingredients')

        expect(ingredientComponents).toHaveLength(1);
    });
});
