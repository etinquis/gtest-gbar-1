using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Guitar.Lib;
using Guitar.Lib.GTest;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Linq;
using Plugin.VisualStudio2012.VisualStudio;

namespace Plugin.VisualStudio2012.GTest
{
    [ExtensionUri(GTestExecutor.ExecutorUriString)]
    public class GTestExecutor : ITestExecutor
    {
        public const string ExecutorUriString = "executor://gtestexecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(GTestExecutor.ExecutorUriString);

        private readonly List<KeyValuePair<Process, GTestRunOutputParser>> _testProcesses;
        private IFrameworkHandle _frameworkHandle;

        public GTestExecutor()
        {
            _testProcesses = new List<KeyValuePair<Process, GTestRunOutputParser>>();
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var settingsProvider =
                runContext.RunSettings.GetSettings(VSTestSettings.SettingsName) as VSTestSettingsService;

            VSTestSettings settings;
            if (settingsProvider != null)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Found settings.");
                settings = settingsProvider.Settings;
            }
            else
            {
                frameworkHandle.SendMessage(TestMessageLevel.Informational, "No settings found. Using defaults.");
                settings = new VSTestSettings();
            }

            frameworkHandle.SendMessage(TestMessageLevel.Informational, settings.WorkingDirectory);

            _frameworkHandle = frameworkHandle;

            ITestLogger logger = new VSLogger(frameworkHandle);

            GTestConverter converter = new GTestConverter();
            IEnumerable<ITestSuite> suites = converter.ConvertToGTest(tests.ToArray(), logger);

            foreach (var suite in suites)
            {
                logger.Information(string.Format("Processing suite {0}...", suite.RunTarget));
                VSTracker tracker = new VSTracker(frameworkHandle, suite);
                GTestRunner runner = new GTestRunner(logger, false);

                runner.TestCompleted += tracker.TestCompleted;
                logger.Information(string.Format("Running suite {0}...", suite.RunTarget));
                runner.Run(suite);
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            VSLogger logger = new VSLogger(frameworkHandle);

            foreach (var source in sources)
            {
                logger.Information(string.Format("Attempting to run tests in {0}", source));

                GTestExtractor extractor = new GTestExtractor(false);
                ITestSuite suite = extractor.ExtractFrom(source);

                VSTracker tracker = new VSTracker(frameworkHandle, suite);
                GTestRunner runner = new GTestRunner(logger, false);
                runner.TestCompleted += tracker.TestCompleted;

                runner.Run(suite);
            }
        }

        public void Cancel()
        {
            foreach (var procPair in _testProcesses)
            {
                //procPair.Key.Kill();
                //procPair.Key.OutputDataReceived -= procPair.Value.DataReceivedEventHandler;
            }
        }
    }
}
