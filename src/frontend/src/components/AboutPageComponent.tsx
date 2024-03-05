import React from 'react';

const AboutPage = () => {
    return (
        <div>
            <h1>GilGoblin</h1>
            <p>
                <code>GilGoblin</code> is a back-end REST API designed to calculate in-game crafting profitability in
                Final Fantasy XIV (FFXIV).
            </p>
            <p>
                It determines the most profitable items to craft based on current market prices, vendor prices, and
                crafting component costs.
            </p>
            <p>
                Recent prices are stored in a local PostgreSQL database, which is refreshed by a background service.
                This keeps profits relevant with up-to-date data.
            </p>
            <p>
                The long-term goal of GilGoblin is to create a website that utilizes this API's endpoints to display the
                top crafts for each crafting profession, specific to the user-selected world.
            </p>
            <p>For more information, please visit the project's <a href="https://github.com/NickReinlein/GilGoblin">GitHub
                repository</a>.</p>
        </div>
    );
};

export default AboutPage;
