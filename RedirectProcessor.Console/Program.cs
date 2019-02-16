using System;
using System.Collections.Generic;
using System.Linq;

namespace RedirectProcessor.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new List<string>
            {
                "/home",
                "/one.html -> /two.html",
                "/two.html -> /three.html",
                "/three.html -> /home"
            };

            var analyzer = new RouteAnalyzer();
            Console.WriteLine("Processing...");
            input.ForEach(Console.WriteLine);
            var results = analyzer.Process(input).ToList();
            Console.WriteLine("Resulting paths:");
            results.ForEach(Console.WriteLine);
            Console.ReadLine();
        }
    }
}
