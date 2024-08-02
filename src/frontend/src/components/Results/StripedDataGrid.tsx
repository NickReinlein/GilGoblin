import { styled } from '@mui/system';
import { DataGrid, gridClasses } from '@mui/x-data-grid';
import '../../styles/StripedDataGrid.css';

export const getRowClassName = (params: { indexRelativeToCurrentPage: number; }) => {
    return params.indexRelativeToCurrentPage % 2 === 0 ? 'evenRow' : 'oddRow';
};

const StripedDataGrid = styled(DataGrid)(() => {
    return ({
        [`& .${gridClasses.columnHeader}`]: {
            backgroundColor: 'var(--background-headers)',
            color: 'var(--text-color)',
            autosizeOnMount: true,
        },
        [`& .${gridClasses.row}`]: {
            color: 'var(--text-color)',
            '&:hover': {
                backgroundColor: 'var(--background-hover)',
                fontWeight: 'bold',
            },
            '&.Mui-selected': {
                backgroundColor: 'var(--background-selected)',
                fontStyle: 'italic',
                '&:hover': {
                    backgroundColor: 'var(--background-selected-hover)',
                },
            },
        },
        [`& .evenRow`]: {
            backgroundColor: 'var(--background-even)',
        },
        [`& .oddRow`]: {
            backgroundColor: 'var(--background-odd)',
        },
    });
});

export default StripedDataGrid;
