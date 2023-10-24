using GilGoblin.Services;
using NUnit.Framework;

namespace GilGoblin.Tests.Services;

public class GeneralFunctionsTests
{
    [Test]
    public void WhenCallingConvertLongUnixMsToDateTimeWithASpecificInput_AConvertedDateTimeIsReturned()
    {
        var testTime = 1677761781541;

        var result = GeneralFunctions.ConvertLongUnixMsToDateTime(testTime);

        Assert.That(result.Year, Is.GreaterThanOrEqualTo(2023));
    }

    [Test]
    public void WhenCallingConvertLongUnixMsToDateTimeWithACorrectInput_AConvertedDateTimeIsReturned()
    {
        var currentTime = DateTime.Now.ToLocalTime();
        var currentMs = new DateTimeOffset(currentTime).ToUnixTimeMilliseconds();

        var result = GeneralFunctions.ConvertLongUnixMsToDateTime(currentMs);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.GreaterThan(currentTime.AddSeconds(-2)));
            Assert.That(result, Is.LessThan(currentTime.AddSeconds(2)));
        });
    }

    [Test]
    public void WhenCallingConvertLongUnixMsToDateTimeWithIncorrectInput_ADefaultDateTimeIsReturned()
    {
        var result = GeneralFunctions.ConvertLongUnixMsToDateTime(253402300800000);

        Assert.That(result, Is.EqualTo(new DateTime()));
    }
}
