import React from 'react';
import '../styles/AboutPageComponent.css';

const AboutPageComponent = () => {
    return (
        <div className="about-page">
            <h1>About GilGoblin</h1>
            <p>
                <code>GilGoblin</code> is a project designed to calculate in-game crafting profitability in
                <a href="https://www.finalfantasyxiv.com/"> Final Fantasy XIV (FFXIV)</a>, and display a result for
                users sorted by profitability.
            </p>
            <p>
                It determines the most profitable items to craft based on current market prices, vendor prices, and
                crafting component costs.
            </p>
            <p>
                Recent prices are stored in a local PostgresSQL database, which is refreshed by a background service.
                This keeps profits relevant with up-to-date data.
            </p>
            <p>For more information, please visit the project's <a href="https://github.com/NickReinlein/GilGoblin">GitHub
                repository</a>.
            </p>
            <h1>About the author</h1>
            <p>
                Nick Reinlein is a software developer with 9 years experience, currently working from Canada.
                Nick has a passion for web development and programming.
                He has a dual background in Computer Science and Financial Accounting.
            </p>
            <p>
                He is married and father of 2 children. Trying to balance life, work and leisure is a perpetual
                struggle, but he is determined to try.
            </p>
            <h2> Contact </h2>
            <a href="mailto:gilgoblinproject@gmail.com">gilgoblinproject@gmail.com</a>
        </div>
    )
        ;
};

export default AboutPageComponent;
