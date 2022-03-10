using Applique.LoadTester.Core.Design;

namespace Applique.LoadTester.Logic.Environment;

public class Equation
{
    public ConstantType Type { get; set; }
    public object[] Terms { get; set; }
}