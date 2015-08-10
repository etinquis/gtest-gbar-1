using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar.Lib;
using NUnit.Framework;
using TestStatus = Guitar.Lib.TestStatus;

namespace Guitar.Tests.GTest
{
    [TestFixture]
    class GTestTestFactoryTests
    {
        private GTestTestFactory createFactroyUnderTest()
        {
            return new GTestTestFactory();
        }

        [Test]
        public void BuildTestSuite()
        {
            var factoryUnderTest = createFactroyUnderTest();

            var suite = factoryUnderTest.BuildTestSuite("SuiteName", @"C:\tests.exe");
            Assert.AreEqual("SuiteName", suite.Name);
            Assert.AreEqual(@"C:\tests.exe", suite.RunTarget);
            Assert.AreEqual(0, suite.TestCases.Length);
        }

        [Test]
        public void BuidlTestCase_NullSuite()
        {
            var factoryUnderTest = createFactroyUnderTest();

            var testCase = factoryUnderTest.BuildTestCase(null, "TestCase");
            Assert.NotNull(testCase);
            Assert.AreEqual("TestCase", testCase.Name);
            Assert.IsNull(testCase.Suite);
            Assert.NotNull(testCase.LastResult);
            Assert.AreEqual(TestStatus.NotRun, testCase.LastResult.Outcome);
            Assert.AreEqual(0, testCase.Tests.Length);
        }

        [Test]
        public void BuildTest_NullTestCase()
        {
            var factoryUnderTest = createFactroyUnderTest();

            var test = factoryUnderTest.BuildTest(null, "Test");
            Assert.AreEqual("Test", test.Name);
            Assert.IsNull(test.Case);
            Assert.AreEqual("Test", test.FullyQualifiedName);
        }
    }
}
