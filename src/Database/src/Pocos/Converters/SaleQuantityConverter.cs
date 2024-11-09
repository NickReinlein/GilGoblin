using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GilGoblin.Database.Pocos.Converters;

public class SaleQuantityConverter() : ValueConverter<SaleQuantity?, decimal?>(
    v => v != null ? v.Quantity : 0,
    v => new SaleQuantity(v ?? 0));