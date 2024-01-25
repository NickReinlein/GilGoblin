import React from 'react';
import '../../../styles/ProfitTableHeaderComponent.css';

interface ProfitTableHeaderComponent {
    headers: string[];
}

const ProfitTableHeaderComponent: React.FC<ProfitTableHeaderComponent> = ({headers}) => {
    return (
        <thead className="profits-table-headers">
        <tr>{
            headers.map((header: string) => (
                <th>{header}</th>
            ))
        }
        </tr>
        </thead>
    );
};

export default ProfitTableHeaderComponent;