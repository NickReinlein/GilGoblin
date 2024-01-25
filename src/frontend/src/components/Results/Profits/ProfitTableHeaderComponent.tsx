import React from 'react';
import '../../../styles/ProfitTableHeaderComponent.css';
import {Crafts} from "../../../types/types";

const ProfitTableHeaderComponent: React.FC = () => {
    return (
        <thead className="profits-table-headers">
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
    );
};

export default ProfitTableHeaderComponent;