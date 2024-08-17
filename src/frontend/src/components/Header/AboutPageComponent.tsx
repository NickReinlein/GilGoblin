import React from 'react';
import '../../styles/AboutPageComponent.css';

const AboutPageComponent = () => {
    return (
        <div className="about-page">
            <div>
                <h1>GilGoblin</h1>
                <code>GilGoblin</code> is a project designed to calculate in-game crafting profitability in
                <a href="https://www.finalfantasyxiv.com/"> Final Fantasy XIV (FFXIV)</a>, and display a result for
                users sorted by profitability.<br/><br/>
                It determines the most profitable items to craft based on current market prices, vendor prices, and
                crafting component costs.<br/><br/>
                Recent prices are stored in a local PostgresSQL database, which is refreshed by a background service.
                This keeps profits relevant with up-to-date data.<br/><br/>
                For more information, please visit the project's <a href="https://github.com/NickReinlein/GilGoblin">GitHub
                repository</a>.<br/>

            </div>
            <div>
                <h2>About the author</h2>

                Nick Reinlein is a software developer from Montreal, Canada.
                Nick has a passion for web development and automation, especially when it involves complexity.
                He has a background in Computer Science and Financial Accounting. He is married and father of 3
                children. Nick is a lifelong fan of Final Fantasy and video games.<br/>
            </div>
            <div>
                <h2> Contact </h2>
                <a href="mailto:admin@gilgoblin.com">admin@gilgoblin.com</a>
            </div>
        </div>
    );
};

export default AboutPageComponent;
