import React, { useState } from 'react';
import '../styles/TabComponent.css';

const TabComponent = () => {
    const [activeTab, setActiveTab] = useState('Items');

    const handleTabClick = (tabName: string) => {
        setActiveTab(tabName);
    };

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
                renderComponent(activeTab, `{'id':'1'});
            </div>
        </div>
    );
};

export default TabComponent;
