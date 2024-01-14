import React from 'react';
import {render, screen} from '@testing-library/react';
import App from './App';

describe('App component', () => {
    it('renders without crashing', () => {
        render(<App/>);

        const appDivElement = screen.getByTestId("app-tab");

        expect(appDivElement).toBeInTheDocument();
    });
});