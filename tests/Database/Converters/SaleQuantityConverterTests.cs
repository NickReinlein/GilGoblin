// using GilGoblin.Database.Pocos;
// using GilGoblin.Database.Pocos.Converters;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.Database.Converters;
//
// public class SaleQuantityConverterTests
// {
//     private SaleQuantityConverter _converter;
//
//     [SetUp]
//     public void SetUp()
//     {
//         _converter = new SaleQuantityConverter();
//     }
//
//     [Test]
//     public void ConvertToProvider_ValidSaleQuantity_ReturnsExpectedDecimal()
//     {
//         var saleQuantity = new SaleQuantity(5.5m);
//
//         var result = _converter.ConvertToProvider(saleQuantity);
//
//         Assert.That(result, Is.EqualTo(5.5m));
//     }
//
//     [Test]
//     public void ConvertFromProvider_ValidDecimal_ReturnsExpectedSaleQuantity()
//     {
//         const decimal valueFromDb = 10.25m;
//
//         var result = _converter.ConvertFromProvider(valueFromDb) as SaleQuantity;
//
//         Assert.That(result, Is.Not.Null);
//         Assert.That(result.Quantity, Is.EqualTo(10.25m));
//     }
//
//     [Test]
//     public void ConvertToProvider_NullSaleQuantity_ReturnsNull()
//     {
//         var result = _converter.ConvertToProvider(null);
//
//         Assert.That(result, Is.Null);
//     }
//
//     [Test]
//     public void ConvertFromProvider_NullDecimal_ReturnsNull()
//     {
//         var result = _converter.ConvertFromProvider(null);
//
//         Assert.That(result, Is.Null);
//     }
// }