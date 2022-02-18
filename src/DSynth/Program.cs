/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DSynth.Extensions;

namespace DSynth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string mode = string.Empty;
            if (args.Length > 0)
            {
                mode = args[0];
            }

            if (mode == "--headless")
            {
                CreateHostBuilderHeadless();
            }
            else
            {
                CreateHostBuilder(args).Build().Run();
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Limits.MaxRequestBodySize = int.MaxValue;
                        serverOptions.ListenAnyIP(5443, o =>
                        {
                            o.UseHttps();
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });

        public static void CreateHostBuilderHeadless() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddDSynth(isHeadless: true)
                        .AddDSynthLoggingAndTelemetry(hostContext.Configuration, isHeadless: true);
                }).RunConsoleAsync().Wait();
    }
}
