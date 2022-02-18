/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using DSynth.Extensions;
using DSynth.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DSynth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointDependencies();
            services.AddDSynthLoggingAndTelemetry(Configuration, isHeadless: false);
            services.AddDSynth(isHeadless: false);
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IDSynthService dSynth,
            IHostApplicationLifetime applicationLifetime,
            TelemetryClient telemetryClient,
            ILogger<Startup> logger)
        {
            app.UseEndpointDependencies(env.IsDevelopment());

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                dSynth.StartAsync().GetAwaiter().GetResult();
            });
        }
    }
}
