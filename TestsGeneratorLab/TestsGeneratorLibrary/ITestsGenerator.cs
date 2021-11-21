using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestsGeneratorLibrary.CodeStructures;

namespace TestsGeneratorLibrary
{
    public interface ITestsGenerator
    {
        Dictionary<string, string> generateTests(CodeData data);
    }

    public interface IParser
    {
        CodeData parse(string code); 
    }
}
