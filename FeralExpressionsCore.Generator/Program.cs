using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore.Generator
{
    class Program
    {
        public static void Main(string[] args)
        {
            IEnumerable<string> rest = args;
            try
            {
                var expressionsExtensionPrefix = rest.First();
                rest = rest.Skip(1);

                IEnumerable<string> inputFileNames = Enumerable.Empty<string>();
                if (rest.Any() && rest.Skip(1).Any() && rest.First().Equals("-f", StringComparison.CurrentCultureIgnoreCase))
                {
                    rest = rest.Skip(1); // skip over the -f
                    var fileNameOfListOfInputFileNames = rest.First();
                    rest = rest.Skip(1);

                    inputFileNames = inputFileNames.Concat(ReadLinesFromFile(fileNameOfListOfInputFileNames));
                }
                else
                {

                }

                inputFileNames = inputFileNames.Concat(rest);

                var runner = new GeneratorRunner(expressionsExtensionPrefix, msg => { });

                var outputFileNames = runner.RunGenerator(inputFileNames);

                foreach (var fileName in outputFileNames)
                {
                    System.Console.Out.WriteLine(fileName);
                }

            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex.ToString());
            }
        }

        private static IEnumerable<string> ReadLinesFromFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        yield return line.Trim();
                    }
                    line = reader.ReadLine();
                }
            }

        }
    }
}
