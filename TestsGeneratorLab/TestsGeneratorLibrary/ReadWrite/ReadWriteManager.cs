using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TestsGeneratorLibrary.ReadWrite
{
    public class ReadWriteManager : IReadWrite
    {

        private IParser parser;
        private ITestsGenerator testsGenerator;

        public ReadWriteManager(IParser parser, ITestsGenerator testsGenerator)
        {
            this.parser = parser;
            this.testsGenerator = testsGenerator;
        }

        public Task generateTests(string[] filePaths, string destinationPath, int maxRead,int maxWrite, int maxProcess)
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            Directory.CreateDirectory(destinationPath);
            var readBlock = new TransformBlock<string, string>
            (
                async path =>
                {
                    using (var reader = new StreamReader(path))
                    {
                        return await reader.ReadToEndAsync();
                    }
                },
                 new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxRead }
            );

            var generateTestsBlock = new TransformManyBlock<string, KeyValuePair<string, string>>
            (
                async sourceCode =>
                {
                    var code = await Task.Run(() => parser.parse(sourceCode));
                    return await Task.Run(() => testsGenerator.generateTests(code));
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxProcess }
            );

    
            var writeBlock = new ActionBlock<KeyValuePair<string, string>>
            (
                async fileName =>
                {

                    using (var writer = new StreamWriter(destinationPath + '\\' + fileName.Key + ".cs"))
                    {
                        await writer.WriteAsync(fileName.Value);
                    }
                },
                 new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxWrite }
            );

            readBlock.LinkTo(generateTestsBlock, linkOptions);
            generateTestsBlock.LinkTo(writeBlock, linkOptions);
            foreach (var filePath in filePaths)
            {
                readBlock.Post(filePath);
            }

            readBlock.Complete();
            return writeBlock.Completion;
        }
    
    }
}
