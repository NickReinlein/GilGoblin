import React, {SyntheticEvent, useState} from 'react';
import {Box, Tab, Tabs} from '@mui/material';
import SearchComponent from './SearchComponent';
import ResultsComponent from '../Results/Data/ResultsComponent';
import TitleComponent from '../Header/TitleComponent';
import DataFetcher from '../DataFetcher';
import '../../styles/Tab.css';

const TabComponent = () => {
    const [activeTab, setActiveTab] = useState(buttonTitles[0]);
    const [tabData, setTabData] = useState({});

    const handleChange = (event: SyntheticEvent, newValue: string) => {
        setActiveTab(newValue);
    };

    const handleSearchClick = async (id: number, world: number) => {
        console.log(`Fetching data for id ${id} world ${world}`);
        const result = await getDataForTab(activeTab, id, world);
        if (result) setTabData(result);
    };

    const getDataForTab = async (tabName: string, id: number, world: number) => {
        try {
            return await DataFetcher.fetchData(tabName, id, world);
        } catch (error) {
            console.error(`Error fetching data for id ${id} world ${world}`);
            return null;
        }
    };

    return (
        <div className="tab-container">
            <div className="tabs-header">
                <TitleComponent/>
                <div className="tabs-wrapper">
                    <Box sx={{borderBottom: 1, borderColor: 'divider'}}>
                        <Tabs value={activeTab} onChange={handleChange} indicatorColor={'primary'}>
                            {buttonTitles.map((title, index) => (
                                <Tab
                                    key={index}
                                    label={title}
                                    value={title}
                                    className={activeTab === title ? 'header-tab-selected' : 'header-tab'}
                                    sx={{
                                        '&': {
                                            color: 'white',
                                            border: `2px solid var(--background-selected)`,
                                            borderRadius: '15px',
                                            margin: '10px',
                                        },
                                        '&:hover': {
                                            backgroundColor: 'var(--background-hover)',
                                        },
                                        '&.Mui-selected': {
                                            color: 'white',
                                            backgroundColor: 'var(--background-selected)',
                                            border: `2px solid white`,
                                            '&:hover': {
                                                backgroundColor: 'var(--background-selected-hover)',
                                            },
                                        },
                                    }}
                                />
                            ))}
                        </Tabs>
                    </Box>
                </div>
                <div className="search-container" >
                    <SearchComponent onClick={handleSearchClick}/>
                </div>
            </div>
            <div className="results-container">
                <ResultsComponent componentName={activeTab} data={tabData}/>
            </div>
        </div>
    );
};

export const buttonTitles = ['Profits', 'Items', 'Recipes', 'Prices', 'About'];

export default TabComponent;
