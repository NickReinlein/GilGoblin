import React from 'react';
import { render, fireEvent, screen } from '@testing-library/react';
import IdSearchInputComponent from './IdSearchInputComponent';

describe('IdSearchInputComponent', () => {
    it('renders correctly with provided ID', () => {
        const id = 123;
        const onIdChangeMock = jest.fn();
        render(<IdSearchInputComponent id={id} onIdChange={onIdChangeMock} />);

        expect(screen.getByLabelText('Id')).toBeInTheDocument();
        expect(screen.getByDisplayValue(id.toString())).toBeInTheDocument();

        const newValue = '456';
        fireEvent.change(screen.getByDisplayValue(id.toString()), { target: { value: newValue } });

        expect(onIdChangeMock).toHaveBeenCalledWith(Number(newValue));
    });

    it('renders correctly with null ID', () => {
        const onIdChangeMock = jest.fn();
        render(<IdSearchInputComponent id={null} onIdChange={onIdChangeMock} />);

        expect(screen.getByLabelText('Id')).toBeInTheDocument();
        expect(screen.getByDisplayValue('')).toBeInTheDocument();

        const newValue = '456';
        fireEvent.change(screen.getByDisplayValue(''), { target: { value: newValue } });

        expect(onIdChangeMock).toHaveBeenCalledWith(Number(newValue));
    });

    it('handles null ID correctly', () => {
        const onIdChangeMock = jest.fn();
        render(<IdSearchInputComponent id={null} onIdChange={onIdChangeMock} />);

        const newValue = '456';
        fireEvent.change(screen.getByDisplayValue(''), { target: { value: newValue } });

        expect(onIdChangeMock).toHaveBeenCalledWith(Number(newValue));
    });
});
