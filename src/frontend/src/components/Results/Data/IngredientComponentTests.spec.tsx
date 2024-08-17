import React from 'react';
import {render, screen} from '@testing-library/react';
import {Ingredient} from '../../../types/types';
import IngredientComponent from "./IngredientComponent";

describe('IngredientComponent', () => {
    const ingredientData: Ingredient[] = [
        {
            "recipeId": 1,
            "itemId": 1601,
            "quantity": 10
        },
        {
            "recipeId": 2,
            "itemId": 1499,
            "quantity": 22
        }
    ];

    Object.entries(ingredientData)
        .forEach((ingredient, index) => {
            it(`renders ingredient ${index} on screen`, () => {
                render(<IngredientComponent qty={ingredient[1].quantity} id={ingredient[1].itemId}/>);

                const fieldElement = screen.getByText(`${ingredient[1].quantity} x ${ingredient[1].itemId}`);

                expect(fieldElement).toBeInTheDocument();
            });
        });
});
