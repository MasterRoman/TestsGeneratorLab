using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary.CodeStructures
{
    public struct CodeData
    {
        public List<ClassData> classesData { get;}

        public CodeData(List<ClassData> classesData)
        {
            this.classesData = classesData;
        }
    }
}
