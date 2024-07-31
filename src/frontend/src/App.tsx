import React from 'react';
import TabComponent from "./components/Header/TabComponent";
import {createTheme, ThemeProvider} from "@mui/material";

const theme = createTheme({
    palette: {
        primary: {
            main: '#001E00FF',
        },
        secondary: {
            main: '#006400FF',
        },
        text: {
            primary: '#FFFFFF',
            secondary: '#FFFFFF',
        },
    },
});

const App = () => {
    return (
        <ThemeProvider theme={theme}>
            <div className="app" data-testid="app">
                <TabComponent/>
            </div>
        </ThemeProvider>
    );
};

export default App;