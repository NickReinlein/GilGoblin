import React, {useState} from 'react';
import SearchInputsComponent from "./SearchInputsComponent";
import '../../styles/Search.css';

interface SearchComponentProps {
    onClick: (id: number, world: number) => void;
}

const SearchComponent: React.FC<SearchComponentProps> = ({onClick}) => {
    const [id, setId] = useState(2855);
    const [world, setWorld] = useState(21);

    const handleIdChange = (newId: number) => {
        setId(newId);
    };

    const handleWorldChange = (newWorld: number) => {
        setWorld(newWorld);
    };

    function handleClick() {
        onClick(id, world);
    }

    return (
        <div className="search-container">
            <SearchInputsComponent id={id}
                                   world={world}
                                   onIdChange={handleIdChange}
                                   onWorldChange={handleWorldChange}
            />
            <button className="search-button" onClick={() => handleClick()}>
                Search
            </button>
        </div>
    );
};

export default SearchComponent;