import React from "react";
import Typography from "@mui/material/Typography";
import {Item} from '../../../types/types';
import {IDataProps} from "./IDataProps";
import BoxedCardComponent from "../BoxedCardComponent";
import '../../../styles/ItemComponent.css';
import '../../../styles/BoxedCardComponent.css';

interface ItemProps extends IDataProps {
    item: Item | null | undefined;
}

const ItemComponent: React.FC<ItemProps> = ({item}) => {
    return <div><BoxedCardComponent children={itemContent(item)}/></div>;
};

const itemContent = (item: Item | null | undefined) =>
    <Typography className="MuiTypography-root">
        <Typography className="MuiTypography-name-and-id">
            <span>{item?.name ?? 'Name missing'}</span><br/>
            <span>Id {item?.id}</span>
        </Typography>
        <Typography className="MuiTypography-description">
            {item?.description ?? 'Description missing'}
        </Typography>
        <Typography className="MuiTypography-details">
            <span>Level: {item?.level}</span><br/>
            <span>Icon Id: {item?.iconId}</span><br/>
            <span>Stack size: {item?.stackSize}</span><br/>
            <span>PriceMid: {item?.priceMid}</span><br/>
            <span>PriceLow: {item?.priceLow}</span><br/>
            <span>CanHq: {item?.canHq ? 'Yes' : 'No'}</span><br/>
        </Typography>
    </Typography>;

export default ItemComponent;
