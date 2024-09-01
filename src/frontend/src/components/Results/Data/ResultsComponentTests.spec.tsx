import React from 'react';
import {render, screen} from '@testing-library/react';
import ResultsComponent from './ResultsComponent';

jest.mock('./ItemComponent', () => ({item}: { item: any }) => <div>{item.name}</div>);
jest.mock('./RecipeComponent', () => ({recipe}: { recipe: any }) => <div>{recipe.name}</div>);
jest.mock('./PriceComponent', () => ({price}: { price: any }) => <div>{price.amount}</div>);

describe('ResultsComponent', () => {
    it('renders ItemsComponent correctly', () => {
        const data = {name: 'Item Name'};
        render(<ResultsComponent componentName="Items" data={data}/>);
        expect(screen.getByText('Item Name')).toBeInTheDocument();
    });

    it('renders RecipeComponent.css correctly', () => {
        const data = {name: 'Recipe Name'};
        render(<ResultsComponent componentName="Recipes" data={data}/>);
        expect(screen.getByText('Recipe Name')).toBeInTheDocument();
    });

    it('renders PriceComponent correctly', () => {
        const data = {amount: 50};
        render(<ResultsComponent componentName="Prices" data={data}/>);
        expect(screen.getByText('50')).toBeInTheDocument();
    });

    it('returns null for unknown component', () => {
        const data = {unknownProp: 'Unknown'};
        render(<ResultsComponent componentName="Unknown" data={data}/>);
        expect(screen.queryByText('Unknown')).toBeNull();
    });
});
