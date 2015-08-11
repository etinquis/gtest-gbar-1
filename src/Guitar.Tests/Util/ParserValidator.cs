using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar.Lib;

namespace Guitar.Tests.Util
{
    internal class ParserValidator
    {
        private ITestRunOutputParser _parser;

        public bool TestDiscovered { get; private set; }
        public ITest DiscoveredTest { get; private set; }
        public bool TestStarted { get; private set; }
        public bool TestFinished { get; private set; }
        public TestResult FinishResult { get; private set; }

        public ParserValidator(ITestRunOutputParser parser)
        {
            _parser = parser;

            _parser.TestStarted += ParserOnTestStarted;
            _parser.TestDiscovered += ParserOnTestDiscovered;
            _parser.TestFinished += ParserOnTestFinished;

            TestDiscovered = false;
            TestStarted = false;
            TestFinished = false;
        }

        private void ParserOnTestDiscovered(ITest test)
        {
            TestDiscovered = true;
            DiscoveredTest = test;
        }

        private void ParserOnTestStarted(ITest test)
        {
            TestStarted = true;
        }

        private void ParserOnTestFinished(ITest test, TestResult result)
        {
            TestFinished = true;
            FinishResult = result;
        }
    }
}
