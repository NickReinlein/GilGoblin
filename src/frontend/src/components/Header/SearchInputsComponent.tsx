import React from "react";
import '../../styles/SearchInputs.css';

interface SearchInputsComponentProps {
    id: number,
    world: number
    onIdChange?: (value: number) => void;
    onWorldChange?: (value: number) => void;
}

const SearchInputsComponent: React.FC<SearchInputsComponentProps> = ({id, world, onIdChange, onWorldChange}) => {
    return (
        <div className="search-inputs">
            <div>
                <label className="search-label" htmlFor="worldInput">World</label>
                <select
                    className="search-dropdown"
                    id="worldInput"
                    value={world}
                    onChange={(e) => {
                        if (onWorldChange)
                            onWorldChange(Number(e.target.value))
                    }}>
                    <option value="1">1</option>
                    <option value="34">34</option>
                </select>
            </div>
            <div>
                <label className="search-label" htmlFor="idInput">Id</label>
                <input
                    className="search-input"
                    type="number"
                    id="idInput"
                    value={id}
                    onChange={(e) => {
                        if (onIdChange)
                            onIdChange(Number(e.target.value))
                    }}
                />
            </div>
        </div>
    );
};

export default SearchInputsComponent;