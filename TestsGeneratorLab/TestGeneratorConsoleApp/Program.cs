using System;

using TestsGeneratorLibrary;
using TestsGeneratorLibrary.ReadWrite;

namespace TestGeneratorConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser();
            var testsGenerator = new TestsGenerator();

            ReadWriteManager readWriteManager = new ReadWriteManager(parser, testsGenerator);

            var filesPath = new string[] { @"D:\SPPLABS\TestsGeneratorLab\TestsGeneratorLab\TestGeneratorConsoleApp\ClassForTest\1.cs" };
            var destinationPath = @"D:\SPPLABS\TestsGeneratorLab\TestsGeneratorLab\GeneratedTests";

            readWriteManager.generateTests(filesPath, destinationPath, 1, 1, 1);

            Console.ReadLine();
        }
    }
}
