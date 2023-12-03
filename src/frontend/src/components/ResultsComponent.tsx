import React from 'react';
import CraftingComponent from './CraftComponent';
import PricesComponent from './PriceComponent';
import ItemsComponent from './ItemComponent';
import RecipesComponent from './RecipeComponent';

const renderComponent = (componentName: string, data: any) => {
    switch (componentName) {
        case 'Items':
            return <ItemsComponent data={data} />;
        case 'Recipes':
            return <RecipesComponent data={data} />;
        case 'Prices':
            return <PricesComponent data={data} />;
        case 'Crafting':
            return <CraftingComponent data={data} />;
        default:
            return null;
    }
};

// Usage example:
const componentName = 'Prices'; // Example string
const data = {}; // Example data to pass to the component
const ComponentToRender = renderComponent(componentName, data);

export default ComponentToRender;
