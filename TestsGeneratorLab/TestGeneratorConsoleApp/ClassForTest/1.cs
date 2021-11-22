using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorConsoleApp.ClassForTest
{
    class Test1
    {
        public Test1(IList<int> list,string str) { }

        public string func1(string a,string b)
        {
            return a + b;
        }
    }
}
