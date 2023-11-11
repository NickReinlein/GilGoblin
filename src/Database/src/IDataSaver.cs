using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Database;

public interface IDataSaver<in T> where T : class
{
    Task<bool> SaveAsync(IEnumerable<T> updates);
    bool SanityCheck(IEnumerable<T> updates);
}