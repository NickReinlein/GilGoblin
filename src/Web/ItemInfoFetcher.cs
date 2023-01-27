using System.Text.Json;
using GilGoblin.Pocos;

namespace GilGoblin.Web;

public class ItemInfoFetcher : DataFetcher<ItemInfoPoco>
{
    public static readonly string ItemInfoBaseUrl = """https://xivapi.com/item/""";

    public ItemInfoFetcher() : base(ItemInfoBaseUrl) { }
}
