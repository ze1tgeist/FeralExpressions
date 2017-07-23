

using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace FeralExpressions.Generator
{
    public class ExpressionGenerationTask : Task
    {
        public ITaskItem[] InputItems { get; set; }
        public ITaskItem[] OutputItems { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Hello World!");

            return true;
        }
    }
}
