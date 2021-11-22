using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using TestsGeneratorLibrary;
using TestsGeneratorLibrary.CodeStructures;

namespace Tests
{
    [TestFixture]
    class TestClass
    {
        private Parser parser;
        private TestsGenerator testsGenerator;

        private CodeData parsedCode;

        [SetUp]
        public void SetUp()
        {
            this.parser = new Parser();
            this.testsGenerator = new TestsGenerator();
            parseCode();
        }

        private void parseCode()
        {
            var path = @"D:\SPPLABS\TestsGeneratorLab\TestsGeneratorLab\TestGeneratorConsoleApp\ClassForTest\1.cs";

            string code = "";

            using (var reader = new StreamReader(path))
            {
                code = reader.ReadToEnd();
            }
            var parsedCode = parser.parse(code);
            this.parsedCode = parsedCode;

        }

        [Test]
        public void testParser()
        {
            Assert.AreEqual(parsedCode.classesData.Count, 1);

            Assert.AreEqual(parsedCode.classesData[0].name, "Test1");

            Assert.AreEqual(parsedCode.classesData[0].methods[0].returnType, "string"); 
        }


        [Test]
        public void testGenerator()
        {
            var generatedTest = testsGenerator.generateTests(parsedCode);

            var path = @"D:\SPPLABS\TestsGeneratorLab\TestsGeneratorLab\GeneratedTests\Test1Test.cs";

            string generetedCode = "";

            using (var reader = new StreamReader(path))
            {
                generetedCode = reader.ReadToEnd();
            }

            Assert.AreEqual(generetedCode, generatedTest["Test1Test"]);
        }


    }
}
