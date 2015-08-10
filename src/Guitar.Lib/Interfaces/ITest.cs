﻿namespace Guitar.Lib
{
    public interface ITest 
    {
        ITestCase Case { get; }
        string Name { get; }
		string Description { get; }
        TestResult LastResult { get; }
        string FullyQualifiedName { get; }
        void Completed(TestResult result);
        event TestCompletedHandler TestCompleted;
    }
}