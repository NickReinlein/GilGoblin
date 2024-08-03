import React from 'react';
import Typography from "@mui/material/Typography/Typography";

interface IngredientProps {
    qty: number | null | undefined;
    id: number | null | undefined;
}

const IngredientComponent: React.FC<IngredientProps> = ({qty, id}) => {
    return qty ? <Typography>{qty} x {id}</Typography> : null;
};

export default IngredientComponent;
