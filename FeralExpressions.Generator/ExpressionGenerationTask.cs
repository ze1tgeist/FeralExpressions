

using System;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using System.Collections;

namespace FeralExpressions.Generator
{
    public class ExpressionGenerationTask : ITask
    {
        public ExpressionGenerationTask()
        {
            ExpressionsExtensionPrefix = ".expressions";
        }
        public string ExpressionsExtensionPrefix { get; set; }
        public ITaskItem[] InputItems { get; set; }
        [Output]public ITaskItem[] OutputItems { get; set; }

        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            //Log.LogMessage($"ExpressionGenerationTask: generating *{ExpressionsExtensionPrefix}.cs files");

            var generator = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter(), ExpressionsExtensionPrefix);

            var outputs = new List<ITaskItem>();

            foreach (ITaskItem item in InputItems)
            {
                string output = generator.GenerateFile(item.ItemSpec);
                if (!String.IsNullOrEmpty(output))
                {
                    var outputItem = new TaskItem() { ItemSpec = output };
                    outputItem.SetMetadata("DependentUpon", item.ItemSpec);
                    outputItem.SetMetadata("SubType", "ExpressionPartialClass");
                    outputs.Add(outputItem);
                    //Log.LogMessage($"ExpressionGenerationTask: generated {output}");
                }
            }

            OutputItems = outputs.ToArray();

            //Log.LogMessage($"ExpressionGenerationTask: finished generating\r\n\toutput {outputs.Count}  *{ExpressionsExtensionPrefix}.cs files\r\n\tfrom {InputItems.Length} input *.cs files");
            return true;
        }

        private class TaskItem : ITaskItem
        {
            public string ItemSpec { get; set; }

            public ICollection MetadataNames => metadata.Keys;

            public int MetadataCount => metadata.Count;

            public IDictionary CloneCustomMetadata()
            {
                return new Dictionary<string, string>(metadata);
            }

            public void CopyMetadataTo(ITaskItem destinationItem)
            {
                foreach (var key in metadata.Keys)
                {
                    destinationItem.SetMetadata(key, metadata[key]);
                }
            }

            public string GetMetadata(string metadataName)
            {
                return metadata[metadataName];
            }

            public void RemoveMetadata(string metadataName)
            {
                metadata.Remove(metadataName);
            }

            public void SetMetadata(string metadataName, string metadataValue)
            {
                metadata[metadataName] = metadataValue;
            }

            private Dictionary<string, string> metadata = new Dictionary<string, string>();
        }

    }
}
