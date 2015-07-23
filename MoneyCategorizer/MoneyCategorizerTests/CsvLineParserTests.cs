using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoneyCategorizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer.Tests
{
    [TestClass()]
    public class CsvLineParserTests
    {
        CsvLineParser parser = new CsvLineParser();

        [TestMethod()]
        public void ParseTest1()
        {
            var parsed = parser.Parse("1,2,3");
            CollectionAssert.AreEqual(parsed, new[] { "1", "2", "3" });
        }

        [TestMethod()]
        public void ParseTest2()
        {
            var parsed = parser.Parse(",2,3");
            CollectionAssert.AreEqual(parsed, new[] { "", "2", "3" });
        }

        [TestMethod()]
        public void ParseTest3()
        {
            var parsed =parser.Parse("1,,3");
            CollectionAssert.AreEqual(parsed, new[] { "1", "", "3" });
        }

        [TestMethod()]
        public void ParseTest4()
        {
            var parsed =parser.Parse("1,2,");
            CollectionAssert.AreEqual(parsed, new[] { "1", "2", "" });
        }

        [TestMethod()]
        public void ParseTest5()
        {
            var parsed =parser.Parse("\"1,a\",2,3");
            CollectionAssert.AreEqual(parsed, new[] { "1,a", "2", "3" });
        }

        [TestMethod()]
        public void ParseTest6()
        {
            var parsed =parser.Parse("1,\"2,a\",3");
            CollectionAssert.AreEqual(parsed, new[] { "1", "2,a", "3" });
        }

        [TestMethod()]
        public void ParseTest7()
        {
            var parsed =parser.Parse("1,2,\"3,a\"");
            CollectionAssert.AreEqual(parsed, new[] { "1", "2", "3,a" });
        }

        [TestMethod()]
        public void ParseTest8()
        {
            var parsed = parser.Parse("1,\"2,b\",\"3,a\"");
            CollectionAssert.AreEqual(parsed, new[] { "1", "2,b", "3,a" });
        }
    }
}