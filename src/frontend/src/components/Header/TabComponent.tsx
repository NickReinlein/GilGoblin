import React, {useState} from 'react';
import '../../styles/Tab.css';
import SearchComponent from './SearchComponent';
import TabButtonsComponent from './TabButtonsComponent';
import ResultsComponent from "../Results/ResultsComponent";
import DataFetcher from "../DataFetcher";

const TabComponent = () => {
    const [activeTab, setActiveTab] = useState(tabNames[0]);
    const [tabData, setTabData] = useState({});

    const handleTabClick = (tabName: string) => {
        setActiveTab(tabName);
    };

    const handleSearchClick = async (id: number, world: number) => {
        console.log(`Fetching data for id ${id} world ${world}`);
        const result = await getDataForTab(activeTab, id, world);
        if (result)
            setTabData(result);
    };

    const getDataForTab = async (tabName: string, id: number, world: number) => {
        try {
            return await DataFetcher.fetchData(tabName, id, world);
        } catch (error) {
            console.error(`Error fetching data for id ${id} world ${world}`);
            return null;
        }
    }

    return (
        <div className="tab-container">
            <div className="tabs">
                {tabNames.map((tabName) => (
                    <TabButtonsComponent
                        key={tabName}
                        tabName={tabName}
                        activeTab={activeTab}
                        onClick={handleTabClick}
                    >
                        {tabName}
                    </TabButtonsComponent>
                ))}
            </div>
            <div className="search-container">
                <SearchComponent onClick={handleSearchClick}/>
            </div>
            <div className="results-container">
                <ResultsComponent componentName={activeTab} data={tabData}/>
            </div>
        </div>
    );
};

export const tabNames = ['Profits', 'Crafts', 'Items', 'Recipes', 'Prices'];

export default TabComponent;