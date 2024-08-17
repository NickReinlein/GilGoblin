import React from 'react';
import {Recipe} from '../../../types/types';
import {IDataProps} from "./IDataProps";
import Typography from "@mui/material/Typography";
import BoxedCardComponent from "../BoxedCardComponent";
import IngredientComponent from "./IngredientComponent";
import '../../../styles/RecipeComponent.css';

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
        <Typography className="MuiTypography-id">
            <span>Recipe Id: {recipe?.id || 'Missing'}</span><br/>
        </Typography>
        <span>Craft Type: {recipe?.craftType}</span><br/>
        <span>Recipe Level Table: {recipe?.recipeLevelTable}</span><br/>
        <span>Target Item Id: {recipe?.targetItemId}</span><br/>
        <span>Result Quantity: {recipe?.resultQuantity}</span><br/>
        <span>Can Hq: {recipe?.canHq ? 'Yes' : 'No'}</span><br/>
        <span>Can QuickSynth: {recipe?.canQuickSynth ? 'Yes' : 'No'}</span><br/>
    </Typography>

const ingredientsContent = (recipe: Recipe | null | undefined) =>
    <div>
        <Typography className="MuiTypography-ingredients-title">
            Ingredients
        </Typography>
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
    </div>
export default RecipeComponent;
