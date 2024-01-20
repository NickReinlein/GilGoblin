import React from 'react';
import {render, screen} from '@testing-library/react';
import '@testing-library/jest-dom';
import TimestampToAgeConverter from './TimestampToAgeConverter';

describe('TimestampToAgeConverter', () => {
    test('renders age indicator correctly for less than a minute', () => {
        const now = new Date();
        now.setSeconds(now.getSeconds() - 30); // 30 seconds ago
        let timestamp = now.toLocaleString();

        render(<TimestampToAgeConverter timestamp={timestamp}/>);

        expect(screen.getByText(/seconds ago/)).toBeInTheDocument();
    });

    test('renders age indicator correctly for less than an hour', () => {
        const now = new Date();
        now.setMinutes(now.getMinutes() - 30); // 30 minutes ago
        let timestamp = now.toLocaleString();

        render(<TimestampToAgeConverter timestamp={timestamp}/>);

        expect(screen.getByText(/minutes ago/)).toBeInTheDocument();
    });

    test('renders age indicator correctly for less than a day', () => {
        const now = new Date();
        now.setHours(now.getHours() - 6); // 6 hours ago
        let timestamp = now.toLocaleString();

        render(<TimestampToAgeConverter timestamp={timestamp}/>);

        expect(screen.getByText(/hours ago/)).toBeInTheDocument();
    });

    test('renders age indicator correctly for more than a day', () => {
        const now = new Date();
        now.setDate(now.getDate() - 2); // 2 days ago
        let timestamp = now.toLocaleString();

        render(<TimestampToAgeConverter timestamp={timestamp}/>);

        expect(screen.getByText(/days ago/)).toBeInTheDocument();
    });

    test('renders original timestamp', () => {
        const now = new Date();
        const timestamp = now.toISOString();

        render(<TimestampToAgeConverter timestamp={timestamp}/>);

        expect(screen.getByText(`Original Timestamp: ${timestamp}`)).toBeInTheDocument();
    });
});
