import React, {useState} from 'react';
import '../styles/TabComponent.css';
import '../styles/Sparkles.css';
import ResultsComponent from './ResultsComponent';

const TabComponent = () => {
    const [activeTab, setActiveTab] = useState('Items');
    const [clicked, setClicked] = useState(false);

    const handleTabClick = (tabName: string) => {
        setActiveTab(tabName);
        setTimeout(() => {
            setClicked(false);
        }, 2000);
    };

    const data = {};

    return (
        <div className="tab-container">
            <div className="tabs">
                <button
                    className={`button-sparkle ${activeTab === 'Items' ? 'active' : ''} ${clicked ? 'clicked' : ''}`}
                    onClick={() => {
                        setClicked(true)
                        handleTabClick('Items')
                    }
                    }
                >
                    Items
                </button>
                <button
                    className={`button-sparkle ${activeTab === 'Recipes' ? 'active' : ''} ${clicked ? 'clicked' : ''}`}
                    onClick={() => {
                        setClicked(true)
                    }
                    }
                >
                    Recipes
                </button>
                <button
                    className={`button-sparkle ${activeTab === 'Prices' ? 'active' : ''} ${clicked ? 'clicked' : ''}`}

                    onClick={() => {
                        setClicked(true)
                        handleTabClick('Prices')
                    }
                    }
                >
                    Prices
                </button>
                <button
                    className={`button-sparkle ${activeTab === 'Crafting' ? 'active' : ''} ${clicked ? 'clicked' : ''}`}
                    onClick={() => {
                        setClicked(true)
                        handleTabClick('Crafting')
                    }
                    }
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