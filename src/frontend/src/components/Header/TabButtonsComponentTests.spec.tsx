import React from 'react';
import { render, fireEvent, screen } from '@testing-library/react';
import TabButtonsComponent from './TabButtonsComponent';

describe('TabButtonsComponent', () => {
    const tabName = 'Tab 1';
    const activeTab = 'Tab 1';
    const onClickMock = jest.fn();

    test('renders TabButtonsComponent with correct class when active', () => {
        render(
            <TabButtonsComponent tabName={tabName} activeTab={activeTab} onClick={onClickMock}>
                {tabName}
            </TabButtonsComponent>
        );

        const button = screen.getByText(tabName);
        expect(button).toBeInTheDocument();
        expect(button).toHaveClass('active');
    });

    test('renders TabButtonsComponent without active class when not active', () => {
        render(
            <TabButtonsComponent tabName={tabName} activeTab="Tab 2" onClick={onClickMock}>
                {tabName}
            </TabButtonsComponent>
        );

        const button = screen.getByText(tabName);
        expect(button).toBeInTheDocument();
        expect(button).not.toHaveClass('active');
    });

    test('calls onClick function with tabName when clicked', () => {
        render(
            <TabButtonsComponent tabName={tabName} activeTab={activeTab} onClick={onClickMock}>
                {tabName}
            </TabButtonsComponent>
        );

        const button = screen.getByText(tabName);
        fireEvent.click(button);
        expect(onClickMock).toHaveBeenCalledWith(tabName);
    });
});
