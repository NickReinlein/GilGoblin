import React from "react";
import Typography from "@mui/material/Typography/Typography";
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
        <Typography className="MuiTypography-name">
            {item?.name ?? 'Name missing'} (Id {item?.id})
        </Typography>
        <Typography className="MuiTypography-description">
            {item?.description ?? 'Description missing'}<br/>
        </Typography>
        <Typography className="MuiTypography-details">
            <span>Level:</span> {item?.level}<br/>
            <span>IconId:</span> {item?.iconId}<br/>
            <span>StackSize:</span> {item?.stackSize}<br/>
            <span>PriceMid:</span> {item?.priceMid}<br/>
            <span>PriceLow:</span> {item?.priceLow}<br/>
            <span>CanHq:</span> {item?.canHq ? 'Yes' : 'No'}<br/>
        </Typography>
    </Typography>;

export default ItemComponent;
