using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public record PriceWebPoco(
    int ItemId,
    QualityPriceDataPoco? Hq = null,
    QualityPriceDataPoco? Nq = null,
    List<WorldUploadTimestampPoco>? WorldUploadTimes = null)
    : BasePricePoco(ItemId, Hq, Nq, WorldUploadTimes), IIdentifiable
{
}