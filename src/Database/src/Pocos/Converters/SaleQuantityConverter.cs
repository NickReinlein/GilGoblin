using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GilGoblin.Database.Pocos.Converters;

public class SaleQuantityConverter()
    : ValueConverter<SaleQuantity, decimal>(
        saleQuantity => saleQuantity.Quantity, 
        quantity => new SaleQuantity(quantity));