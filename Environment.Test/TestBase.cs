using System.Globalization;

namespace Applique.LoadTester.Environment.Test
{
    public abstract class TestBase<ISUT>
    {
        protected TestBase() => CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        protected ISUT SUT { get; private set; }

        protected void Arrange()
        {
            Given();
            SUT = CreateSUT();
        }

        protected abstract ISUT CreateSUT();

        protected void ArrangeAndAct()
        {
            Arrange();
            Act();
        }

        protected abstract void Act();

        protected virtual void Given() { }
    }
}