import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import SearchComponent from './SearchComponent';

jest.mock('../DataFetcher', () => ({
    fetchData: async () => [
        {id: 1, name: 'World 1'},
        {id: 2, name: 'World 2'},
        {id: 3, name: 'World 3'}
    ]
}));

describe('SearchComponent', () => {
    const onClickMock = jest.fn();

    it('renders correctly, with all worlds selectable', async () => {
        render(<SearchComponent onClick={onClickMock}/>);
        await screen.findByText('1:World 1');

        expect(screen.getByLabelText('Id')).toBeInTheDocument();
        expect(screen.getByLabelText('World')).toBeInTheDocument();
        expect(screen.getByText('Search')).toBeInTheDocument();
        expect(screen.getByText('1:World 1')).toBeInTheDocument();
        expect(screen.getByText('2:World 2')).toBeInTheDocument();
        expect(screen.getByText('3:World 3')).toBeInTheDocument();
    });

    it('calls onClick handler with default arguments when Search button is clicked', async () => {
        render(<SearchComponent onClick={onClickMock}/>);
        await screen.findByText('1:World 1');
        const searchButton = screen.getByText('Search');

        fireEvent.click(searchButton);

        expect(onClickMock).toHaveBeenCalledWith(2855, 21);
    });

    it('calls onClick handler with selected values when Search button is clicked', async () => {
        render(<SearchComponent onClick={onClickMock}/>);
        await screen.findByText('1:World 1');
        const idInput = screen.getByLabelText('Id');
        const worldSelect = screen.getByLabelText('World');
        const searchButton = screen.getByText('Search');

        fireEvent.change(idInput, {target: {value: '123'}});
        fireEvent.change(worldSelect, {target: {value: '2'}});
        fireEvent.click(searchButton);

        expect(onClickMock).toHaveBeenCalledWith(123, 2);
    });
});
