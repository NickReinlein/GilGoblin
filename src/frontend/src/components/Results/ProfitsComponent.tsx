import React from 'react';
import {Craft} from '../../types/types';
import ProfitComponent from './ProfitComponent';
import {convertCraftToProfit} from "../../converters/CraftToProfitConverter";

interface ProfitsProps {
    crafts: Craft[];
}

const ProfitsComponent: React.FC<ProfitsProps> = ({crafts}) => {
    return (
        <div className="profits">
            <ol>
                {crafts.map((craft) => (
                    <li key={craft?.itemId}>
                        <ProfitComponent profit={convertCraftToProfit(craft)}/>
                    </li>
                ))}
            </ol>
        </div>
    );
};

export default ProfitsComponent;