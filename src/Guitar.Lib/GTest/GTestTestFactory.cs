using System;
using Guitar.Lib.GTest;
using Guitar.Lib.Interfaces;

namespace Guitar.Lib
{
    public class GTestTestFactory : ITestFactory
    {
        public ITest BuildTest(ITestCase testCase, string testName)
        {
            return new GTest.GTest(testName, testCase);
        }

        public ITestCase BuildTestCase(ITestSuite suite, string testCaseName)
        {
            return new GTestTestCase(testCaseName, suite);
        }

        public ITestSuite BuildTestSuite(string suiteName, string suiteFile)
        {
            return new GTestTestSuite(suiteName, suiteFile);
        }

        public ITestRunner BuildRunner(ITestLogger logger)
        {
            return new GTestRunner(logger);
        }
    }
}
