import Typography from '@mui/material/Typography';
import React from 'react';
import {Price} from '../../../types/types';
import BoxedCardComponent from "../BoxedCardComponent";
import '../../../styles/PriceComponent.css';
import '../../../styles/BoxedCardComponent.css';

interface PriceProps {
    price: Price;
}

const PriceComponent: React.FC<PriceProps> = ({price}) => {
    return <div><BoxedCardComponent children={priceContent(price)}/></div>;
};

export function formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-based, so add 1const day = String(date.getDate()).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

const priceContent = (price: Price | null | undefined) =>
    <Typography className="MuiTypography-root">
        <Typography className="MuiTypography-ids">
            <span>Item Id: {price?.itemId}</span><br/>
            <span>World: {price?.worldId}</span>
        </Typography>
        <Typography className="MuiTypography-details">
            <span> Average Listing Price: {price?.averageListingPrice}</span><br/>
            <span>Average Listing Price NQ: {price?.averageListingPriceNQ}</span><br/>
            <span>Average Listing Price HQ: {price?.averageListingPriceHQ}</span><br/>
            <span>Average Sold: {price?.averageSold}</span><br/>
            <span>Average Sold NQ: {price?.averageSoldNQ}</span><br/>
            <span>Average Sold HQ: {price?.averageSoldHQ}</span><br/>
            <span>Last Uploaded: <br/>
                <span>
                    {price?.lastUploadTime !== undefined
                        ? formatDate(new Date(price?.lastUploadTime))
                        : <i>Missing last upload timestamp</i>}
                </span>
            </span>
        </Typography>
    </Typography>
;

export default PriceComponent;