﻿using System;

namespace Applique.LoadTester.Core.Design;

public class Constant
{
    public string Name { get; set; }
    public string Value { get; set; }
    public ConstantType Type { get; set; } = ConstantType.String;
    public bool Overshadow { get; set; }
    public decimal Tolerance { get; set; }
    public ConstantType[] Conversions { get; set; } = Array.Empty<ConstantType>();
    public Constraint Constraint { get; set; }
}