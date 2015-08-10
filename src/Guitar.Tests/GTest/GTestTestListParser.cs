using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar.Lib;
using Guitar.Tests.Util;
using Moq;
using NUnit.Framework;

namespace Guitar.Tests.GTest
{
    [TestFixture]
    class GTestTestListParserTests
    {
        private GTestTestListParser CreateParserUnderTest(ITestSuite testSuite)
        {
            return new GTestTestListParser(testSuite);
        }

        private ITestSuite CreateMockTestSuite(IList<ITestCase> testCases)
        {
            Moq.Mock<ITestSuite> suite = new Mock<ITestSuite>();
            suite.SetupGet(s=>s.TestCases).Returns(testCases.ToArray());
            suite.Setup(s => s.AddTestCase(It.IsAny<ITestCase>())).Callback<ITestCase>(testCases.Add);
            return suite.Object;
        }

        [Test]
        public void ListParser_ParseNull()
        {
            var testCases = new List<ITestCase>();
            ITestSuite mockSuite = CreateMockTestSuite(testCases);
            var parserUnderTest = CreateParserUnderTest(mockSuite);
            var parserValidator = new ParserValidator(parserUnderTest);

            Assert.DoesNotThrow(() =>
                parserUnderTest.ParseLine(null));
        }

        [Test]
        public void ListParser_ParseTestCase()
        {
            var testCases = new List<ITestCase>();
            ITestSuite mockSuite = CreateMockTestSuite(testCases);
            var parserUnderTest = CreateParserUnderTest(mockSuite);
            var parserValidator = new ParserValidator(parserUnderTest);

            parserUnderTest.ParseLine("TestCase.");
            
            Assert.AreEqual(1, testCases.Count());
            Assert.False(parserValidator.TestDiscovered);
        }

        [Test]
        public void ListParser_ParseMultipleTestCases()
        {
            var testCases = new List<ITestCase>();
            ITestSuite mockSuite = CreateMockTestSuite(testCases);
            var parserUnderTest = CreateParserUnderTest(mockSuite);
            var parserValidator = new ParserValidator(parserUnderTest);

            parserUnderTest.ParseLine("TestCase1.");
            parserUnderTest.ParseLine("  Test");
            parserUnderTest.ParseLine("TestCase2.");
            parserUnderTest.ParseLine("  Test");

            Assert.AreEqual(2, testCases.Count());
        }

        [Test]
        public void ListParser_Discover()
        {
            var testCases = new List<ITestCase>();
            ITestSuite mockSuite = CreateMockTestSuite(testCases);
            var parserUnderTest = CreateParserUnderTest(mockSuite);
            var parserValidator = new ParserValidator(parserUnderTest);

            parserUnderTest.ParseLine("TestCase.");
            parserUnderTest.ParseLine("  Test");

            Assert.True(parserValidator.TestDiscovered);
        }
    }
}
