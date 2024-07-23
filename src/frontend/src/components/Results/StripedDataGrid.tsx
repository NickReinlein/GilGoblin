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
        },
        [`& .${gridClasses.row}`]: {
            color: 'var(--text-color)',
        },
        [`& .evenRow`]: {
            backgroundColor: 'var(--background-even)',
            '&:hover': {
                backgroundColor: 'var(--background-hover)',
            },
            '&.Mui-selected': {
                backgroundColor: 'var(--background-selected)',
                '&:hover': {
                    backgroundColor: 'var(--background-selected-hover)',
                },
            },
        },
        [`& .oddRow`]: {
            backgroundColor: 'var(--background-odd)',
            '&:hover': {
                backgroundColor: 'var(--background-hover)',
            },
            '&.Mui-selected': {
                backgroundColor: 'var(--background-selected)',
                '&:hover': {
                    backgroundColor: 'var(--background-selected-hover)',
                },
            },
        },
    });
});

export default StripedDataGrid;
