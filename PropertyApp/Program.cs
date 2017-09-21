using ProcessDocument;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using ProcessDocument.Config;

namespace PropertyApp
{
    class Program
    {

        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppConfig.json");

            Configuration = builder.Build();

            var settings = new AppConfig();
            Configuration.Bind(settings);

            string file = @"C:\Users\ca64374\Desktop\test.jpeg";

            var _pdf = new ProcessHandWritten(file, settings);
            var res = _pdf.ReadHandwrittenTextAsync(file);

            Console.WriteLine(res);
            Console.ReadLine();
        }
    }
}