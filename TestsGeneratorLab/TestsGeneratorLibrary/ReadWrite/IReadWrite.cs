using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary.ReadWrite
{
    public interface IReadWrite
    {
        Task generateTests(string[] filePaths, string destinationPath, int maxRead, int maxWrite, int maxProcess);
    }
}
