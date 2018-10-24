using Microsoft.Extensions.CommandLineUtils;
using System;

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

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                if (fileOption.HasValue())
                {
                    var result = logConverter.ConvertFileToJson(fileOption.Value());
                    Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
                    return 0;
                }
                else if (directoryOption.HasValue())
                {
                    var result = logConverter.ConvertFolderToJson(directoryOption.Value());
                    Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
                    return 0;
                }
                else
                    return 1;
            });

            return commandLineApplication.Execute(args);
        }
    }
}
