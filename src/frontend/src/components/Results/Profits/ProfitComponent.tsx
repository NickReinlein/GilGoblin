import React, {ReactNode} from 'react';
import {Profit} from '../../../types/types';
import TimestampToAgeConverter from "../../../converters/TimestampToAgeConverter";

interface ProfitProps {
    profit: Profit;
    index: number;
    children?: ReactNode;
}

const ProfitComponent: React.FC<ProfitProps> = ({profit, index}) => {
    return (<>
            <td>{index + 1 ?? ''}</td>
            <td>{profit?.name}</td>
            <td>{profit?.profitSold.toLocaleString()}</td>
            <td>{profit?.profitListings.toLocaleString()}</td>
            <td>{profit?.averageSold.toLocaleString()}</td>
            <td>{profit?.averageListing.toLocaleString()}</td>
            <td>{profit?.cost.toLocaleString()}</td>
            <td>{profit?.resultQuantity}</td>
            <td><TimestampToAgeConverter timestamp={profit?.updated}/></td>
        </>
    );
};

export default ProfitComponent;