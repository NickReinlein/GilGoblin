import React from 'react';
import {Craft, Profit} from '../../types/types';
import ProfitComponent from './ProfitComponent';
import SortableTable from "../SortableTableComponent";
import {convertCraftToProfit, convertMultipleCraftsToProfits} from '../../converters/CraftToProfitConverter';
import '../../styles/ProfitTableComponent.css';

interface ProfitTableProps {
    crafts: Craft[];
}

const columns = [
    {key: 'name', label: 'Name'},
    {key: 'profitSold', label: 'Sold Profit'},
    {key: 'profitListings', label: 'Listings Profit'},
    {key: 'averageSold', label: 'Avg. Sold'},
    {key: 'averageListing', label: 'Avg. Listing'},
    {key: 'cost', label: 'Cost'},
    {key: 'resultQuantity', label: 'Qty'},
    {key: 'age', label: 'Age'}
];

const ProfitTableComponent: React.FC<ProfitTableProps> = ({crafts}) => {
    return (!crafts || crafts.length === 0)
        ? <div>Press the search button to search for a World's best recipes to craft</div>
        : (
            // const profits = convertMultipleCraftsToProfits(crafts)
            <div className="profits">
                <SortableTable
                    columns={columns}
                    data={profits}
                    renderRow={(profit: Profit, index: number) => (
                        <tr key={index}>
                            <td>{profit.name}</td>
                            <td>{profit.averageSold}</td>
                            <td>{profit.averageListing}</td>
                            {/* Add more columns as needed */}
                        </tr>
                    )}
                />
                {/*<SortableTable columns={columns} data={convertMultipleCraftsToProfits(crafts)} renderRow={(craft, index) => (*/}
                {/*    <tr key={index}>*/}
                {/*        <ProfitComponent profit={convertCraftToProfit(craft)} index={index}/>*/}
                {/*    </tr>*/}
                {/*)}/>;*/}
            </div>
        )
};

export default ProfitTableComponent;