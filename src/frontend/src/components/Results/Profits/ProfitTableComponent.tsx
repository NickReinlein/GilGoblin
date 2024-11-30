import React from 'react';
import {Crafts, Profit} from '../../../types/types';
import {convertMultipleCraftsToProfits} from '../../../converters/CraftToProfitConverter';
import {GridColDef} from "@mui/x-data-grid";
import StripedDataGrid, {getRowClassName} from "../StripedDataGrid";
import '../../../styles/ProfitTableComponent.css';

interface ProfitTableProps {
    crafts: Crafts;
}

const profitTableHeaders: GridColDef<(Profit)[number]>[] = [
    {field: 'recipeId', headerName: 'Recipe Id', minWidth: 70, maxWidth: 100, type: 'number'},
    {field: 'worldId', headerName: 'World Id', minWidth: 70, maxWidth: 100, type: 'number'},
    {
        field: 'isHq',
        headerName: 'Is HQ',
        minWidth: 70,
        maxWidth: 100,
        type: 'boolean',
        cellClassName: () => 'whiteCheckmark'
    },
    {field: 'itemId', headerName: 'Item Id', minWidth: 70, maxWidth: 100, type: 'number'},
    {field: 'name', headerName: 'Name', minWidth: 100, maxWidth: 250, type: 'string', flex: 1},
    {field: 'salePrice', headerName: 'Sale Price', type: 'number', minWidth: 100, maxWidth: 150, flex: 1},
    {field: 'craftingCost', headerName: 'Crafting Cost', type: 'number', minWidth: 100, maxWidth: 150, flex: 1},
    {field: 'profit', headerName: 'Profit', type: 'number', minWidth: 150, maxWidth: 200, flex: 1,
        cellClassName: (params) => params.value > 0 ? 'positiveProfit' : 'negativeProfit'},
    {field: 'resultQuantity', headerName: 'Qty', type: 'number', width: 50, flex: 1},
    {field: 'iconId', headerName: 'Icon Id', type: 'number', width: 50, flex: 1},
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

export const convertTimestampToAge = (timestamp: string): string => {
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

const ProfitTableComponent: React.FC<ProfitTableProps> = ({crafts,}) => {
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
                                    field: 'profit',
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