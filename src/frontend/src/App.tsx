import React from 'react';
import TabComponent from "./components/Header/TabComponent";
import TitleComponent from "./components/Header/TitleComponent";


const App = () => {
    return (
        <div data-testid="app-tab">
            <TitleComponent/>
            <TabComponent/>
        </div>
    );
};

export default App;