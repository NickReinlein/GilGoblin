import React from 'react';
import { render, screen } from '@testing-library/react';
import App from '../src/App';

test('renders TabComponent', () => {
  render(<App />);
  const linkElement = screen.getByText(/Recipes/i);
  expect(linkElement).toBeInTheDocument();
});

