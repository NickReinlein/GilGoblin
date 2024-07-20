import { styled } from '@mui/system';
import { DataGrid, gridClasses } from '@mui/x-data-grid';

// Define the colors
const EVEN_ROW_BACKGROUND = 'rgba(0, 0, 0, 1)';
const EVEN_ROW_BACKGROUND_HOVER = 'rgba(150, 150, 150, 1)';
const EVEN_ROW_BACKGROUND_SELECTED = 'rgba(200, 200, 200, 1)';
const EVEN_ROW_BACKGROUND_SELECTED_HOVER = 'rgba(255, 255, 255, 1)';
;

const ODD_ROW_BACKGROUND = 'rgba(50, 50, 50, 1)';
const ODD_ROW_BACKGROUND_HOVER = 'rgba(100, 100, 100, 1)';
const ODD_ROW_BACKGROUND_SELECTED = 'rgba(200, 200, 200, 1)';
const ODD_ROW_BACKGROUND_SELECTED_HOVER = 'rgba(255, 255, 255, 1)';

const StripedDataGrid = styled(DataGrid)(({ theme }) => ({
    [`& .${gridClasses.row}`]: {
        color: 'white', // Ensure text is white
        textAlign: 'center', // Center text
    },
    [`& .${gridClasses.row}.even`]: {
        backgroundColor: EVEN_ROW_BACKGROUND,
        '&:hover': {
            backgroundColor: EVEN_ROW_BACKGROUND_HOVER,
        },
        '&.Mui-selected': {
            backgroundColor: EVEN_ROW_BACKGROUND_SELECTED,
            '&:hover': {
                backgroundColor: EVEN_ROW_BACKGROUND_SELECTED_HOVER,
            },
        },
    },
    [`& .${gridClasses.row}.odd`]: {
        backgroundColor: ODD_ROW_BACKGROUND,
        '&:hover': {
            backgroundColor: ODD_ROW_BACKGROUND_HOVER,
        },
        '&.Mui-selected': {
            backgroundColor: ODD_ROW_BACKGROUND_SELECTED,
            '&:hover': {
                backgroundColor: ODD_ROW_BACKGROUND_SELECTED_HOVER,
            },
        },
    },
}));

export default StripedDataGrid;
