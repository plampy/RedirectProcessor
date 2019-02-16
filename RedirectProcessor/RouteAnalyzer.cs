using RedirectProcessor.Interface;
using RedirectProcessor.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedirectProcessor
{
    public class RouteAnalyzer : IRouteAnalyzer
    {
        public IEnumerable<string> Process(IEnumerable<string> routes)
        {
            var results = new List<RedirectGraph>();
            var redirectRules = routes.Select(s =>
            {
                var parts = s.Split(new string[] { " -> " }, StringSplitOptions.None);
                var route = new Route { Path = parts[0] };
                if (parts.Length == 2)
                    route.RedirectsTo = parts[1];
                return route;
            });
            var entryPoints = redirectRules.Select(r => r.Path);
            foreach (var entry in entryPoints)
            {
                var graph = new RedirectGraph(entry);
                Func<Route, bool> isRouteValidRedirect = route => route.Path == graph.End && route.IsRedirect;
                while (redirectRules.Any(isRouteValidRedirect))
                {
                    var nextRedirect = redirectRules.First(isRouteValidRedirect);
                    graph.Append(nextRedirect.RedirectsTo);
                    if (graph.Start == graph.End)
                    {
                        throw new Exception($"Circular route found: {graph.ToString()}");
                    }
                }
                results.Add(graph);
            }
            //group by terminating route, order by longest chain, take first
            return results.GroupBy(g => g.End)
                .Select(grp => grp
                    .OrderByDescending(graph => graph.Length)
                    .First()
                )
                .Select(g => g.ToString());
        }
    }
}
