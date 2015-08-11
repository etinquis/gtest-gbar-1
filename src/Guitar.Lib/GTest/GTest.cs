using System;

namespace Guitar.Lib.GTest
{
    class GTest : ITest
    {
        public GTest(string name, ITestCase testCase)
        {
            if(name == null) throw new ArgumentNullException("name");
            if(testCase == null) throw new ArgumentNullException("testCase");

            if(string.IsNullOrEmpty(name)) throw new ArgumentException("Test Name cannot be null", "name");

            Name = name;
            Case = testCase;

	        if (Name.Contains("#"))
	        {
		        var hashIdx = Name.IndexOf('#');
		        Description = Name.Substring(hashIdx + 1).Trim(' ');
				Name = Name.Substring(0, hashIdx).Trim(' ');
			}
        }

        public ITestCase Case { get; private set; }
        public string Name { get; private set; }
		public string Description { get; private set; }
        public TestResult LastResult { get; private set; }
        public string FullyQualifiedName { get { return GTestNameFormatter.GetRunName(this); } }

        public void Completed(TestResult result)
        {
            LastResult = result;
            TestCompletedHandler handler = TestCompleted;
            if (handler != null) handler(this, result);
        }

        public event TestCompletedHandler TestCompleted;
    }
}
