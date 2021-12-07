# GilGoblin
In FFXIV, calculates the most profitable items to flip or craft based on market prices, vendor prices and crafting component costs

This is a pre-alpha status project to calculate profitable items in FFXIV. The plan is to pull market data from the appropriate world/data center and compare to the best available cost to calculate potential profits. This means: 
* Estimating the average price from the market board listings
* Getting the vendor cost, if available via vendor
* Getting the recipe to craft if the item can be crafted
* Calculating the cost to craft from the components, based on the recipe in the previous step
* The minimum price of the vendor or the crafting cost or the market board is used to calculate a minimum cost
* The profitability per item is calculated based on the difference of current market prices and the estimated minimum cost


Additional functionality that is currently considered:
* Sorting by profitability
* Filter for HQ/NQ
* Filters for crafting/harvesting items
* Limiting the data freshness (ie: max listing time from 7 days ago)
* Filtering by job level availability (later)
* Including estimated profitability based on sale momentum and approximate demand (later)
