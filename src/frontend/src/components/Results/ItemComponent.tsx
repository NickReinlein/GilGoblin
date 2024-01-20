import React from "react";
import {Item} from '../../types/types';

interface ItemProps {
    item: Item;
}

const ItemComponent: React.FC<ItemProps> = ({item}) => {
    return (
        <div>
            <p>Item Id: {item?.id}</p>
            <p>Name: {item?.name}</p>
            <p>Description: {item?.description}</p>
            <p>Level: {item?.level}</p>
            <p>IconId: {item?.iconId}</p>
            <p>StackSize: {item?.stackSize}</p>
            <p>PriceMid: {item?.priceMid}</p>
            <p>PriceLow: {item?.priceLow}</p>
            <p>CanHq: {item?.canHq ? `true` : `false`}</p>
        </div>
    );
};

export default ItemComponent;
