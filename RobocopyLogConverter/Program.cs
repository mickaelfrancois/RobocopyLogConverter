using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace RobocopyLogConverter
{
    class Program
    {
        static int Main(string[] args)
        {           
            var logConverter = new LogConverter();
            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            
            var fileOption = commandLineApplication.Option("-f | --file", "Robocopy log file.", CommandOptionType.SingleValue);
            var directoryOption = commandLineApplication.Option("-d | --directory", "Robocopy log folder.", CommandOptionType.SingleValue);
            var outputOption = commandLineApplication.Option("-o | --output", "Output JSON to file.", CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                if (fileOption.HasValue())
                {
                    var result = logConverter.ConvertFileToJson(fileOption.Value());
                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

                    if (outputOption.HasValue())
                        GenerateFileOutput(jsonData, outputOption.Value());
                    else
                        Console.Write(jsonData);

                    return 0;
                }
                else if (directoryOption.HasValue())
                {
                    var result = logConverter.ConvertFolderToJson(directoryOption.Value());
                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

                    if (outputOption.HasValue())
                        GenerateFileOutput(jsonData, outputOption.Value());
                    else
                        Console.Write(jsonData);

                    return 0;
                }
                else
                    return 1;
            });

            return commandLineApplication.Execute(args);
        }


        private static void GenerateFileOutput(string jsonData, string file)
        {
            File.WriteAllText(file, jsonData, System.Text.Encoding.UTF8);
        }
    }
}
