import React from 'react';
import '../../../styles/ProfitTableHeaderComponent.css';

interface ProfitTableHeaderComponentProps {
    headers: string[];
    onHeaderClick: (clickedHeader: string) => void;
}

const ProfitTableHeaderComponent: React.FC<ProfitTableHeaderComponentProps> = (
    {
        headers,
        onHeaderClick,
    }) => {
    const handleHeaderClick = (clickedHeader: string) => {
        onHeaderClick(clickedHeader);
    };

    return (
        <thead className="profits-table-headers">
        <tr>
            {headers.map((header: string) => (
                <th key={header} onClick={() => handleHeaderClick(header)}>
                    {header}
                </th>
            ))}
        </tr>
        </thead>
    );
};

export default ProfitTableHeaderComponent;