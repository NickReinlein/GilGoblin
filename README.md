# GilGoblin

## Summary

`GilGoblin` is a end-to-end application which calculates in-game crafting profitability in Final Fantasy XIV (FFXIV). 

It calculates the most profitable items to craft based on current market prices, vendor prices and crafting component costs. It stores recent prices in a local PostgreSQL database. The prices are refreshed by a background service.

A webpage front-end is available with the resulting calculations. Filters are available per world, allowing users to see the most profitable items specific to their server.

**Complete**
* :heavy_check_mark: Getting the vendor cost, if available via vendor
* :heavy_check_mark: Getting the recipe to craft if the item can be crafted
* :heavy_check_mark: Calculating the cost to craft from the components, based on the recipe in the previous step
* :heavy_check_mark: The minimum price of the vendor or the crafting cost or the market board is used to calculate a minimum cost
* :heavy_check_mark: The profitability per item is calculated based on the difference of current market prices and the estimated minimum cost
* :heavy_check_mark: A crafting calculator that breaks down recipes to ingredients
* :heavy_check_mark: Calculate most profitable recipes to craft for a given FFXIV server/world
* :heavy_check_mark: A new service to pre-calculate costs and profits
* :heavy_check_mark: A website to search items, recipes, prices, and the craft summary  💰
* :heavy_check_mark: Add to website a table to display the most profitable recipes to craft for each world 📈
* :heavy_check_mark: Enhance most profitable recipes table to be sortable 💲
* :heavy_check_mark: Add arrows to profit table column headers to indicate sorting status (ascending, descending, none) ⬇️
* :heavy_check_mark: Create a Docker image of the front-end 🐋
* :heavy_check_mark: Fix existing high-priority bugs 🐛
* :heavy_check_mark: Add support for multiple worlds 🌐
* :heavy_check_mark: Add about project page ℹ️
* :heavy_check_mark: Add Gilgoblin favicon and title to header 📖
* :heavy_check_mark: Add performance tests for high-traffic bottlenecks (top-crafts) 🚥
* :heavy_check_mark: Improve performance for top-crafts endpoint ⏩

*Work-in-progress*
* :hatching_chick: Make everything *prettier* ✨
* :hatching_chick: Launch and test website 🚀

*Upcoming*
* :egg: Live release bug fixes 🐛

*Nice-to-have*
* :question: Top things to gather 🌳🪓 🪨⛏️
* :question: A link/recipe details for each profit result's items, ingredients, prices, etc. in the profit table row 🕵️
* :question: A hover or clickable option to see the profit's ingredients & cost info ☁️
* :question: Advanced filtering: Enter class levels to filter available recipes 🧰
* :question: A website displaying the top crafts *for each profession* 🛠️

*Not currently on the product roadmap*
* :heavy_multiplication_x: Filter for HQ/NQ 
* :heavy_multiplication_x: Any interactions with the market board or in-game

## Technologies
* The back-end REST API is written in C# (dotnet 7, EF6)
* The back-end tests use NUnit3
* The front-end code uses the React library, written in Typescript (React 18)
* The front-end tests use Jest
* There are 5 Docker images (listed below) specific to GilGoblin
* K6 benchmarks feed Prometheus. Test results are displayed in a Grafana dashboard

## Startup
From the root folder of the project, run `docker-compose up -d` to build the images. The database will load, and if necessary run the scripts to populate the tables. The front end is available by default over port 3000. The other services will run in the background periodically to refresh data.

The docker containers need to be running and healthy for the component tests to pass.

## Docker Images

* [nickreinlein/gilgoblin-api](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-api/general)
  * A C# REST API to get information on items, recipes, prices
  * Provides the best crafts for a specific world given current prices
* [nickreinlein/gilgoblin-frontend](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-frontend/general)
  * A React frontend that displays results from endpoints in `gilgoblin-api`
* [nickreinlein/gilgoblin-database](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-database/general)
  * A PostgreSQL database which hosts the GilGoblin database
  * On initialization, scripts are run to create the tables and populate with data
* [nickreinlein/gilgoblin-dataupdater](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-dataupdater/general)
  * A background service that will check every 5 minutes for outdated prices to update in `gilgoblin-database`
  * Currently, prices are considered outdated after 48 hours
* [nickreinlein/gilgoblin-accountant](https://hub.docker.com/repository/docker/nickreinlein/gilgoblin-accountant/general)
  * A background service that will verify every 5 minutes for costs and profits to calculate in `gilgoblin-database`

## Special Thanks & References
Support:
* Jetbrains IDEs ( https://www.jetbrains.com/ )

Other Projects:
* XIVAPI: A FINAL FANTASY XIV: Online REST API ( https://xivapi.com/ )
* Universalis: A crowdsourced market board aggregator for the game FFXIV ( https://github.com/Universalis-FFXIV/Universalis )
