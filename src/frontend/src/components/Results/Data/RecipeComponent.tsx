import React from 'react';
import {Recipe} from '../../../types/types';
import {IDataProps} from "./IDataProps";
import Typography from "@mui/material/Typography/Typography";
import BoxedCardComponent from "../BoxedCardComponent";

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
        <span>ItemIngredient0TargetId:</span> {recipe?.itemIngredient0TargetId}<br/>
        <span>AmountIngredient0:</span> {recipe?.amountIngredient0}<br/>
        <span>ItemIngredient1TargetId:</span> {recipe?.itemIngredient1TargetId}<br/>
        <span>AmountIngredient1:</span> {recipe?.amountIngredient1}<br/>
        <span>ItemIngredient2TargetId:</span> {recipe?.itemIngredient2TargetId}<br/>
        <span>AmountIngredient2:</span> {recipe?.amountIngredient2}<br/>
        <span>ItemIngredient3TargetId:</span> {recipe?.itemIngredient3TargetId}<br/>
        <span>AmountIngredient3:</span> {recipe?.amountIngredient3}<br/>
        <span>ItemIngredient4TargetId:</span> {recipe?.itemIngredient4TargetId}<br/>
        <span>AmountIngredient4:</span> {recipe?.amountIngredient4}<br/>
        <span>ItemIngredient5TargetId:</span> {recipe?.itemIngredient5TargetId}<br/>
        <span>AmountIngredient5:</span> {recipe?.amountIngredient5}<br/>
        <span>ItemIngredient6TargetId:</span> {recipe?.itemIngredient6TargetId}<br/>
        <span>AmountIngredient6:</span> {recipe?.amountIngredient6}<br/>
        <span>ItemIngredient7TargetId:</span> {recipe?.itemIngredient7TargetId}<br/>
        <span>AmountIngredient7:</span> {recipe?.amountIngredient7}<br/>
        <span>ItemIngredient8TargetId:</span> {recipe?.itemIngredient8TargetId}<br/>
        <span>AmountIngredient8:</span> {recipe?.amountIngredient8}<br/>
        <span>ItemIngredient9TargetId:</span> {recipe?.itemIngredient9TargetId}<br/>
        <span>AmountIngredient9:</span> {recipe?.amountIngredient9}<br/>
    </Typography>
export default RecipeComponent;
