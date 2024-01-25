import React, {useState} from 'react';

interface SortableTableProps<T extends Record<string, string | number>> {
    columns: { key: string; label: string }[];
    data: T[];
    renderRow: (item: T, index: number) => React.ReactNode;
}

interface SortingState {
    column: string;
    ascending: boolean;
}

const SortableTable = <T extends Record<string, string | number>>({
                                                                      columns,
                                                                      data,
                                                                      renderRow
                                                                  }: SortableTableProps<T>) => {
    const [sorting, setSorting] = useState<SortingState>({
        column: columns[0].key,
        ascending: true
    });

    const sortedData = [...data].sort((a, b) => {
        if (sorting === undefined) return 0;
        const columnA = getColumnValue(a, sorting.column);
        const columnB = getColumnValue(b, sorting.column);

        if (columnA < columnB) return sorting.ascending ? -1 : 1;
        if (columnA > columnB) return sorting.ascending ? 1 : -1;
        return 0;
    });

    const handleSort = (column: string) => {
        setSorting((prevSorting) => ({
            column,
            ascending: prevSorting.column === column ? !prevSorting.ascending : true,
        }));
    };

    const getColumnValue = (item: T, column: string): string | number => {
        const value = item[column];
        return (typeof value === 'string' || typeof value === 'number')
            ? value
            : String(value);
    };

    return (
        <table>
            <thead>
            <tr>
                {columns.map((col) => (
                    <th key={col.key} onClick={() => handleSort(col.key)}>
                        {col.label}
                    </th>
                ))}
            </tr>
            </thead>
            <tbody>{sortedData?.map((item, index) => renderRow(item, index))}</tbody>
        </table>
    );
};

export default SortableTable;