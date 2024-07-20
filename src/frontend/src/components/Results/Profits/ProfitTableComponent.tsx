import React from 'react';
import {Crafts, Profit} from '../../../types/types';
import {convertMultipleCraftsToProfits} from '../../../converters/CraftToProfitConverter';
import '../../../styles/ProfitTableComponent.css';
import {GridColDef} from "@mui/x-data-grid";
import StripedDataGrid from "../StripedDataGrid";

interface ProfitTableProps {
    crafts: Crafts;
    columnSort?: string;
    ascending?: boolean;
}

const profitTableHeaders: GridColDef<(Profit)[number]>[] = [
    {field: 'index', headerName: '#', width: 70},
    {field: 'name', headerName: 'Name', width: 300},
    {field: 'profitSold', headerName: 'Sold Profit', type: 'number', width: 150},
    {field: 'profitListings', headerName: 'Listings Profit', type: 'number', width: 150},
    {field: 'averageSold', headerName: 'Avg. Sold', type: 'number', width: 150},
    {field: 'averageListing', headerName: 'Avg. Listing', type: 'number', width: 150},
    {field: 'cost', headerName: 'Cost', type: 'number', width: 150},
    {field: 'resultQuantity', headerName: 'Qty', type: 'number', width: 100},
    {
        field: 'updated',
        headerName: 'Age',
        width: 150,
        type: 'string',
        valueGetter: (value, row) => convertTimestampToAge(row.updated),
    },
];

const convertTimestampToAge = (timestamp: string): string => {
    const date = new Date(timestamp);
    const now = new Date();

    const ageInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (ageInSeconds < 60) {
        return `${ageInSeconds} seconds ago`;
    } else if (ageInSeconds < 3600) {
        const ageInMinutes = Math.floor(ageInSeconds / 60);
        return `${ageInMinutes} minutes ago`;
    } else if (ageInSeconds < 86400) {
        const ageInHours = Math.floor(ageInSeconds / 3600);
        return `${ageInHours} hours ago`;
    } else {
        const ageInDays = Math.floor(ageInSeconds / 86400);
        return `${ageInDays.toLocaleString()} days ago`;
    }
}

const ProfitTableComponent: React.FC<ProfitTableProps> = ({
                                                              crafts,
                                                          }) => {
    if (!(crafts?.length > 0))
        return (<div>Press the search button to search for the World's best recipes to craft</div>);

    const profits = convertMultipleCraftsToProfits(crafts);
    const profitsMapped = profits.map((profit: Profit, index: number) => {
        return {
            ...profit,
            id: index,
        }
    })

    return (
        <div className="profits-table" style={{height: 800, width: '80%'}}>
            <StripedDataGrid
                rows={profitsMapped}
                columns={profitTableHeaders}
                autosizeOnMount={true}
                initialState={
                    {
                        sorting: {
                            sortModel: [
                                {
                                    field: 'profitSold',
                                    sort: 'desc'
                                }
                            ]
                        }
                    }
                }
            />
        </div>
    );
};

export default ProfitTableComponent;