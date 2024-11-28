import React from 'react';
import {render, screen} from '@testing-library/react';
import PriceComponent, {formatDate} from './PriceComponent';
import {Price} from '../../../types/types';

describe('PriceComponent', () => {

    it('renders all fields correctly', () => {
        render(<PriceComponent price={mockPriceData}/>);

        expect(screen.getByText('Item Id: 1639')).toBeInTheDocument();
        expect(screen.getByText('World: 34')).toBeInTheDocument();
        expect(screen.getByText('IsHq: Yes')).toBeInTheDocument();

        expect(screen.getByText('Average Sale Price: Price: 225.5, Type: average')).toBeInTheDocument();
        expect(screen.getByText('Min Listing: Price: 210, Type: min')).toBeInTheDocument();
        expect(screen.getByText('Recent Purchase: Price: 230, Type: recent')).toBeInTheDocument();
        expect(screen.getByText('Daily Sale Velocity: World: 10, DC: 15, Region: 25')).toBeInTheDocument();

        const formattedDate = formatDate(new Date(mockPriceData.updated));
        expect(screen.getByText('Last Updated:')).toBeInTheDocument();
        expect(screen.getByText(formattedDate)).toBeInTheDocument();
    });

    it('renders missing fields gracefully', () => {
        const incompletePriceData: Price = {
            ...mockPriceData,
            averageSalePrice: null,
            minListing: null,
            recentPurchase: null,
            dailySaleVelocity: null,
        };

        render(<PriceComponent price={incompletePriceData}/>);

        expect(screen.getByText('Item Id: 1639')).toBeInTheDocument();
        expect(screen.getByText('World: 34')).toBeInTheDocument();
        expect(screen.getByText('IsHq: Yes')).toBeInTheDocument();

        expect(screen.getByText('Average Sale Price: N/A')).toBeInTheDocument();
        expect(screen.getByText('Min Listing: N/A')).toBeInTheDocument();
        expect(screen.getByText('Recent Purchase: N/A')).toBeInTheDocument();
        expect(screen.getByText('Daily Sale Velocity: N/A')).toBeInTheDocument();

        const formattedDate = formatDate(new Date(incompletePriceData.updated));
        expect(screen.getByText('Last Updated:')).toBeInTheDocument();
        expect(screen.getByText(formattedDate)).toBeInTheDocument();
    });

    it('renders the formatted date correctly', () => {
        const formattedDate = formatDate(new Date(mockPriceData.updated));
        render(<PriceComponent price={mockPriceData}/>);

        expect(screen.getByText('Last Updated:')).toBeInTheDocument();
        expect(screen.getByText(formattedDate)).toBeInTheDocument();
    });

    const mockPriceData: Price = {
        id: 1,
        worldId: 34,
        itemId: 1639,
        isHq: true,
        updated: '2024-11-23T22:15:35.521899+00:00',
        averageSalePrice: {
            id: 100,
            itemId: 1639,
            worldId: 34,
            isHq: true,
            worldDataPoint: {
                id: 200,
                priceType: 'average',
                price: 225.5,
                worldId: 34,
                timestamp: 1700000001,
            },
            dcDataPoint: null,
            regionDataPoint: null,
        },
        minListing: {
            id: 101,
            itemId: 1639,
            worldId: 34,
            isHq: false,
            worldDataPoint: {
                id: 201,
                priceType: 'min',
                price: 210.0,
                worldId: 34,
                timestamp: 1700000002,
            },
            dcDataPoint: null,
            regionDataPoint: null,
        },
        recentPurchase: {
            id: 102,
            itemId: 1639,
            worldId: 34,
            isHq: true,
            worldDataPoint: {
                id: 202,
                priceType: 'recent',
                price: 230.0,
                worldId: 34,
                timestamp: 1700000003,
            },
            dcDataPoint: null,
            regionDataPoint: null,
        },
        dailySaleVelocity: {
            id: 103,
            itemId: 1639,
            worldId: 34,
            isHq: true,
            world: 10,
            dc: 15,
            region: 25,
        },
    };
});
