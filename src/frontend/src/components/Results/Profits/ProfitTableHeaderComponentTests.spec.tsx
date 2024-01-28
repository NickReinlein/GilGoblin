import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import ProfitTableHeaderComponent from './ProfitTableHeaderComponent';

describe('ProfitTableHeaderComponent', () => {
    const headersMock = ['#', 'Name', 'Sold Profit', 'Listings Profit', 'Avg. Sold', 'Avg. Listing', 'Cost', 'Qty', 'Age'];
    const onHeaderClickMock = jest.fn();

    test('renders all the headers', () => {
        render(<ProfitTableHeaderComponent headers={headersMock} onHeaderClick={onHeaderClickMock} />);

        expect(screen.getByText('#')).toBeInTheDocument();
        expect(screen.getByText('Name')).toBeInTheDocument();
        expect(screen.getByText('Sold Profit')).toBeInTheDocument();
        expect(screen.getByText('Listings Profit')).toBeInTheDocument();
        expect(screen.getByText('Avg. Sold')).toBeInTheDocument();
        expect(screen.getByText('Avg. Listing')).toBeInTheDocument();
        expect(screen.getByText('Cost')).toBeInTheDocument();
        expect(screen.getByText('Qty')).toBeInTheDocument();
        expect(screen.getByText('Age')).toBeInTheDocument();
    });

    test('fires the onHeaderClick callback when a header is clicked', () => {
        render(<ProfitTableHeaderComponent headers={headersMock} onHeaderClick={onHeaderClickMock}/>);

        fireEvent.click(screen.getByText('Name'));

        expect(onHeaderClickMock).toHaveBeenCalledWith('Name');
    });
});
