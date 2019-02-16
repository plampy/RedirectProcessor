using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace RedirectProcessor.Test
{
    [TestClass]
    public class RouteAnalyzerTests
    {
        [TestMethod]
        public void Process_SampleInput_MatchesOutput()
        {
            var input = new string[]
            {
                "/home",
                "/our-ceo.html -> /about-us.html",
                "/about-us.html -> /about",
                "/product-1.html -> /seo"
            };
            var analyzer = new RouteAnalyzer();
            var result = analyzer.Process(input).ToList();

            var expected = new[]
            {
                "/home",
                "/product-1.html -> /seo",
                "/our-ceo.html -> /about-us.html -> /about",
            };
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void Process_SingleCircularRedirectPath_ThrowsExceptionWithMessage()
        {
            var failureInput = new string[]
            {
                "/about-us.html -> /about",
                "/about -> /about-us.html"
            };
            var analyzer = new RouteAnalyzer();

            try
            {
                var result = analyzer.Process(failureInput).ToList();
                Assert.Fail("Expected to throw");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Circular route found"));
            }
        }
    }
}
