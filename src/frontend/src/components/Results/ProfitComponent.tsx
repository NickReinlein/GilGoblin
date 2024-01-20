import React, {ReactNode} from 'react';
import {Profit} from '../../types/types';
import TimestampToAgeConverter from "../../converters/TimestampToAgeConverter";

interface ProfitProps {
    profit: Profit;
    index: number;
    children?: ReactNode;
}

const ProfitComponent: React.FC<ProfitProps> = ({profit, index}) => {
    return (<>
            <td>{index + 1 ?? ''}</td>
            <td>{profit?.name}</td>
            <td>{profit?.recipeProfitVsSold.toLocaleString()}</td>
            <td>{profit?.recipeProfitVsListings.toLocaleString()}</td>
            <td>{profit?.averageSold.toLocaleString()}</td>
            <td>{profit?.averageListingPrice.toLocaleString()}</td>
            <td>{profit?.recipeCost.toLocaleString()}</td>
            <td>{profit?.resultQuantity}</td>
            <td><TimestampToAgeConverter timestamp={profit?.updated}/></td>
        </>
    );
};

export default ProfitComponent;