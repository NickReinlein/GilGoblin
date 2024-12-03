import React, {ReactNode} from 'react';
import {Profit} from '../../../types/types';
import TimestampToAgeConverter from "../../../converters/TimestampToAgeConverter";

interface ProfitProps {
    profit: Profit;
    index: number;
    children?: ReactNode;
}

const ProfitComponent: React.FC<ProfitProps> = ({profit, index}) => {
    return (
        <tr>
            <td title={`Index: ${index}`}>{index}</td>
            <td title={`Recipe ID: ${profit?.recipeId}`}>{profit?.recipeId}</td>
            <td title={`World ID: ${profit?.worldId}`}>{profit?.worldId}</td>
            <td title={profit?.isHq ? 'High Quality' : 'Not High Quality'}>
                {profit?.isHq ? '✔️' : '❌'}
            </td>
            <td title={`Item ID: ${profit?.itemId}`}>{profit?.itemId}</td>
            <td title={`Sale Price: ${profit?.salePrice?.toLocaleString() || 'N/A'}`}>
                {profit?.salePrice?.toLocaleString() || 'N/A'}
            </td>
            <td title={`Crafting Cost: ${profit?.craftingCost?.toLocaleString() || 'N/A'}`}>
                {profit?.craftingCost?.toLocaleString() || 'N/A'}
            </td>
            <td title={`Profit: ${profit?.profit?.toLocaleString() || 'N/A'}`}>
                {profit?.profit?.toLocaleString() || 'N/A'}
            </td>
            <td title={`Quantity: ${profit?.resultQuantity}`}>{profit?.resultQuantity}</td>
            <td title={`Name: ${profit?.name}`}>{profit?.name}</td>
            <td title={`Icon ID: ${profit?.iconId}`}>{profit?.iconId}</td>
            <td title={`Updated: ${profit?.updated}`}>
                <TimestampToAgeConverter timestamp={profit?.updated}/>
            </td>
        </tr>
    );
};

export default ProfitComponent;