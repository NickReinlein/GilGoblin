import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import SearchComponent from './SearchComponent';

describe('SearchComponent', () => {
    it('renders correctly and calls onClick with updated values on button click', () => {
        const onClickMock = jest.fn();

        render(
            <SearchComponent onClick={onClickMock}/>
        );

        const searchButton = screen.getByText('Search') as HTMLButtonElement;
        const worldInput = screen.getByLabelText('World') as HTMLSelectElement;
        const idInput = screen.getByLabelText('Id') as HTMLInputElement;

        expect(searchButton).toBeInTheDocument();
        expect(worldInput).toBeInTheDocument();
        expect(idInput).toBeInTheDocument();

        fireEvent.change(worldInput, {target: {value: '1'}});
        fireEvent.change(idInput, {target: {value: '2'}});

        fireEvent.click(searchButton);

        expect(onClickMock).toHaveBeenCalledWith(2, 1);
    });
});