import React, {useState} from 'react';
import {Crafts, Profit, Profits} from '../../../types/types';
import ProfitComponent from './ProfitComponent';
import ProfitTableHeaderComponent from './ProfitTableHeaderComponent';
import {convertMultipleCraftsToProfits} from '../../../converters/CraftToProfitConverter';
import '../../../styles/ProfitTableComponent.css';

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
                                                              columnSort: initialColumnSort = columnHeaders[0],
                                                              ascending: initialAscending = true,
                                                          }) => {
    const [localColumnSort, setLocalColumnSort] = useState<string | number>(initialColumnSort);
    const [localAscending, setLocalAscending] = useState<boolean>(initialAscending);
    const handleHeaderClick = (clickedColumn: string) => {
        if (localColumnSort === clickedColumn) {
            setLocalAscending((prevAscending) => !prevAscending);
        } else {
            setLocalColumnSort(clickedColumn);
            setLocalAscending(false);
        }
    };

    if (!(crafts?.length > 0))
        return (<div>Press the search button to search for the World's best recipes to craft</div>);

    const profits = convertMultipleCraftsToProfits(crafts);
    const sortedProfits = sortColumns(profits, localColumnSort, localAscending);


    return (
        <div className="profits-table">
            <table>
                <ProfitTableHeaderComponent headers={columnHeaders} onHeaderClick={handleHeaderClick}/>
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