﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.Generator
{
    class Program
    {
        public static void Main(string[] args)
        {
            var expressionsExtensionPrefix = args[0];

            var inputFileNames = args.Skip(1);

            var runner = new GeneratorRunner(expressionsExtensionPrefix, msg => { });

            var outputFileNames = runner.RunGenerator(inputFileNames);

            foreach (var fileName in outputFileNames)
            {
                System.Console.WriteLine(fileName);
            }
        }
    }
}
