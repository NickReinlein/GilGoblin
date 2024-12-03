import React, {useEffect, useState} from 'react';
import {World} from '../../types/types';
import DataFetcher from '../DataFetcher';

interface WorldSearchInputProps {
    world: number | null;
    onWorldChange?: (newId: number) => void;
}

const WorldSearchInputComponent: React.FC<WorldSearchInputProps> = ({world, onWorldChange}) => {
    const [worlds, setWorlds] = useState<World[]>([]);

    const fetchWorlds = () => {
        try {
            DataFetcher.fetchData('World', null, null)
                .then((fetchedWorlds) => {
                    setWorlds(fetchedWorlds);
                });
        } catch (error) {
            console.error('Error fetching worlds:', error);
        }
    };

    useEffect(() => {
        fetchWorlds();
    }, []);

    return (
        <div>
            <label className="search-label" htmlFor="worldInput"> World </label>
            <select
                className="search-dropdown"
                aria-label="World Selector"
                id="worldInput"
                value={world === null ? '' : world}
                onChange={(e) => {
                    if (onWorldChange)
                        onWorldChange(Number(e.target.value));
                }}>
                {worlds?.map((w) => (
                    <option key={w.id} value={w.id}>
                        {`${w.id}:${w.name}`}
                    </option>
                ))}
            </select>
        </div>
    );
};

export default WorldSearchInputComponent;