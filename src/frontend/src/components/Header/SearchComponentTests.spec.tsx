import React from 'react';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import SearchComponent from './SearchComponent';

describe('SearchComponent', () => {
    const onClickMock = jest.fn();

    it('renders correctly', () => {
        render(
            <SearchComponent onClick={onClickMock}/>
        );

        const searchButton = screen.getByText('Search');
        const worldInput = screen.getByLabelText('World');
        const idInput = screen.getByLabelText('Id');

        expect(searchButton).toBeInTheDocument();
        expect(worldInput).toBeInTheDocument();
        expect(idInput).toBeInTheDocument();
    });

    it('calls onClick with updated id value on button click', () => {
        render(
            <SearchComponent onClick={onClickMock}/>
        );
        const searchButton = screen.getByText('Search');
        const idInput = screen.getByLabelText('Id');

        userEvent.type(idInput, '999');
        userEvent.click(searchButton);

        expect(onClickMock).toHaveBeenCalledWith(999, 0);
    });

    it('calls onClick with updated world value on button click', () => {
        render(
            <SearchComponent onClick={onClickMock}/>
        );
        const searchButton = screen.getByText('Search');
        const worldInput = screen.getByLabelText('World');

        userEvent.selectOptions(worldInput, '22');
        userEvent.click(searchButton);

        expect(onClickMock).toHaveBeenCalledWith(0, 22);
    });
});
