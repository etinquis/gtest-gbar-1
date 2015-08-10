﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Guitar.Lib.GTest
{
    public class GTestRunner : ITestRunner
    {
        private BackgroundWorker _worker;
        private ITestLogger _logger;
        private bool _async;

        private class RunArgs
        {
            public string Filter;
            public ITestSuite Suite;
        }

        public GTestRunner(ITestLogger logger, bool async = true)
        {
            _async = async;
            _logger = logger;
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += WorkerDoWork;
            _worker.RunWorkerCompleted += WorkerCompleted;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            OnRunCompleted();
        }

        private void QueueRun(ITestSuite suite, string filter)
        {
            _worker.RunWorkerAsync(new RunArgs() {Suite = suite, Filter = filter});

            if(!_async)
            {
                while (_worker.IsBusy)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Run(e.Argument as RunArgs);
            } 
            catch(Exception ex)
            {
                e.Result = ex;
            }
        }

        private void Run(RunArgs args)
        {
            _logger.Information(string.Format("Beginning test run of {0}...", args.Suite.RunTarget));
            GTestRunOutputParser parser = new GTestRunOutputParser(args.Suite, _logger);
            DataReceivedEventHandler handler = (sender, eventArgs) => parser.ParseLine(eventArgs.Data);
            parser.TestFinished += OnTestCompleted;

            foreach (var testCase in args.Suite.TestCases)
            {
                foreach (var test in testCase.Tests)
                {
                    if(test.Name.StartsWith("DISABLED_"))
                    {
                        TestResult result = new TestResult() {Outcome = TestStatus.Ignored};
                        test.Completed(result);
                        OnTestCompleted(test, result);
                    }
                }
            }

            ProcessStartInfo gtestProcInfo = new ProcessStartInfo(args.Suite.RunTarget)
                {
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
	        if (!string.IsNullOrEmpty(args.Filter))
	        {
		        gtestProcInfo.Arguments = string.Format("--gtest_filter={0}", args.Filter);
	        }
	        else
	        {
		        List<string> caseNames = new List<string>();
				foreach (var testCase in args.Suite.TestCases)
				{
					foreach (var test in testCase.Tests)
					{
						caseNames.Add(test.FullyQualifiedName);
					}
				}
		        if (caseNames.Count > 0)
		        {
			        gtestProcInfo.Arguments = string.Format("--gtest_filter={0}", string.Join(":", caseNames.ToArray()));
		        }
	        }

            Process gtestProc = new Process();
            gtestProc.StartInfo = gtestProcInfo;
            gtestProc.OutputDataReceived += handler;

            gtestProc.Start();
            gtestProc.BeginOutputReadLine();

            while (!gtestProc.HasExited)
            {
                gtestProc.WaitForExit(0);
                if (_worker.CancellationPending)
                {
                    gtestProc.Close();
                    break;
                }
            }

            gtestProc.WaitForExit();

            gtestProc.OutputDataReceived -= handler;
        }

        public void Run(ITestSuite testSuite)
        {
            QueueRun(testSuite, string.Empty);
        }

        public void Run(ITestCase testCase)
        {
            Run(new[] {testCase});
        }

        public void Run(IEnumerable<ITestCase> testCases)
        {
            if (testCases == null) throw new ArgumentNullException("testCases");

            StringBuilder filter = new StringBuilder();
            ITestSuite suite = null;
            bool first = true;
            foreach (var @case in testCases)
            {
                if (first)
                {
                    first = false;
                    suite = @case.Suite;

                    filter.Append(@case.Name + ".*");
                }
                else
                {
                    if(@case.Suite != suite)
                    {
                        throw new ArgumentException("test cases must be from the same suite");
                    }
                    filter.AppendFormat(":{0}", @case.Name + ".*");
                }
            }

            if(first) throw new ArgumentException("testCases cannot be empty");
            if (suite == null) throw new ArgumentNullException("suite of first test case");
            QueueRun(suite, filter.ToString());
        }

        public void Run(ITest test)
        {
            Run(new[] {test});
        }

        public void Run(IEnumerable<ITest> tests)
        {
            if (tests == null) throw new ArgumentNullException("tests");

            StringBuilder filter = new StringBuilder();
            ITestSuite suite = null;
            bool first = true;
            foreach (var test in tests)
            {
                if (first)
                {
                    first = false;
                    suite = test.Case.Suite;

                    filter.AppendFormat("{0}.{1}", test.Case.Name, test.Name);
                }
                else
                {
                    if (test.Case.Suite != suite)
                    {
                        throw new ArgumentException("tests must be from the same suite");
                    }
                    filter.AppendFormat(":{0}.{1}", test.Case.Name, test.Name);
                }
            }

            if (suite == null) throw new ArgumentNullException("suite of first test");
            QueueRun(suite, filter.ToString());
        }

        public event TestCompletedHandler TestCompleted;

        protected virtual void OnTestCompleted(ITest test, TestResult result)
        {
            TestCompletedHandler handler = TestCompleted;
            if (handler != null) handler(test, result);
        }

        public event EventHandler RunCompleted;

        protected virtual void OnRunCompleted()
        {
            EventHandler handler = RunCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
