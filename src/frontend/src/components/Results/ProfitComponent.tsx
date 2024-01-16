import React, {ReactNode} from 'react';
import {Profit} from '../../types/types';

interface ProfitProps {
    profit: Profit;
    index: number;
    children?: ReactNode;
}

const ProfitComponent: React.FC<ProfitProps> = ({profit, index}) => {
    return (<>
            <td>{index + 1 ?? ''}</td>
            <td>{profit?.name}</td>
            <td>{profit?.recipeProfitVsSold}</td>
            <td>{profit?.recipeProfitVsListings}</td>
            <td>{profit?.averageSold}</td>
            <td>{profit?.averageListingPrice}</td>
            <td>{profit?.recipeCost}</td>
            <td>{profit?.resultQuantity}</td>
            <td>{profit?.recipeId}</td>
            <td>{profit?.itemId}</td>
            <td>{profit?.worldId}</td>
            <td>{profit?.updated}</td>
        </>
    );
};

export default ProfitComponent;