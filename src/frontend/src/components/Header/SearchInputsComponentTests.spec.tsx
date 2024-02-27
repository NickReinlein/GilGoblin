import React from 'react';
import {fireEvent, render, screen, waitFor} from '@testing-library/react';
import SearchInputsComponent from './SearchInputsComponent';
import DataFetcher from '../DataFetcher';

jest.mock('../DataFetcher', () => ({
    fetchData: jest.fn(),
}));

const mockedWorlds = [
    {id: 1, name: 'Dagobah'},
    {id: 2, name: 'Tattooine'},
];

test('renders SearchInputsComponent', async () => {
    (DataFetcher.fetchData as jest.Mock).mockResolvedValueOnce(mockedWorlds);

    render(<SearchInputsComponent id={1} world={1}/>);

    expect(screen.getByText('1:World 1')).toBeInTheDocument();
    expect(screen.getByText('2:World 2')).toBeInTheDocument();
    expect(screen.getByLabelText('Id')).toBeInTheDocument();
});

// Test user interaction
test('calls onWorldChange when world dropdown value changes', async () => {
    (DataFetcher.fetchData as jest.Mock).mockResolvedValueOnce(mockedWorlds);
    const onWorldChangeMock = jest.fn();
    render(<SearchInputsComponent id={1} world={1} onWorldChange={onWorldChangeMock}/>);

    fireEvent.change(screen.getByLabelText('World'), {target: {value: '2'}});

    expect(onWorldChangeMock).toHaveBeenCalledWith(2);
});

test('displays error message when API call fails', async () => {
    (DataFetcher.fetchData as jest.Mock).mockRejectedValueOnce(new Error('API call failed'));

    render(<SearchInputsComponent id={1} world={1}/>);

    await waitFor(() => {
        expect(screen.getByText('Error fetching worlds:')).toBeInTheDocument();
    });
});
