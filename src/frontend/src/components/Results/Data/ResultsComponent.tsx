import React from 'react';
import '../../../styles/Results.css';
import PricesComponent from './PriceComponent';
import ItemsComponent from './ItemComponent';
import RecipesComponent from './RecipeComponent';
import ProfitTableComponent from "../Profits/ProfitTableComponent";
import AboutPageComponent from "../../Header/AboutPageComponent";

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
        case 'Profits':
            return <ProfitTableComponent crafts={data}/>;
        case 'About':
            return <AboutPageComponent/>
        default:
            return null;
    }
};

export default ResultsComponent;
