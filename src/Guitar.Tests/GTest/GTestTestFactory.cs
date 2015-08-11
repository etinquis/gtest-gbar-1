using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar.Lib;
using Moq;
using NUnit.Framework;
using TestStatus = Guitar.Lib.TestStatus;

namespace Guitar.Tests.GTest
{
    [TestFixture]
    class GTestTestFactoryTests
    {
        private GTestTestFactory createFactoryUnderTest()
        {
            return new GTestTestFactory();
        }

        [Test]
        public void BuildTestSuite()
        {
            var factoryUnderTest = createFactoryUnderTest();

            var suite = factoryUnderTest.BuildTestSuite("SuiteName", @"C:\tests.exe");
            Assert.AreEqual("SuiteName", suite.Name);
            Assert.AreEqual(@"C:\tests.exe", suite.RunTarget);
            Assert.AreEqual(0, suite.TestCases.Length);
        }

        [Test]
        public void BuildTestSuite_NullOrEmptySuiteName()
        {
            var factoryUnderTest = createFactoryUnderTest();

            Assert.Throws<ArgumentNullException>(() =>
                factoryUnderTest.BuildTestSuite(null, "C:\tests.exe"));
            Assert.Throws<ArgumentException>(() =>
                factoryUnderTest.BuildTestSuite(string.Empty, "C:\tests.exe"));
        }

        [Test]
        public void BuildTestSuite_NullOrEmptyRunTarget()
        {
            var factoryUnderTest = createFactoryUnderTest();

            Assert.Throws<ArgumentNullException>(() =>
                factoryUnderTest.BuildTestSuite("TestSuite", null));
            Assert.Throws<ArgumentException>(() =>
                factoryUnderTest.BuildTestSuite("TestSuite", string.Empty));
        }

        [Test]
        public void BuidlTestCase_NullSuite()
        {
            var factoryUnderTest = createFactoryUnderTest();

            Assert.Throws<ArgumentNullException>(() =>
                factoryUnderTest.BuildTestCase(null, "TestCase"));
        }

        [Test]
        public void BuildTest_NullTestCase()
        {
            var factoryUnderTest = createFactoryUnderTest();

            Assert.Throws<ArgumentNullException>(() =>
                factoryUnderTest.BuildTest(null, "Test"));
        }

        [Test]
        public void BuildTest_NullOrEmptyTestName()
        {
            var factoryUnderTest = createFactoryUnderTest();

            Mock<ITestCase> mockTestCase = new Mock<ITestCase>();
            Assert.Throws<ArgumentNullException>(() =>
                factoryUnderTest.BuildTest(mockTestCase.Object, null));
            Assert.Throws<ArgumentException>(() =>
                factoryUnderTest.BuildTest(mockTestCase.Object, string.Empty));
        }

        [Test]
        public void BuildRunner_NullLogger()
        {
            var factoryUnderTest = createFactoryUnderTest();
            var runner = factoryUnderTest.BuildRunner(null);
            Assert.NotNull(runner);
        }

        [Test]
        public void BuildRunner_WithLogger()
        {
            Mock<ITestLogger> mockLogger = new Mock<ITestLogger>();
            var factoryUnderTest = createFactoryUnderTest();
            var runner = factoryUnderTest.BuildRunner(mockLogger.Object);
            Assert.NotNull(runner);
        }
    }
}
