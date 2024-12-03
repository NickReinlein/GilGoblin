import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import TabComponent, {buttonTitles} from './TabComponent';
import DataFetcher from '../DataFetcher';

jest.mock('../DataFetcher', () => ({
    fetchData: jest.fn(),
}));

describe('TabComponent', () => {

    beforeAll(() => {
        console.error = jest.fn();
        console.log = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    test('renders tabs with correct names', () => {
        render(<TabComponent/>);

        buttonTitles.forEach((tabName) => {
            expect(screen.getByText(tabName)).toBeInTheDocument();
        });
    });

    test('sets active tab on tab click', () => {
        render(<TabComponent/>);
        const tabToClick = buttonTitles[1];
        expect(screen.getByText(tabToClick)).not.toHaveClass('tab-active');

        fireEvent.click(screen.getByText(tabToClick));

        expect(screen.getByText(tabToClick)).toHaveClass('tab-active');
    });

    test('fetches and displays data on search click', async () => {
        render(<TabComponent/>);
        const mockData = {example: 'data'};
        (DataFetcher.fetchData as jest.Mock).mockResolvedValue(mockData);

        fireEvent.click(screen.getByText(buttonTitles[2]));
        fireEvent.click(screen.getByText('Search'));

        expect(DataFetcher.fetchData).toHaveBeenCalledWith(buttonTitles[2], 2855, 21);
        expect(screen.getByText(buttonTitles[2])).toHaveClass('tab-active');
    });

    test('handles error during data fetch', async () => {
        render(<TabComponent/>);
        (DataFetcher.fetchData as jest.Mock).mockRejectedValue(null);


        fireEvent.click(screen.getByText(buttonTitles[3]));
        fireEvent.click(screen.getByText('Search'));

        expect(DataFetcher.fetchData).toHaveBeenCalledWith(buttonTitles[3], 2855, 21);
        expect(screen.getByText('Search')).toBeDefined();
    });
});
