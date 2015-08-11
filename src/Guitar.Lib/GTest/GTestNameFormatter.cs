using System;
using System.Collections.Generic;
using System.Text;

namespace Guitar.Lib.GTest
{
    class GTestNameFormatter
    {
        public static string GetRunName(ITest test)
        {
            if (test == null) return string.Empty;
            if (test.Case != null)
            {
                return string.Format("{0}.{1}", test.Case.Name, test.Name);
            }
            else
            {
                return test.Name;
            }
        }

        public static string GetRunName(ITestCase testCase)
        {
            return string.Format("{0}.*", testCase.Name);
        }
    }
}
