using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Test;
using System;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Assembly.Test.TestSuite
{
    public abstract class TestSuiteTestBase : TestBase<Assembly.TestSuite>
    {
        protected Constant[] SuiteConstants;
        protected StepTemplate[] StepTemplates;

        protected override Assembly.TestSuite CreateSUT()
        {
            return new Assembly.TestSuite
            {
                Constants = SuiteConstants,
                StepTemplates = StepTemplates
            };
        }
    }

    public abstract class TestSuiteTestBase<TReturn> : TestSuiteTestBase
    {
        protected TReturn ReturnValue;
    }
}