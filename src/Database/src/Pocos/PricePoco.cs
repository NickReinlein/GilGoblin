using System.Collections.Generic;

namespace GilGoblin.Database.Pocos;

public record PricePoco(
    int ItemId,
    QualityPriceDataPoco? Hq = null,
    QualityPriceDataPoco? Nq = null,
    List<WorldUploadTimestampPoco>? WorldUploadTimes = null)
    : BasePricePoco(ItemId, Hq, Nq, WorldUploadTimes);