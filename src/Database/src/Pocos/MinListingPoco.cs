namespace GilGoblin.Database.Pocos;

public record MinListingPoco(
    PriceDataDetailPoco? World,
    PriceDataDetailPoco? Dc,
    PriceDataDetailPoco? Region,
    int Id,
    int ItemId,
    bool IsHq,
    int? WorldDataPointId,
    int? DcDataPointId,
    int? RegionDataPointId)
    : PriceDataPointDbPoco(World, Dc, Region, Id, ItemId, IsHq, WorldDataPointId,
        DcDataPointId, RegionDataPointId);