import React from 'react';
import { render, screen } from '@testing-library/react';
import App from '../src/App';
import TabComponent from '../src/components/TabComponent';

test('renders TabComponent', () => {
  render(<App />);
  const linkElement = screen.getByText(/Recipes/i);
  expect(linkElement).toBeInTheDocument();
});

