import React from "react";
import '../../styles/SearchInputs.css';
import IdSearchInputComponent from "./IdSearchInputComponent";
import WorldSearchInputComponent from "./WorldSearchInputComponent";

interface SearchInputsComponentProps {
    id: number,
    world: number
    onIdChange?: (value: number) => void;
    onWorldChange?: (value: number) => void;
}

const SearchInputsComponent: React.FC<SearchInputsComponentProps> = ({id, world, onIdChange, onWorldChange}) => {
    return (
        <div className="search-inputs">
            <WorldSearchInputComponent world={world} onWorldChange={onWorldChange}/>
            <IdSearchInputComponent id={id} onIdChange={onIdChange}/>
        </div>
    );
};

export default SearchInputsComponent;