import React from 'react';
import {Recipe} from '../../../types/types';
import {IDataProps} from "./IDataProps";
import Typography from "@mui/material/Typography/Typography";
import BoxedCardComponent from "../BoxedCardComponent";
import IngredientComponent from "./IngredientComponent";

interface RecipeProps extends IDataProps {
    recipe: Recipe | null | undefined;
}

const RecipeComponent: React.FC<RecipeProps> = ({recipe}) => {
    return <div><BoxedCardComponent children={recipeContent(recipe)}/></div>;
}

const recipeContent = (recipe: Recipe | null | undefined) =>
    <div>
        <BoxedCardComponent children={recipeRoot(recipe)}/>
        <BoxedCardComponent children={ingredientsContent(recipe)}/>
    </div>;

const recipeRoot = (recipe: Recipe | null | undefined) =>
    <Typography className="MuiTypography-root">
        <span>Recipe Id:</span> {recipe?.id || 'Missing'}<br/>
        <span>Craft Type:</span> {recipe?.craftType}<br/>
        <span>Recipe Level Table:</span> {recipe?.recipeLevelTable}<br/>
        <span>Target Item Id:</span> {recipe?.targetItemId}<br/>
        <span>Result Quantity:</span> {recipe?.resultQuantity}<br/>
        <span>Can HQ:</span> {recipe?.canHq ? 'true' : 'false'}<br/>
        <span>Can QuickSynth:</span> {recipe?.canQuickSynth ? 'true' : 'false'}<br/>
    </Typography>

const ingredientsContent = (recipe: Recipe | null | undefined) =>
    <Typography className="MuiTypography-ingredients">
        <IngredientComponent qty={recipe?.amountIngredient0} id={recipe?.itemIngredient0TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient1} id={recipe?.itemIngredient1TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient2} id={recipe?.itemIngredient2TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient3} id={recipe?.itemIngredient3TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient4} id={recipe?.itemIngredient4TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient5} id={recipe?.itemIngredient5TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient6} id={recipe?.itemIngredient6TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient7} id={recipe?.itemIngredient7TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient8} id={recipe?.itemIngredient8TargetId}/>
        <IngredientComponent qty={recipe?.amountIngredient9} id={recipe?.itemIngredient9TargetId}/>
    </Typography>
export default RecipeComponent;
