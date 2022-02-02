using System.Globalization;

namespace Applique.LoadTester.Test
{
    public abstract class TestBase
    {
        protected TestBase() => CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        protected virtual void Arrange()
        {
            Given();
        }

        protected void ArrangeAndAct()
        {
            Arrange();
            Act();
        }

        protected abstract void Act();

        protected virtual void Given() { }
    }

    public abstract class TestBase<ISUT> : TestBase
    {
        protected TestBase() => CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        protected ISUT SUT { get; private set; }

        protected override sealed void Arrange()
        {
            Given();
            SUT = CreateSUT();
        }

        protected abstract ISUT CreateSUT();
    }
}