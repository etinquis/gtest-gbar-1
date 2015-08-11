using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Guitar.Lib;
using Moq;
using TestStatus = Guitar.Lib.TestStatus;

namespace Guitar.Tests.GTest
{
    [TestFixture]
    internal class GTestTestCaseTests
    {
        private ITestCase createTestCaseUnderTest(ITestSuite suite, string testCaseName)
        {
            GTestTestFactory factory = new GTestTestFactory();
            return factory.BuildTestCase(suite, testCaseName);
        }

        [Test]
        public void InitialState()
        {
            Mock<ITestSuite> mockSuite = new Mock<ITestSuite>();
            var testCaseUnderTest = createTestCaseUnderTest(mockSuite.Object, "TestCase");

            Assert.NotNull(testCaseUnderTest.LastResult);
            Assert.AreEqual(TestStatus.NotRun, testCaseUnderTest.LastResult.Outcome);
            Assert.AreEqual("Not Run", testCaseUnderTest.LastResult.Message);
            Assert.NotNull(testCaseUnderTest.Tests);
            Assert.AreEqual(0, testCaseUnderTest.Tests.Length);
            Assert.AreEqual("TestCase", testCaseUnderTest.Name);
            Assert.AreEqual(mockSuite.Object, testCaseUnderTest.Suite);
        }

        [Test]
        public void AddTest_NullTest()
        {
            Mock<ITestSuite> mockSuite = new Mock<ITestSuite>();
            var testCaseUnderTest = createTestCaseUnderTest(mockSuite.Object, "TestCase");

            Assert.Throws<ArgumentNullException>(() =>
                testCaseUnderTest.AddTest(null));
        }

        [Test]
        public void AddTest()
        {
            Mock<ITestSuite> mockSuite = new Mock<ITestSuite>();
            var testCaseUnderTest = createTestCaseUnderTest(mockSuite.Object, "TestCase");

            Mock<ITest> mockTest = new Mock<ITest>();
            testCaseUnderTest.AddTest(mockTest.Object);

            Assert.AreEqual(1, testCaseUnderTest.Tests.Length);
        }

        [Test]
        public void AddedTest_UpdatesPassingTestCaseStatusWhenCompleted()
        {
            Mock<ITestSuite> mockSuite = new Mock<ITestSuite>();
            var testCaseUnderTest = createTestCaseUnderTest(mockSuite.Object, "TestCase");

            Mock<ITest> mockTest = new Mock<ITest>();
            var testResult = new TestResult();
            testResult.Outcome = TestStatus.NotRun;
            testResult.Message = null;
            mockTest.Setup(test => test.LastResult).Returns(() => testResult);
            testCaseUnderTest.AddTest(mockTest.Object);

            testResult.Outcome = TestStatus.Passed;
            testResult.Message = "Test Message";
            mockTest.Raise(test => test.TestCompleted += null, mockTest.Object, testResult);

            Assert.NotNull(testCaseUnderTest.LastResult);
            Assert.AreEqual(TestStatus.Passed, testCaseUnderTest.LastResult.Outcome);
        }

        [Test]
        public void AddedTest_UpdatesFailingTestCaseStatusWhenCompleted()
        {
            Mock<ITestSuite> mockSuite = new Mock<ITestSuite>();
            var testCaseUnderTest = createTestCaseUnderTest(mockSuite.Object, "TestCase");

            Mock<ITest> mockTest = new Mock<ITest>();
            var testResult = new TestResult();
            testResult.Outcome = TestStatus.NotRun;
            testResult.Message = null;
            mockTest.Setup(test => test.LastResult).Returns(() => testResult);
            testCaseUnderTest.AddTest(mockTest.Object);

            testResult.Outcome = TestStatus.Failed;
            testResult.Message = "Test Message";
            mockTest.Raise(test => test.TestCompleted += null, mockTest.Object, testResult);

            Assert.NotNull(testCaseUnderTest.LastResult);
            Assert.AreEqual(TestStatus.Failed, testCaseUnderTest.LastResult.Outcome);
        }
    }
}
