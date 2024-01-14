import React from 'react';
import {render, screen} from '@testing-library/react';
import CraftComponent from './CraftComponent';
import {Craft} from '../../types/types';

describe('CraftComponent', () => {
    const CraftData: Craft =
        {
            "itemId": 2551,
            "worldId": 34,
            "itemInfo": {
                "id": 2551,
                "name": "Plumed Iron Hatchet",
                "description": "A Plumed Iron Hatchet",
                "iconId": 38105,
                "level": 28,
                "stackSize": 1,
                "priceMid": 2388,
                "priceLow": 37,
                "canHq": true
            },
            "recipe": {
                "id": 100,
                "craftType": 1,
                "recipeLevelTable": 29,
                "targetItemId": 2551,
                "resultQuantity": 1,
                "canHq": true,
                "canQuickSynth": true,
                "itemIngredient0TargetId": 5057,
                "amountIngredient0": 1,
                "itemIngredient1TargetId": 5371,
                "amountIngredient1": 1,
                "itemIngredient2TargetId": 5359,
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
                "amountIngredient8": 3,
                "itemIngredient9TargetId": 5,
                "amountIngredient9": 2
            },
            "averageListingPrice": 22575,
            "averageSold": 18909,
            "recipeCost": 1518,
            "recipeProfitVsSold": 17391,
            "recipeProfitVsListings": 21057,
            "ingredients": [
                {
                    "recipeId": 100,
                    "itemId": 5057,
                    "quantity": 1
                },
                {
                    "recipeId": 100,
                    "itemId": 5371,
                    "quantity": 1
                },
                {
                    "recipeId": 100,
                    "itemId": 5359,
                    "quantity": 1
                },
                {
                    "recipeId": 100,
                    "itemId": 2,
                    "quantity": 3
                },
                {
                    "recipeId": 100,
                    "itemId": 5,
                    "quantity": 2
                }
            ],
            "updated": "0001-01-02T23:10:32.7760275+00:00"
        };

    const expectedLabels =
        {
            "itemId": `Craft Summary for Item ID ${CraftData.itemId} in world ${CraftData.worldId}`,
            "worldId": `Craft Summary for Item ID ${CraftData.itemId} in world ${CraftData.worldId}`,
            "itemInfo": `Item Id: ${CraftData.itemId}`,
            "recipe": `Target Item Id: ${CraftData.recipe.targetItemId}`,
            "averageListingPrice": `Average Listing Price: ${CraftData.averageListingPrice}`,
            "averageSold": `Average Sold: ${CraftData.averageSold}`,
            "recipeCost": `Recipe Cost: ${CraftData.recipeCost}`,
            "recipeProfitVsSold": `Recipe Profit vs Sold: ${CraftData.recipeProfitVsSold}`,
            "recipeProfitVsListings": `Recipe Profit vs Listings: ${CraftData.recipeProfitVsListings}`,
            "Ingredients": `Ingredients`,
            "ingredient1": `Item ID: ${CraftData.ingredients[0].itemId}, Quantity: ${CraftData.ingredients[0].quantity}`,
            "ingredient2": `Item ID: ${CraftData.ingredients[1].itemId}, Quantity: ${CraftData.ingredients[1].quantity}`,
            "ingredient3": `Item ID: ${CraftData.ingredients[2].itemId}, Quantity: ${CraftData.ingredients[2].quantity}`,
            "ingredient4": `Item ID: ${CraftData.ingredients[3].itemId}, Quantity: ${CraftData.ingredients[3].quantity}`,
            "ingredient5": `Item ID: ${CraftData.ingredients[4].itemId}, Quantity: ${CraftData.ingredients[4].quantity}`,
            "updated": `Last Updated: ${CraftData.updated}`,
            "stackSize": `StackSize: ${CraftData.itemInfo.stackSize}`,
            "resultQuantity": `Recipe Result Quantity: ${CraftData?.recipe?.resultQuantity}`,
        };

    Object.entries(expectedLabels)
        .forEach(([field, value]) => {
            it(`renders ${field} on screen`, () => {
                render(<CraftComponent craft={CraftData}/>);

                const fieldElement = screen.getByText(value);

                expect(fieldElement).toBeInTheDocument();
            });
        });
});