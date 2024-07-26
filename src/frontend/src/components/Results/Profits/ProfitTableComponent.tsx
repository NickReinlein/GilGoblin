import React from 'react';
import {Crafts, Profit} from '../../../types/types';
import {convertMultipleCraftsToProfits} from '../../../converters/CraftToProfitConverter';
import {GridColDef} from "@mui/x-data-grid";
import StripedDataGrid, {getRowClassName} from "../StripedDataGrid";

interface ProfitTableProps {
    crafts: Crafts;
}

const profitTableHeaders: GridColDef<(Profit)[number]>[] = [
    {field: 'id', headerName: '#', minWidth: 70, maxWidth: 100, type: 'number'},
    {field: 'name', headerName: 'Name', minWidth: 300, type: 'string', flex: 1},
    {field: 'profitSold', headerName: 'Sold Profit', type: 'number', minWidth: 150, flex: 1},
    {field: 'profitListings', headerName: 'Listings Profit', type: 'number', minWidth: 150, flex: 1},
    {field: 'averageSold', headerName: 'Avg. Sold', type: 'number', minWidth: 150, flex: 1},
    {field: 'averageListing', headerName: 'Avg. Listing', type: 'number', minWidth: 150, flex: 1},
    {field: 'cost', headerName: 'Cost', type: 'number', minWidth: 150, flex: 1},
    {field: 'resultQuantity', headerName: 'Qty', type: 'number', minWidth: 100, flex: 1},
    {
        field: 'updated',
        headerName: 'Age',
        minWidth: 200,
        maxWidth: 400,
        flex: 1,
        type: 'string',
        valueGetter: (value, row) => convertTimestampToAge(row.updated),
    },
];

const convertTimestampToAge = (timestamp: string): string => {
    const date = new Date(timestamp);
    const now = new Date();
    const ageInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    return timeAgo(ageInSeconds);
}

const timeAgo = (ageInSeconds: number) => {
    switch (true) {
        case ageInSeconds < 60:
            return `${ageInSeconds} seconds ago`;
        case ageInSeconds < 3600:
            const ageInMinutes = Math.floor(ageInSeconds / 60);
            return `${ageInMinutes} minutes ago`;
        case ageInSeconds < 86400:
            const ageInHours = Math.floor(ageInSeconds / 3600);
            return `${ageInHours} hours ago`;
        default:
            const ageInDays = Math.floor(ageInSeconds / 86400);
            return `${ageInDays.toLocaleString()} days ago`;
    }
};

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
        <div className="profits-table" style={{minHeight: 800, width: '90%'}}>
            <StripedDataGrid
                rows={profitsMapped}
                columns={profitTableHeaders}
                getRowClassName={getRowClassName}
                className="dataGrid"
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