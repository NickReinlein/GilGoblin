import React, {useEffect, useState} from "react";
import '../../styles/SearchInputs.css';
import {World} from "../../types/types";
import DataFetcher from "../DataFetcher";

interface SearchInputsComponentProps {
    id: number,
    world: number
    onIdChange?: (value: number) => void;
    onWorldChange?: (value: number) => void;
}

const SearchInputsComponent: React.FC<SearchInputsComponentProps> = ({id, world, onIdChange, onWorldChange}) => {
    const [worlds, setWorlds] = useState<World[]>([]);

    useEffect(() => {
        const fetchWorlds = async () => {
            try {
                const fetchedWorlds = await DataFetcher.fetchData('World', null, null);
                setWorlds(fetchedWorlds);
            } catch (error) {
                console.error("Error fetching worlds:", error);
            }
        };

        fetchWorlds();
    }, []);


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
                    {worlds.map((w) => (
                        <option key={w.id} value={w.id}>{`${w.id}:${w.name}`}</option>
                    ))}
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