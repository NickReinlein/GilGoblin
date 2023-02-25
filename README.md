# GilGoblin

## Summary

GilGoblin is a REST API for FFXIV, written in C#. It calculates the most profitable items to craft based on current market prices, vendor prices and crafting component costs. It stores recent prices in a local SQLite database. The prices are refreshed by a background service. 

The long-term goal is to have a website using this API's endpoints to display the top crafts for each crafting profession (specific to the user-selected world)

Eventually, features will includes the following:
* Getting the vendor cost, if available via vendor
* Getting the recipe to craft if the item can be crafted
* Calculating the cost to craft from the components, based on the recipe in the previous step
* The minimum price of the vendor or the crafting cost or the market board is used to calculate a minimum cost
* The profitability per item is calculated based on the difference of current market prices and the estimated minimum cost
* A crafting calculator that breaks down recipes to ingredients 
* Basic filtering. Top things to craft. Top things to gather
* Advanced filtering: Enter class levels to filter available recipes
* A website displaying the top crafts for each profession for each world
* Filter for HQ/NQ

## Special Thanks & References
Other Projects:
* XIVAPI: A FINAL FANTASY XIV: Online REST API ( https://xivapi.com/ )
* Universalis: A crowdsourced market board aggregator for the game FFXIV ( https://github.com/Universalis-FFXIV/Universalis )
