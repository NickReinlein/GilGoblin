import "../../styles/TitleComponent.css";

const TitleComponent = () => {
    return (
        <div className="title">
            <h1 data-testid="title">GilGoblin</h1>
            <img src="assets/gilgoblin_icon_32x32.png" alt="GilGoblin Icon" className="icon"/>
        </div>
    );
};

export default TitleComponent;