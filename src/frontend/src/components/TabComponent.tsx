import React, {useState} from 'react';
import '../styles/Tab.css';
import SearchInputComponent from './SearchInputComponent';

import TabButtonsComponent from './TabButtonsComponent';
import ResultsComponent from "./ResultsComponent";

const TabComponent = () => {
    const tabNames = ['Items', 'Recipes', 'Prices', 'Crafting'];
    const [activeTab, setActiveTab] = useState(tabNames[0]);
    const [tabData, setTabData] = useState({});

    const handleTabClick = (tabName: string) => {
        setActiveTab(tabName);
        const result = getDataForTab(tabName);
        if (result)
            setTabData(result);
    };
    const handleSearchClick = (id: number, world: number) => {
        // get data and set tab data
    };

    const getDataForTab = (tabName: string) => {
        switch (tabName) {
            case 'Items':
                return {
                    "id": 1639,
                    "name": "Steel Longsword",
                    "description": "A sharp longsword made of steel",
                    "iconId": 30435,
                    "level": 36,
                    "stackSize": 1,
                    "priceMid": 4795,
                    "priceLow": 41,
                    "canHq": true
                }
            case 'Recipes':
                return {
                    "id": 3,
                    "craftType": 1,
                    "recipeLevelTable": 2,
                    "targetItemId": 1602,
                    "resultQuantity": 1,
                    "canHq": true,
                    "canQuickSynth": true,
                    "itemIngredient0TargetId": 5056,
                    "amountIngredient0": 1,
                    "itemIngredient1TargetId": 5361,
                    "amountIngredient1": 1,
                    "itemIngredient2TargetId": 5432,
                    "amountIngredient2": 1,
                    "itemIngredient3TargetId": 0,
                    "amountIngredient3": 0,
                    "itemIngredient4TargetId": 0,
                    "amountIngredient4": 0,
                    "itemIngredient5TargetId": 0,
                    "amountIngredient5": 0,
                    "itemIngredient6TargetId": 0,
                    "amountIngredient6": 0,
                    "itemIngredient7TargetId": 0,
                    "amountIngredient7": 0,
                    "itemIngredient8TargetId": 2,
                    "amountIngredient8": 1,
                    "itemIngredient9TargetId": 5,
                    "amountIngredient9": 1
                };
            case 'Prices':
                return {
                    "itemId": 1639,
                    "worldId": 34,
                    "averageListingPrice": 10500,
                    "averageListingPriceNQ": 0,
                    "averageListingPriceHQ": 10500,
                    "averageSold": 14470.15,
                    "averageSoldNQ": 7356,
                    "averageSoldHQ": 21584.3,
                    "lastUploadTime": new Date(1698281137868).toUTCString()
                };
            case 'Crafting':
                return {};
            default:
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
                <SearchInputComponent
                    id={1639}
                    world={34}
                    onClick={handleSearchClick}/>
            </div>
            <div className="results-container">
                <ResultsComponent
                    componentName={activeTab}
                    data={tabData}/>
            </div>
        </div>
    );
};

export default TabComponent;