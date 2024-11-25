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
    const month = String(date.getMonth() + 1).padStart(2, '0');
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
            <span>Average Sale Price: {price?.averageSalePrice}</span><br/>
            <span>Min Listing: {price?.minListing}</span><br/>
            <span>Recent Purchase: {price?.recentPurchase}</span><br/>
            <span>Daily Sale Velocity: {price?.dailySaleVelocity}</span><br/>
            <span>Last Uploaded: <br/>
                <span>
                    {price?.lastUploadTime !== undefined
                        ? formatDate(new Date(price?.lastUploadTime))
                        : <i>Missing last upload timestamp</i>}
                </span>
            </span>
        </Typography>
    </Typography>


// const priceContent = (price: Price | null | undefined) => (
//     <div className="price-content">
//         <Typography className="MuiTypography-root">
//             <div>
//                 <div className="MuiTypography-ids">
//                     <div>Item Id: {price?.itemId}</div>
//                     <div>World: {price?.worldId}</div>
//                     <div>IsHq: {price?.isHq ? 'Yes' : 'No'}</div>
//                 </div>
//                 <div>
//                     <div className="MuiTypography-details">
//                         <div>Average Sale Price: {price?.averageSalePrice}</div>
//                         <div>Min Listing: {price?.minListing}</div>
//                         <div>Recent Purchase: {price?.recentPurchase}</div>
//                         <div>Daily Sale Velocity: {price?.dailySaleVelocity}</div>
//                         <div>
//                             Last Updated:
//                             {price?.lastUploadTime !== undefined ? (
//                                 <span>{formatDate(new Date(price.lastUploadTime))}</span>
//                             ) : (
//                                 <i>Missing last upload timestamp</i>
//                             )}
//                         </div>
//                     </div>
//                     </div>
//                 </div>
//         </Typography>
//     </div>
// );


export default PriceComponent;