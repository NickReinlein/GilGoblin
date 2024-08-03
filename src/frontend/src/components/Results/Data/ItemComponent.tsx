import React from "react";
import Typography from "@mui/material/Typography/Typography";
import {Item} from '../../../types/types';
import {IDataProps} from "./IDataProps";
import BoxedCardComponent from "../BoxedCardComponent";
import '../../../styles/ItemComponent.css';

interface ItemProps extends IDataProps {
    item: Item | null | undefined;
}

const ItemComponent: React.FC<ItemProps> = ({item}) => {
    return <div><BoxedCardComponent children={itemContent(item)}/></div>;
};

const itemContent = (item: Item | null | undefined) =>
    <Typography className="MuiTypography-root">
        <span>Item Id:</span> {item?.id}<br/>
        <span>Name:</span> {item?.name}<br/>
        <span>Description:</span> {item?.description}<br/>
        <span>Level:</span> {item?.level}<br/>
        <span>IconId:</span> {item?.iconId}<br/>
        <span>StackSize:</span> {item?.stackSize}<br/>
        <span>PriceMid:</span> {item?.priceMid}<br/>
        <span>PriceLow:</span> {item?.priceLow}<br/>
        <span>CanHq:</span> {item?.canHq ? 'Yes' : 'No'}<br/>
    </Typography>;

export default ItemComponent;
