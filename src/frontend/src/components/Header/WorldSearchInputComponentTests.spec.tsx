import React from 'react';
import {fireEvent, render, screen} from '@testing-library/react';
import '@testing-library/jest-dom';
import WorldSearchInputComponent from './WorldSearchInputComponent';
import DataFetcher from '../DataFetcher';

describe('WorldSearchInputComponent', () => {
    const worldIds = [1, 2, 3];

    beforeEach(() => {
        jest.clearAllMocks();

        DataFetcher.fetchData = async () => [
            {id: 1, name: 'World 1'},
            {id: 2, name: 'World 2'},
            {id: 3, name: 'World 3'}
        ];

        console.error = jest.fn();
    });

    it('renders without crashing', async () => {
        render(<WorldSearchInputComponent world={worldIds[0]}/>);

        expect(screen.getByLabelText('World')).toBeInTheDocument();
    });

    it('displays correct world options', async () => {
        render(<WorldSearchInputComponent world={worldIds[0]}/>);

        await screen.findByText('1:World 1');
        await screen.findByText('2:World 2');
        await screen.findByText('3:World 3');
    });

    test.each(worldIds)('calls onWorldChange when a different world is selected', async (worldValue) => {
        const mockOnWorldChange = jest.fn();
        render(<WorldSearchInputComponent world={0} onWorldChange={mockOnWorldChange}/>);
        await screen.findByText('1:World 1');
        const worldInput = screen.getByRole('combobox', {name: 'World'});

        fireEvent.change(worldInput, {target: {value: String(worldValue)}});

        expect(mockOnWorldChange).toHaveBeenCalledWith(worldValue);
    });

    it('handles error gracefully', async () => {
        DataFetcher.fetchData = jest.fn(() => {
            throw new Error('Error fetching worlds:')
        });

        render(<WorldSearchInputComponent world={0}/>);

        expect(DataFetcher.fetchData).toHaveBeenCalledWith('World', null, null);
        expect(console.error).toHaveBeenCalled();
    });

    it('handles the world id being null gracefully', async () => {
        expect(() => {
            render(<WorldSearchInputComponent world={null} />);
        }).not.toThrow();
    });

    test('invokes onWorldChange when a new option is selected', () => {
        const mockOnWorldChange = jest.fn();
        const worlds = [
            { id: 1, name: 'World One' },
            { id: 2, name: 'World Two' },
        ];

        render(
            <WorldSearchInputComponent
                world={null}
                worlds={worlds}
                onWorldChange={mockOnWorldChange}
            />
        );
        const dropdown = screen.getByLabelText('World');
        fireEvent.change(dropdown, { target: { value: '2' } });

        expect(mockOnWorldChange).toHaveBeenCalledTimes(1);
        expect(mockOnWorldChange).toHaveBeenCalledWith(2);
    });
});