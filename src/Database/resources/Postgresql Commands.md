# Useful SQL commands

## Get the most recent price in the database

Updated by the `DataUpdater` service

```SELECT max(to_timestamp(lastuploadtime / 1000.0) AT TIME ZONE 'UTC' AT TIME ZONE 'EST') FROM price;```

## Get the most recent calculated recipe cost

Updated by the `Accountant` service

```SELECT max(updated AT TIME ZONE 'EDT') FROM recipecost;```

## Get the most recent calculated recipe profit

Updated by the `Accountant` service

```SELECT max(updated AT TIME ZONE 'EDT') FROM recipeprofit;```
