using Applique.LoadTester.Core.Design;
using Applique.WhenGivenThen.Core;
using System;
using Xunit;
using static Applique.LoadTester.Domain.Design.ConstantFactory;
using static Applique.LoadTester.Test.TestData;

namespace Applique.LoadTester.Environment.Test.Bindings
{
    public abstract class WhenCreate : TestStatic<Constant>
    {
        protected string ConstantExpression;
        protected string ConstantValue;

        protected override void Act() => CollectResult(() => Create(ConstantExpression, ConstantValue));

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
                Act();
                Assert.Null(Result);
            }
        }

        public class GivenEmptyExpression : GivenNoValue
        {
            [Fact]
            public void ThenReturnNull()
            {
                ConstantExpression = string.Empty;
                Act();
                Assert.Null(Result);
            }
        }

        public class GivenName : GivenNoValue
        {
            [Fact]
            public void ThenReturnNamedStringConstant()
            {
                ConstantExpression = SomeString;
                Act();
                Assert.Equal(SomeString, Result.Name);
                Assert.Equal(ConstantType.String, Result.Type);
            }
        }

        public class GivenColonName : GivenNoValue
        {
            [Fact]
            public void ThenReturnOvershadowingConstant()
            {
                ConstantExpression = $":{SomeString}";
                Act();
                Assert.True(Result.Overshadow);
            }
        }

        public class GivenNameColonUnrecogniced : GivenNoValue
        {
            [Fact]
            public void ThenThrowArgumentException()
            {
                ConstantExpression = $"{SomeString}:int";
                Assert.Throws<ArgumentException>(Act);
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
                Act();
                Assert.Equal(type, Result.Type);
            }
        }

        public class GivenNameSpaceUnrecogniced : GivenNoValue
        {
            [Fact]
            public void ThenThrowArgumentException()
            {
                ConstantExpression = $"{SomeString} {AnotherString}";
                Assert.Throws<ArgumentException>(Act);
            }
        }

        public class GivenNameSpaceConstraint : GivenNoValue
        {
            [Theory]
            [InlineData(Constraint.Mandatory)]
            public void ThenReturnConstrainedConstant(Constraint constraint)
            {
                ConstantExpression = $"{SomeString} {constraint}";
                Act();
                Assert.Equal(constraint, Result.Constraint);
            }
        }

        public class GivenTypeCast : GivenNoValue
        {
            [Fact]
            public void ThenReturnConstantWithConversion()
            {
                ConstantExpression = $"{SomeString}:Decimal->Int";
                Act();
                Assert.Equal(ConstantType.Decimal, Result.Type);
                Assert.Equal(new[] { ConstantType.Int }, Result.Conversions);
            }
        }

        public class GivenTolerance : GivenNoValue
        {
            [Theory]
            [InlineData(0.3)]
            public void ThenReturnConstantWithTolerance(decimal tolerance)
            {
                ConstantExpression = $"{SomeString}+-{tolerance}";
                Act();
                Assert.Equal(tolerance, Result.Tolerance);
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
                Act();
                Assert.Equal(name, Result.Name);
                Assert.Equal(tolerance, Result.Tolerance);
                Assert.Equal(from, Result.Type);
                Assert.Equal(new[] { to}, Result.Conversions);
                Assert.Equal(constraint, Result.Constraint);
            }
        }
    }
}