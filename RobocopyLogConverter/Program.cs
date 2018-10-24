using System;

namespace RobocopyLogConverter
{
    class Program
    {
        static void Main(string[] args)
        {           
            var logConverter = new LogConverter();
            var result = logConverter.ConvertToJson(args[0]);

            Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
