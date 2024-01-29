import React from 'react';
import '../../../styles/ProfitTableHeaderComponent.css';

interface ProfitTableHeaderComponentProps {
    headers: string[],
    onHeaderClick: (clickedHeader: string) => void,
    columnSort?: string,
    ascending?: boolean
}

const ProfitTableHeaderComponent: React.FC<ProfitTableHeaderComponentProps> = (
    {
        headers,
        onHeaderClick,
        columnSort,
        ascending = false
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
                    {columnSort && header === columnSort && (
                        <span className={`sort-arrow ${ascending ? 'asc' : 'desc'}`}>
                            {ascending ? '▲' : '▼'}
                        </span>
                    )}
                </th>
            ))}
        </tr>
        </thead>
    );
};

export default ProfitTableHeaderComponent;