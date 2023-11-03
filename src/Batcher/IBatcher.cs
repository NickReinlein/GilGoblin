using System.Collections.Generic;

namespace GilGoblin.Batcher;

public interface IBatcher<T>
{
    List<List<T>> SplitIntoBatchJobs(List<T> entries);
}
