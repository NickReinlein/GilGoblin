import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import TabComponent, {tabNames} from './TabComponent';
import DataFetcher from '../DataFetcher';

jest.mock('../DataFetcher', () => ({
    fetchData: jest.fn(),
}));

describe('TabComponent', () => {
    afterEach(() => {
        jest.clearAllMocks();
    });

    test('renders tabs with correct names', () => {
        render(<TabComponent/>);

        tabNames.forEach((tabName) => {
            expect(screen.getByText(tabName)).toBeInTheDocument();
        });
    });

    test('sets active tab on tab click', () => {
        render(<TabComponent/>);
        const tabToClick = tabNames[1];
        expect(screen.getByText(tabToClick)).not.toHaveClass('active');

        fireEvent.click(screen.getByText(tabToClick));

        expect(screen.getByText(tabToClick)).toHaveClass('active');
    });

    test('fetches and displays data on search click', async () => {
        render(<TabComponent/>);
        const mockData = {example: 'data'};
        (DataFetcher.fetchData as jest.Mock).mockResolvedValue(mockData);

        fireEvent.click(screen.getByText(tabNames[2]));
        fireEvent.click(screen.getByText('Search'));

        expect(DataFetcher.fetchData).toHaveBeenCalledWith(tabNames[2], 1639, 34);
        expect(screen.getByText(tabNames[2])).toHaveClass('active');
    });

    test('handles error during data fetch', async () => {
        render(<TabComponent/>);
        (DataFetcher.fetchData as jest.Mock).mockRejectedValue(null);


        fireEvent.click(screen.getByText(tabNames[3]));
        fireEvent.click(screen.getByText('Search'));

        expect(DataFetcher.fetchData).toHaveBeenCalledWith(tabNames[3], 1639, 34);
        expect(screen.getByText('Search')).toBeDefined();
    });
});
