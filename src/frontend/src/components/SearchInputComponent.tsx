import React from 'react';
import '../styles/Search.css';

interface SearchInputProps {
    id: number,
    world: number
    onClick: (id: number, world: number) => void;
}

function onClick(id: number, world: number) {
// search!
}

const SearchInputComponent: React.FC<SearchInputProps> = ({id, world, onClick}) => {
    return (
        <div className="search-container">
            <div className="search-inputs">
                <p>Id: {id}</p>
                <p>World: {world}</p>
            </div>
            <div className="search-button">
                <button onClick={() => onClick(id, world)}>
                    Search
                </button>
            </div>
        </div>
    );
};

export default SearchInputComponent;