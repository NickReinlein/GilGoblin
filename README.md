# GilGoblin

## Summary

`GilGoblin` is a back-end REST API, to calculate in-game crafting profitability in Final Fantasy XIV (FFXIV). It calculates the most profitable items to craft based on current market prices, vendor prices and crafting component costs. It stores recent prices in a local PostgreSQL database. The prices are refreshed by a background service.

The long-term goal is to have a website using this API's endpoints to display the top crafts for each crafting profession (specific to the user-selected world)

**Features will includes the following**:

* :heavy_check_mark:  Getting the vendor cost, if available via vendor
* :heavy_check_mark:  Getting the recipe to craft if the item can be crafted
* :heavy_check_mark:  Calculating the cost to craft from the components, based on the recipe in the previous step
* :heavy_check_mark:  The minimum price of the vendor or the crafting cost or the market board is used to calculate a minimum cost
* :heavy_check_mark:  The profitability per item is calculated based on the difference of current market prices and the estimated minimum cost
* :heavy_check_mark:  A crafting calculator that breaks down recipes to ingredients
* :heavy_check_mark:  Top things to craft
* :construction:  A website displaying the top crafts for each profession for each world

*Nice-to-have*:

* :question: Top things to gather
* :question: Advanced filtering: Enter class levels to filter available recipes

*Not currently on the product roadmap*:
* :heavy_multiplication_x: Filter for HQ/NQ

## Docker Images

* [nickreinlein/gilgoblin-api](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-api/general)
  * A REST API to get information on items, recipes, prices
  * Provides the best crafts for a specific world given current prices
* [nickreinlein/gilgoblin-database](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-database/general)
  * A PostgreSQL database which hosts the GilGoblin database
  * On initialization, scripts are run to create the tables and populate with data
* [nickreinlein/gilgoblin-dataupdater](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-dataupdater/general)
  * A background service that will check every 5 minutes for outdated prices to update in `gilgoblin-database`
  * Currently, prices are considered outdated after 48 hours

## Special Thanks & References
Support:
* Jetbrains IDEs ( https://www.jetbrains.com/ )

Other Projects:
* XIVAPI: A FINAL FANTASY XIV: Online REST API ( https://xivapi.com/ )
* Universalis: A crowdsourced market board aggregator for the game FFXIV ( https://github.com/Universalis-FFXIV/Universalis )
