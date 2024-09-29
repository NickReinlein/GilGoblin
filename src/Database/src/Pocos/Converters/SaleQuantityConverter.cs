using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GilGoblin.Database.Pocos.Converters;

public class SaleQuantityConverter() : ValueConverter<SaleQuantity?, decimal?>(sq => sq.Quantity,
    quantity => quantity.HasValue ? new SaleQuantity(quantity.Value) : null);