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

        [TestMethod]
        public void Process_CircularRedirectWithOtherEntries_ThrowsExceptionWithMessage()
        {
            var failureInput = new string[]
            {
                "/one.html -> /two.html",
                "/two.html -> /home",
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

        [TestMethod]
        public void Process_DifferentInputOrders_ReturnSameResult()
        {
            var input = new string[]
            {
                "/start -> /second",
                "/a -> /b",
                "/b -> /c",
                "/second -> /another",
                "/about"
            };

            var analyzer = new RouteAnalyzer();
            var result = analyzer.Process(input).ToList();
            var reversedResult = analyzer.Process(input.Reverse()).ToList();

            var expected = new[]
            {
                "/start -> /second -> /another",
                "/a -> /b -> /c",
                "/about"
            };
            CollectionAssert.AreEquivalent(expected, result);
            CollectionAssert.AreEquivalent(expected, reversedResult);
        }

        [TestMethod]
        public void Process_LongRedirectChain_ConsolidatesToSingleResult()
        {
            var input = new string[]
            {
                "/home",
                "/one.html -> /two.html",
                "/two.html -> /three.html",
                "/three.html -> /home"
            };

            var analyzer = new RouteAnalyzer();
            var result = analyzer.Process(input).ToList();

            var expected = new[] { "/one.html -> /two.html -> /three.html -> /home" };
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestMethod]
        public void Process_MultipleEntryPointsRedirectToSameDestination_ReturnsBothResults()
        {
            var input = new string[]
            {
                "/a.html -> /home",
                "/b.html -> /home",
                "/c.html -> /home"
            };

            var analyzer = new RouteAnalyzer();
            var result = analyzer.Process(input).ToList();

            var expected = new[] 
            {
                "/a.html -> /home",
                "/b.html -> /home",
                "/c.html -> /home"
            };
            CollectionAssert.AreEquivalent(expected, result);
        }
    }
}
