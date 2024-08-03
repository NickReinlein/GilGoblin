import {Box, Card, CardContent} from "@mui/material";
import {IDataProps} from "./Data/IDataProps";
import React from "react";
import '../../styles/BoxedCardComponent.css';

const BoxedCardComponent: React.FC<IDataProps> = ({children}) => {
    return <Box>
        <Card raised={true} className="MuiCard-root">
            <CardContent className="MuiCardContent-root">
                {children}
            </CardContent>
        </Card>
    </Box>;
};

export default BoxedCardComponent;