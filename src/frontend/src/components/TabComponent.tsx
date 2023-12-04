import React, {useState} from 'react';
import '../styles/TabComponent.css';
import ResultsComponent from './ResultsComponent';

const TabComponent = () => {
    const [activeTab, setActiveTab] = useState('Items');

    const handleTabClick = (tabName: string) => {
        setActiveTab(tabName);
    };

    const data = {};

    return (
        <div className="tab-container">
            <div className="tabs">
                <button
                    className={activeTab === 'Items' ? 'active' : ''}
                    onClick={() => handleTabClick('Items')}
                >
                    Items
                </button>
                <button
                    className={activeTab === 'Recipes' ? 'active' : ''}
                    onClick={() => handleTabClick('Recipes')}
                >
                    Recipes
                </button>
                <button
                    className={activeTab === 'Prices' ? 'active' : ''}
                    onClick={() => handleTabClick('Prices')}
                >
                    Prices
                </button>
                <button
                    className={activeTab === 'Crafting' ? 'active' : ''}
                    onClick={() => handleTabClick('Crafting')}
                >
                    Crafting
                </button>
            </div>
            <div className="results">
                <ResultsComponent componentName={activeTab} data={data}/>
            </div>
        </div>
    );
};

export default TabComponent;