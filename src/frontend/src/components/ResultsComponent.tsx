import React from 'react';
import CraftingComponent from './CraftComponent';
import PricesComponent from './PriceComponent';
import ItemsComponent from './ItemComponent';
import RecipesComponent from './RecipeComponent';

const renderComponent = (componentName: string, data: any) => {
    switch (componentName) {
        case 'Items':
            return <ItemsComponent item={data} />;
        case 'Recipes':
            return <RecipesComponent recipe={data} />;
        case 'Prices':
            return <PricesComponent price={data} />;
        case 'Crafting':
            return <CraftingComponent craftSummary={data} />;
        default:
            return null;
    }
};

export default ComponentToRender;
