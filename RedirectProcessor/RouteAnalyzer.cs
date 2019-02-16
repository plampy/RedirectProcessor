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
            var redirectRules = Parse(routes);
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

            return ConsolidateChildGraphs(results).Select(g => g.ToString());
        }

        //Expected input format is delimited by " -> ", and contains either one or two paths
        private IEnumerable<Route> Parse(IEnumerable<string> routes)
        {
            return routes.Select(s =>
            {
                var parts = s.Split(new string[] { " -> " }, StringSplitOptions.None);
                var route = new Route { Path = parts[0] };
                if (parts.Length == 2)
                    route.RedirectsTo = parts[1];
                return route;
            });
        }

        private IEnumerable<RedirectGraph> ConsolidateChildGraphs(IEnumerable<RedirectGraph> graphs)
        {
            foreach (var graph in graphs)
            {
                var others = graphs.Except(new[] { graph });
                if (!others.Any(g => g.ContainsChild(graph)))
                    yield return graph;
            }
        }
    }
}
