import React from 'react';

interface TabButtonsComponentProps {
    tabName: string;
    activeTab: string;
    onClick: (tabName: string) => void;
    children: React.ReactNode;
}

const TabButtonsComponent: React.FC<TabButtonsComponentProps> = ({tabName, activeTab, onClick, children}) => {
    return (
        <button
            className={activeTab === tabName ? 'active' : ''}
            onClick={() => onClick(tabName)}
        >
            {children}
        </button>
    );
};

export default TabButtonsComponent;