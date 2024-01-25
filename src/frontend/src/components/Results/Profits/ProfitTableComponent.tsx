import React from 'react';
import {Crafts} from '../../../types/types';
import ProfitComponent from './ProfitComponent';
import {convertMultipleCraftsToProfits} from '../../../converters/CraftToProfitConverter';
import '../../../styles/ProfitTableComponent.css';
import ProfitTableHeaderComponent from "./ProfitTableHeaderComponent";

interface ProfitTableProps {
    crafts: Crafts,
    columnSort?: string,
    ascending?: boolean
}

const ProfitTableComponent: React.FC<ProfitTableProps> = ({crafts}) => {
    if (crafts === undefined || crafts === null || crafts.length === 0)
        return (<div>Press the search button to search for a World's best recipes to craft</div>);

    return (
        <div className="profits-table">
            <table>
                <ProfitTableHeaderComponent/>
                <tbody>
                {
                    convertMultipleCraftsToProfits(crafts)
                        .sortColumns((profits) => ())
                        .map((profit, index) => (
                            <tr key={index}>
                                <ProfitComponent profit={profit} index={index}/>
                            </tr>
                        ))
                }
                </tbody>
            </table>
        </div>
    );
};

export default ProfitTableComponent;