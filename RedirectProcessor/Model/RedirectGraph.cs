using System.Collections.Generic;
using System.Linq;

namespace RedirectProcessor.Model
{
    class RedirectGraph
    {
        List<string> Paths = new List<string>();
        public string Start => Paths.First();
        public string End => Paths.Last();

        public int Length => Paths.Count;

        public RedirectGraph(params string[] paths)
        {
            Paths.AddRange(paths);
        }

        public void Append(string route)
        {
            Paths.Add(route);
        }

        public bool ContainsChild(RedirectGraph graph)
        {
            return graph.Paths.All(path => Paths.Contains(path));
        }

        public override string ToString()
        {
            return string.Join(" -> ", Paths);
        }
    }
}
