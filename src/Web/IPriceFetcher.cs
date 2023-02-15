using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public interface IPriceFetcher
    : IDataFetcher<PriceWebPoco, PriceWebResponsePoco>,
        IPriceRepository<PriceWebPoco> { }
