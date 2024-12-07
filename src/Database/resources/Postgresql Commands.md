# Useful SQL commands

## Get the most recent calculated recipe cost

Updated by the `Accountant` service

```SELECT max(updated AT TIME ZONE 'EDT') FROM recipecost;```

## Get the most recent calculated recipe profit

Updated by the `Accountant` service

```SELECT max(updated AT TIME ZONE 'EDT') FROM recipeprofit;```
