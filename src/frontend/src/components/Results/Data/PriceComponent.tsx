import React from 'react';
import Typography from '@mui/material/Typography';
import {DailySaleVelocityPoco, Price, PriceDataPointPoco} from '../../../types/types';
import BoxedCardComponent from '../BoxedCardComponent';
import '../../../styles/PriceComponent.css';
import '../../../styles/BoxedCardComponent.css';

interface PriceProps {
    price: Price;
}

const PriceComponent: React.FC<PriceProps> = ({price}) => {
    return <BoxedCardComponent>{priceContent(price)}</BoxedCardComponent>;
};

const priceContent = (price: Price | null | undefined) => (
    <div>
        <Typography className="MuiTypography-root">
            <Typography className="MuiTypography-ids">
                <span>Item Id: {price?.itemId}</span> <br/>
                <span>World: {price?.worldId}</span> <br/>
                <span>IsHq: {price?.isHq ? 'Yes' : 'No'}</span> <br/>
            </Typography>
            <Typography className="MuiTypography-price_details">
                <span>Average Sale Price: {formatPriceData(price?.averageSalePrice)}</span> <br/>
                <span>Min Listing: {formatPriceData(price?.minListing)}</span> <br/>
                <span>Recent Purchase: {formatPriceData(price?.recentPurchase)}</span> <br/>
                <span>Daily Sale Velocity: {formatDailySaleVelocity(price?.dailySaleVelocity)}</span> <br/>
                <span>Last Updated:</span> <br/>
                {price?.updated ? (
                    <span>{formatDate(new Date(price.updated))}</span>
                ) : (
                    <i>Missing last upload timestamp</i>
                )}
                <br/>
            </Typography>
        </Typography>
    </div>
);

function formatPriceData(data: PriceDataPointPoco | null | undefined): string {
    if (!data) return 'N/A';
    return `Price: ${data.worldDataPoint?.price ?? 'Unknown'}, Type: ${data.worldDataPoint?.priceType ?? 'Unknown'}`;
}

function formatDailySaleVelocity(data: DailySaleVelocityPoco | null | undefined): string {
    if (!data) return 'N/A';
    return `World: ${data.world}, DC: ${data.dc}, Region: ${data.region}`;
}

export function formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

export default PriceComponent;