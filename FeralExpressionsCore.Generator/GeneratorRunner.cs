using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore.Generator
{
    class GeneratorRunner
    {
        public GeneratorRunner(string expressionsExtensionPrefix, Action<string> logger)
        {
            this.expressionsExtensionPrefix = expressionsExtensionPrefix;
            this.logger = logger;
        }

        public IEnumerable<string> RunGenerator(IEnumerable<string> inputFileNames)
        {
            Log($"ExpressionGenerationTask: generating *{expressionsExtensionPrefix}.cs files");

            var outputs = new List<string>();

            var generator = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter(), expressionsExtensionPrefix);
            foreach (var fileName in inputFileNames)
            {
                string outputFileName = generator.GenerateFile(fileName);
                if (outputFileName != null)
                {
                    Log($"ExpressionGenerationTask: generated {outputFileName}");
                    yield return outputFileName;
                }
            }

            Log($"ExpressionGenerationTask: finished generating\r\n\toutput {outputs.Count}  *{expressionsExtensionPrefix}.cs files\r\n\tfrom {inputFileNames.Count()} input *.cs files");

        }

        private void Log(string message)
        {
            logger?.Invoke(message);
        }

        private readonly string expressionsExtensionPrefix;
        private readonly Action<string> logger;
    }
}
