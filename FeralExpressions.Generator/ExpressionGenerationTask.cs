

using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FeralExpressions.Generator
{
    public class ExpressionGenerationTask : Task
    {
        public ExpressionGenerationTask()
        {
            ExpressionsExtensionPrefix = ".expressions";
        }
        public string ExpressionsExtensionPrefix { get; set; }
        public ITaskItem[] InputItems { get; set; }
        [Output]public ITaskItem[] OutputItems { get; set; }

        public override bool Execute()
        {
            var runner = new GeneratorRunner(ExpressionsExtensionPrefix, msg => Log.LogMessage(msg));

            var outputFileNames = runner.RunGenerator(from i in InputItems select i.ItemSpec);
            var outputs = new List<ITaskItem>();
            foreach (var output in outputFileNames)
            {
                var outputItem = new TaskItem(output);
                outputItem.SetMetadata("SubType", "ExpressionPartialClass");
                outputs.Add(outputItem);
            }
            OutputItems = outputs.ToArray();

            return true;
        }

    }
}
