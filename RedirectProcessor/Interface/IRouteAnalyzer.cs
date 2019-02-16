using System.Collections.Generic;

namespace RedirectProcessor.Interface
{
    public interface IRouteAnalyzer
    {
        IEnumerable<string> Process(IEnumerable<string> routes);
    }
}
