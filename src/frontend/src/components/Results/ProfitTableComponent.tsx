import React from 'react';
import {Craft} from '../../types/types';
import ProfitComponent from './ProfitComponent';
import {convertCraftToProfit} from '../../converters/CraftToProfitConverter';
import '../../styles/ProfitTableComponent.css';

interface ProfitTableProps {
    crafts: Craft[];
}

const ProfitTableComponent: React.FC<ProfitTableProps> = ({crafts}) => {
    return (!crafts || crafts.length === 0)
        ? <div>Press the search button to search for a World's best recipes to craft</div>
        : (
            <div className="profits">
                <table>
                    <thead>
                    <tr>
                        <th>#</th>
                        <th>Name</th>
                        <th>Sold Profit</th>
                        <th>Listings Profit</th>
                        <th>Avg. Sold</th>
                        <th>Avg. Listing</th>
                        <th>Cost</th>
                        <th>Qty</th>
                        <th>Age</th>
                    </tr>
                    </thead>
                    <tbody>
                    {
                        Array.isArray(crafts) &&
                        crafts.map((craft, index) => (
                            <tr key={index}>
                                <ProfitComponent profit={convertCraftToProfit(craft)} index={index}/>
                            </tr>
                        ))
                    }
                    </tbody>
                </table>
            </div>
        );
};

export default ProfitTableComponent;