import React from 'react';
import {Crafts, Profit, Profits} from '../../../types/types';
import ProfitComponent from './ProfitComponent';
import {convertMultipleCraftsToProfits} from '../../../converters/CraftToProfitConverter';
import '../../../styles/ProfitTableComponent.css';
import ProfitTableHeaderComponent from './ProfitTableHeaderComponent';

interface ProfitTableProps {
    crafts: Crafts;
    columnSort?: string;
    ascending?: boolean;
}

const columnHeaders = [
    '#',
    'Name',
    'Sold Profit',
    'Listings Profit',
    'Avg. Sold',
    'Avg. Listing',
    'Cost',
    'Qty',
    'Age'
]

const sortColumns = (profits: Profits, columnSort: string | number, ascending: boolean | undefined) => {
    if (columnSort && ascending !== undefined) {
        profits.sort((a, b) => {
            const columnA = a[columnSort as keyof Profit];
            const columnB = b[columnSort as keyof Profit];
            if (columnA === null || columnA === undefined || columnB === null || columnB === undefined)
                return 0;

            if (columnA == null && columnB == null) return 0;
            if (columnA == null) return ascending ? -1 : 1;
            if (columnB == null) return ascending ? 1 : -1;

            return columnA < columnB
                ? (ascending ? -1 : 1)
                : columnA > columnB
                    ? (ascending ? 1 : -1)
                    : 0;
        });
    }

    return profits;
};

const ProfitTableComponent: React.FC<ProfitTableProps> = ({
                                                              crafts,
                                                              columnSort = columnHeaders[0],
                                                              ascending = true
                                                          }) => {
    if (crafts === null || crafts === undefined || crafts.length < 1)
        return <div>Press the search button to search for the World's best recipes to craft</div>;

    const profits = convertMultipleCraftsToProfits(crafts);
    const sortedProfits = sortColumns(profits, columnSort, ascending);

    return (
        <div className="profits-table">
            <table>
                <ProfitTableHeaderComponent headers={columnHeaders}/>
                <tbody>
                {
                    sortedProfits.map((profit, index) => (
                        <tr key={index}>
                            <ProfitComponent profit={profit} index={index}/>
                        </tr>
                    ))
                }
                </tbody>
            </table>
        </div>
    );
};

export default ProfitTableComponent;