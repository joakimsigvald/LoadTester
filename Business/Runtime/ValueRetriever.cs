﻿using Applique.LoadTester.Business.External;
using System;

namespace Applique.LoadTester.Business.Design
{
    public class ValueRetriever
    {
        public const string Int = "int";
        public const string Decimal = "decimal";
        public const string DateTime = "date";
        public const string String = "string";
        public const string Seed = "seed";
        private readonly IFileSystem _fileSystem;
        private readonly TestSuite _testSuite;

        public ValueRetriever(IFileSystem fileSystem, TestSuite testSuite)
        {
            _fileSystem = fileSystem;
            _testSuite = testSuite;
        }

        public object GetValue(Constant constant)
            => constant.Type == Seed ? GetSeed(constant) : ValueOf(constant);

        public static object ValueOf(Constant constant)
        {
            var value = ParseValue(constant);
            var from = constant.Type;
            foreach (var conversion in constant.Conversions)
            {
                value = Cast(value, from, conversion);
                from = conversion;
            }
            return value;
        }

        public static string GetType(object obj)
            => obj is int? ? Int
            : obj is decimal? ? Decimal
            : obj is DateTime? ? DateTime
            : String;

        private object GetSeed(Constant constant)
        {
            var seed = LoadSeed(constant) ?? int.Parse(constant.Value);
            SaveSeed(constant, seed + 1);
            return seed;
        }

        private static object Cast(object value, string from, string to)
            => to switch
            {
                Int when from == Decimal => decimal.ToInt32((decimal)value),
                _ => throw new InvalidOperationException($"Cannot cast {value} from {from} to {to}")
            };

        private static object ParseValue(Constant constant)
            => constant.Type switch
            {
                DateTime => System.DateTime.Parse(constant.Value),
                Int => int.Parse(constant.Value),
                Decimal => decimal.Parse(constant.Value),
                _ => constant.Value,
            };

        private void SaveSeed(Constant constant, int seed)
            => _fileSystem.Write(GetSeedPath(constant), seed);

        private int? LoadSeed(Constant constant)
            => _fileSystem.Exists(GetSeedPath(constant)) 
            ? _fileSystem.Read<int>(GetSeedPath(constant)) 
            : null;

        private string GetSeedPath(Constant constant) => $"{_testSuite.Name}_{constant.Name}_Seed";
    }
}