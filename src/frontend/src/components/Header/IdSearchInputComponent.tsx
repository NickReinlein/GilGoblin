import React from 'react';

interface IdSearchInputProps {
    id: number | null;
    onIdChange?: (newId: number) => void;
}

const IdSearchInputComponent: React.FC<IdSearchInputProps> = ({ id, onIdChange }) => {
    return (
        <div>
            <label className="search-label" htmlFor="idInput">Id</label>
            <input
                className="search-input"
                type="number"
                id="idInput"
                value={id === null ? '' : id}
                onChange={(e) => {
                    if (onIdChange)
                        onIdChange(Number(e.target.value))
                }}
            />
        </div>
    );
};

export default IdSearchInputComponent;