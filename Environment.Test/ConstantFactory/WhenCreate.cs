using Applique.LoadTester.Core.Design;
using Applique.LoadTester.Environment.Test.ConstantFactory;
using System;
using Xunit;
using static Applique.LoadTester.Environment.ConstantFactory;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public abstract class WhenCreate : ConstantFactoryTestBase<Constant>
    {
        protected string ConstantExpression;
        protected string ConstantValue;

        protected override void Act() => ReturnValue = Create(ConstantExpression, ConstantValue);

        public abstract class GivenNoValue : WhenCreate
        {
            protected GivenNoValue() => ConstantValue = null;
        }

        public class GivenNullExpression : GivenNoValue
        {
            [Fact]
            public void ThenReturnNull()
            {
                ConstantExpression = null;
                ArrangeAndAct();
                Assert.Null(ReturnValue);
            }
        }

        public class GivenEmptyExpression : GivenNoValue
        {
            [Fact]
            public void ThenReturnNull()
            {
                ConstantExpression = string.Empty;
                ArrangeAndAct();
                Assert.Null(ReturnValue);
            }
        }

        public class GivenName : GivenNoValue
        {
            [Fact]
            public void ThenReturnNamedStringConstant()
            {
                ConstantExpression = SomeString;
                ArrangeAndAct();
                Assert.Equal(SomeString, ReturnValue.Name);
                Assert.Equal(ConstantType.String, ReturnValue.Type);
            }
        }

        public class GivenColonName : GivenNoValue
        {
            [Fact]
            public void ThenReturnOvershadowingConstant()
            {
                ConstantExpression = $":{SomeString}";
                ArrangeAndAct();
                Assert.True(ReturnValue.Overshadow);
            }
        }

        public class GivenNameColonUnrecogniced : GivenNoValue
        {
            [Fact]
            public void ThenThrowArgumentException()
            {
                ConstantExpression = $"{SomeString}:int";
                Assert.Throws<ArgumentException>(ArrangeAndAct);
            }
        }

        public class GivenNameColonType : GivenNoValue
        {
            [Theory]
            [InlineData(ConstantType.Int)]
            [InlineData(ConstantType.Decimal)]
            [InlineData(ConstantType.Bool)]
            [InlineData(ConstantType.DateTime)]
            [InlineData(ConstantType.Seed)]
            [InlineData(ConstantType.String)]
            public void ThenReturnTypedConstant(ConstantType type)
            {
                ConstantExpression = $"{SomeString}:{type}";
                ArrangeAndAct();
                Assert.Equal(type, ReturnValue.Type);
            }
        }

        public class GivenNameSpaceUnrecogniced : GivenNoValue
        {
            [Fact]
            public void ThenThrowArgumentException()
            {
                ConstantExpression = $"{SomeString} {AnotherString}";
                Assert.Throws<ArgumentException>(ArrangeAndAct);
            }
        }

        public class GivenNameSpaceConstraint : GivenNoValue
        {
            [Theory]
            [InlineData(Constraint.Mandatory)]
            public void ThenReturnConstrainedConstant(Constraint constraint)
            {
                ConstantExpression = $"{SomeString} {constraint}";
                ArrangeAndAct();
                Assert.Equal(constraint, ReturnValue.Constraint);
            }
        }

        public class GivenTypeCast : GivenNoValue
        {
            [Fact]
            public void ThenReturnConstantWithConversion()
            {
                ConstantExpression = $"{SomeString}:Decimal->Int";
                ArrangeAndAct();
                Assert.Equal(ConstantType.Decimal, ReturnValue.Type);
                Assert.Equal(new[] { ConstantType.Int }, ReturnValue.Conversions);
            }
        }

        public class GivenTolerance : GivenNoValue
        {
            [Theory]
            [InlineData(0.3)]
            public void ThenReturnConstantWithTolerance(decimal tolerance)
            {
                ConstantExpression = $"{SomeString}+-{tolerance}";
                ArrangeAndAct();
                Assert.Equal(tolerance, ReturnValue.Tolerance);
            }
        }

        public class GivenFullfeaturedConstantExpression : GivenNoValue
        {
            [Theory]
            [InlineData("TheName", 2.34, ConstantType.Decimal, ConstantType.Int, Constraint.Mandatory)]
            public void ThenReturnFullfeaturedConstant(
                string name,
                decimal tolerance,
                ConstantType from, 
                ConstantType to, 
                Constraint constraint)
            {
                ConstantExpression = $":{name}+-{tolerance}:{from}->{to} {constraint}";
                ArrangeAndAct();
                Assert.Equal(name, ReturnValue.Name);
                Assert.Equal(tolerance, ReturnValue.Tolerance);
                Assert.Equal(from, ReturnValue.Type);
                Assert.Equal(new[] { to}, ReturnValue.Conversions);
                Assert.Equal(constraint, ReturnValue.Constraint);
            }
        }
    }
}