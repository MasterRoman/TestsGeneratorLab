using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary.CodeStructures
{
    public struct ClassData
    {
        public string name { get; }
        public List<ConstructorData> constructors { get; }
        public List<MethodData> methods { get; }

        public ClassData(string name, List<ConstructorData> constructors, List<MethodData> methods)
        {
            this.name = name;
            this.constructors = constructors;
            this.methods = methods;
        }
    }
}
