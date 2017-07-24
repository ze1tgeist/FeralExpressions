

using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Collections.Generic;

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
            Log.LogMessage($"ExpressionGenerationTask: generating *{ExpressionsExtensionPrefix}.cs files");

            var generator = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter(), ExpressionsExtensionPrefix);

            var outputs = new List<ITaskItem>();

            foreach (ITaskItem item in InputItems)
            {
                string output = generator.GenerateFile(item.ItemSpec);
                if (!String.IsNullOrEmpty(output))
                {
                    var outputItem = new TaskItem(output);
                    outputItem.SetMetadata("DependentUpon", item.ItemSpec);
                    outputItem.SetMetadata("SubType", "ExpressionPartialClass");
                    outputs.Add(outputItem);
                    Log.LogMessage($"ExpressionGenerationTask: generated {output}");
                }
            }

            OutputItems = outputs.ToArray();

            Log.LogMessage($"ExpressionGenerationTask: finished generating\r\n\toutput {outputs.Count}  *{ExpressionsExtensionPrefix}.cs files\r\n\tfrom {InputItems.Length} input *.cs files");
            return true;
        }

    }
}
