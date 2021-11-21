using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary.CodeStructures
{
    public struct MethodData
    {
        public string name { get; }
        public Dictionary<string, string> parameters { get; }
        public string returnType { get; }

        public MethodData(string name, Dictionary<string, string> parameters, string returnType)
        {
            this.name = name;
            this.parameters = parameters;
            this.returnType = returnType;
        }
    }
}
