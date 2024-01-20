import React from 'react';
import '../../styles/Results.css';
import PricesComponent from './PriceComponent';
import ItemsComponent from './ItemComponent';
import RecipesComponent from './RecipeComponent';
import ProfitTableComponent from "./ProfitTableComponent";
import CraftComponent from "./CraftComponent";

interface ResultsProps {
    componentName: string;
    data: any;
}

const ResultsComponent: React.FC<ResultsProps> = ({componentName, data}) => {
    switch (componentName) {
        case 'Items':
            return <ItemsComponent item={data}/>;
        case 'Recipes':
            return <RecipesComponent recipe={data}/>;
        case 'Prices':
            return <PricesComponent price={data}/>;
        case 'Crafts':
            return <CraftComponent craft={data}/>;
        case 'Profits':
            return <ProfitTableComponent crafts={data}/>;
        default:
            return null;
    }
};

export default ResultsComponent;
