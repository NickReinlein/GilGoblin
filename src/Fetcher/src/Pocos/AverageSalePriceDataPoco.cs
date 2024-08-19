namespace GilGoblin.Fetcher.Pocos;

public record AverageSalePriceDataPoco(
    PriceAggregatedPriceDetail World,
    PriceAggregatedPriceDetail Dc,
    PriceAggregatedPriceDetail Region);