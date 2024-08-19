using System.Text.Json.Serialization;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public class PriceAggregatedWebPoco : BasePricePoco
{
    public PriceDataPoco NQ;
    public PriceDataPoco HQ;
    public WorldUploadTimesPoco UploadTimes;

    public PriceAggregatedWebPoco() { }

    [JsonConstructor]
    public PriceAggregatedWebPoco(
        int itemId,
        int worldId,
        long lastUploadTime,
        PriceDataPoco nq,
        PriceDataPoco hq,
        WorldUploadTimesPoco uploadTimes
    )
    {
        ItemId = itemId;
        WorldId = worldId;
        LastUploadTime = lastUploadTime;
        NQ = nq;
        HQ = hq;
        UploadTimes = uploadTimes;
    }
}