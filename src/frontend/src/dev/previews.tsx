import {ComponentPreview, Previews} from "@react-buddy/ide-toolbox";
import {PaletteTree} from "./palette";
import ProfitComponent from "../components/Results/Profits/ProfitComponent";

const ComponentPreviews = () => {
    return (
        <Previews palette={<PaletteTree/>}>
            <ComponentPreview path="/ProfitComponent">
                <ProfitComponent/>
            </ComponentPreview>
        </Previews>
    );
};

export default ComponentPreviews;