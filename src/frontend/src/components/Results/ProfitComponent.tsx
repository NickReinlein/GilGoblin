import React, {ReactNode} from 'react';
import {Profit} from '../../types/types';

interface ProfitProps {
    profit: Profit;
    children?: ReactNode;
}

const ProfitComponent: React.FC<ProfitProps> = ({profit}) => {
    return (
        <div className="profit">
            <p>Item Id: {profit?.itemId} </p>
            <p>Name: {profit?.name} </p>
            <p>World Id: {profit?.worldId}</p>
            <p>Recipe Id: {profit?.recipeId}</p>
            <p>Average Listing Price: {profit?.averageListingPrice}</p>
            <p>Average Sold: {profit?.averageSold}</p>
            <p>Recipe Cost: {profit?.recipeCost}</p>
            <p>Recipe Result Quantity: {profit?.resultQuantity}</p>
            <p>Recipe Profit vs Sold: {profit?.recipeProfitVsSold}</p>
            <p>Recipe Profit vs Listings: {profit?.recipeProfitVsListings}</p>
            <p>CanHq: {profit.canHq ? `true` : `false`}</p>
            <p>Last Updated: {profit?.updated}</p>
        </div>
    );
};

export default ProfitComponent;