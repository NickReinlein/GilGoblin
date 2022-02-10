# GilGoblin

## Summary

GilGoblin is a locally run application for FFXIV, written in C#. This is a project intended to help the author learn C# and web API calls with synchronization. However, the project should prove useful to other players. GilGoblin calculates the most profitable items to flip or craft based on market prices, vendor prices and crafting component costs. 

## About
This is intended as a learning exercise that would also provide a useful tool to other FFXIV players. GilGoblin is currently developed & maintained by 1 developer working part-time on the project.

## Soon

GilGoblin is a pre-alpha status project to calculate profitable items in FFXIV. The plan is to pull market data from the appropriate world/data center and compare to the best available cost to calculate maximum potential profits for any items considered (ie: All materials that can be gathered by a "Miner", how much profit (if any) comes from crafting a "Mithril Ingot" based on component prices, such as "Mithril Ore" prices).

This means having the functionality to perform the following: 

* Estimating the average price from the market board listings (75%)
* Getting the vendor cost, if available via vendor (100%)
* Getting the recipe to craft if the item can be crafted (80%)
* Calculating the cost to craft from the components, based on the recipe in the previous step (40%)
* The minimum price of the vendor or the crafting cost or the market board is used to calculate a minimum cost (85%)
* The profitability per item is calculated based on the difference of current market prices and the estimated minimum cost (75%)

In the immediate future (Dec 2021->Feb 2022), the features to be implemented are:

* A database to store prices locally and only update periodically when staleness exceeds a threshold (100%)
* A ~~binary tree~~ recursive algorithm to gather cascading crafting components (60%)
* A front-end UI to display results (0%)
* Basic filtering. Initially by Gil-generating activity (crafting, gathering, quests, etc.) (0%)
* Logging (100%)

## Later

Additional functionality that is currently considered but no immediate plans to implement:

* Sorting by profitability
* Filter for HQ/NQ
* Filters for crafting/harvesting items
* Limiting the data freshness (ie: max listing time from 7 days ago)
* Filtering by job level availability (later)
* Including estimated profitability based on sale momentum and approximate demand (later)

## Special Thanks & References

* XIVAPI: A FINAL FANTASY XIV: Online REST API ( https://xivapi.com/ )
* Universalis: A crowdsourced market board aggregator for the game FFXIV ( https://github.com/Universalis-FFXIV/Universalis )
