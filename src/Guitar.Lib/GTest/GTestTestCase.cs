using System;
using System.Collections.Generic;

namespace Guitar.Lib
{
    class GTestTestCase : ITestCase
    {
        private List<ITest> tests;
        private GTestTestFactory factory = new GTestTestFactory();

        public GTestTestCase(string name, ITestSuite suite)
        {
            if(name == null) throw new ArgumentNullException("name");
            if(suite == null) throw new ArgumentNullException("suite");

            Suite = suite;
            Name = name;
            LastResult = new TestResult() {Message = "Not Run", Outcome = TestStatus.NotRun};
            tests = new List<ITest>();
        }

        public string Name { get; private set; }
        public ITestSuite Suite { get; private set; }
        public ITest[] Tests { get { return tests.ToArray(); } }
        public TestResult LastResult { get; private set; }
        public void AddTest(ITest test)
        {
            if(test == null) throw new ArgumentNullException("test");

            tests.Add(test);
            test.TestCompleted += TestOnTestCompleted;
        }

        public event ResultUpdatedHandler ResultUpdated;

        protected virtual void OnResultUpdated(TestResult result)
        {
            ResultUpdatedHandler handler = ResultUpdated;
            if (handler != null) handler(this, result);
        }

        private void TestOnTestCompleted(ITest test, TestResult result)
        {
            TestStatus status = TestStatus.Passed;
            foreach (var test1 in Tests)
            {
                if (status == TestStatus.Passed)
                {
                    if (test1.LastResult.Outcome == TestStatus.Failed || test1.LastResult.Outcome == TestStatus.NotRun || test1.LastResult.Outcome == TestStatus.Ignored)
                    {
                        status = test1.LastResult.Outcome;
                    }
                }
            }

            LastResult = new TestResult() {Outcome = status};
            OnResultUpdated(LastResult);
        }
    }
}
