using System;
using System.Collections.Generic;
using System.Text;

namespace Guitar.Lib.Interfaces
{
    public interface ITestFactory
    {
        ITestRunner BuildRunner(ITestLogger logger);
    }
}
