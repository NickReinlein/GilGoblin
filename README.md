# GilGoblin

## Summary

GilGoblin is a REST API for FFXIV, written in C#. It calculates the most profitable items to craft based on market prices, vendor prices and crafting component costs. It stores the data in Postgres database. The prices are refreshed by calling an API endpoint.

The initial functionality includes the following:

* Estimating the average price from the market board listings (100%)
* Getting the vendor cost, if available via vendor (100%)
* Getting the recipe to craft if the item can be crafted (100%)
* Calculating the cost to craft from the components, based on the recipe in the previous step (100%)
* The minimum price of the vendor or the crafting cost or the market board is used to calculate a minimum cost (100%)
* The profitability per item is calculated based on the difference of current market prices and the estimated minimum cost (100%)

In the immediate future, the features to be implemented are:

* A database to store prices locally and only update periodically when staleness exceeds a threshold (100%)
* A ~~binary tree~~ recursive algorithm to gather cascading crafting components (100%)
* ~~A front-end UI to display results (0%)~~ (now out of scope)
* Basic filtering. Initially by Gil-generating activity (crafting, ~~gathering, quests, etc.~~) (80% for crafting, others out of scope)
* Logging (100%)

## Later

Additional functionality that is currently considered but no immediate plans to implement:

* Sorting by profitability
* Filter for HQ/NQ
* Filters for crafting/harvesting items (20%)
* Limiting the data freshness (ie: max listing time from 7 days ago) (30%)
* Filtering by job level availability (later)
* Including estimated profitability based on sale momentum and approximate demand (later)

## Special Thanks & References
Other Projects:
* XIVAPI: A FINAL FANTASY XIV: Online REST API ( https://xivapi.com/ )
* Universalis: A crowdsourced market board aggregator for the game FFXIV ( https://github.com/Universalis-FFXIV/Universalis )