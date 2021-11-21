using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary.CodeStructures
{
    public struct ConstructorData
    {
        public string name { get; }
        public Dictionary<string, string> parameters { get; }

        public ConstructorData(string name, Dictionary<string, string> parameters)
        {
            this.name = name;
            this.parameters = parameters;
        }
    }
}
