import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import SearchInputsComponent from './SearchInputsComponent';

describe('SearchInputsComponent', () => {
    const id = 1;
    const world = 34;

    it('renders correctly and calls onIdChange and onWorldChange', () => {
        const onIdChangeMock = jest.fn();
        const onWorldChangeMock = jest.fn();

        render(
            <SearchInputsComponent id={id} world={world} onIdChange={onIdChangeMock} onWorldChange={onWorldChangeMock}/>
        );

        const worldInput = screen.getByLabelText('World') as HTMLSelectElement;
        const idInput = screen.getByLabelText('Id') as HTMLInputElement;

        expect(worldInput).toBeInTheDocument();
        expect(idInput).toBeInTheDocument();

        expect(worldInput.value).toBe('34');
        expect(idInput.value).toBe('1');

        fireEvent.change(worldInput, {target: {value: '1'}});
        fireEvent.change(idInput, {target: {value: '2'}});

        expect(onWorldChangeMock).toHaveBeenCalledWith(1);
        expect(onIdChangeMock).toHaveBeenCalledWith(2);
    });

    it('renders correctly with default values and does not call onIdChange and onWorldChange', () => {
        const onIdChangeMock = jest.fn();
        const onWorldChangeMock = jest.fn();

        render(
            <SearchInputsComponent id={id} world={world}/>
        );

        const worldInput = screen.getByLabelText('World') as HTMLSelectElement;
        const idInput = screen.getByLabelText('Id') as HTMLInputElement;

        expect(worldInput).toBeInTheDocument();
        expect(idInput).toBeInTheDocument();

        expect(worldInput.value).toBe('34');
        expect(idInput.value).toBe('1');

        fireEvent.change(worldInput, {target: {value: '1'}});
        fireEvent.change(idInput, {target: {value: '2'}});

        expect(onWorldChangeMock).not.toHaveBeenCalled();
        expect(onIdChangeMock).not.toHaveBeenCalled();
    });
});
