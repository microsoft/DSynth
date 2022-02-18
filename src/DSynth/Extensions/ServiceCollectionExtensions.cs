/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Reflection;
using DSynth.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace DSynth.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDSynth(this IServiceCollection services, bool isHeadless)
        {
            services.AddSingleton<IProfileService, ProfileService>();

            if (isHeadless)
            {
                services.AddHostedService<DSynthService>();
            }
            else
            {
                services.AddSingleton<IDSynthService, DSynthService>();
            }

            return services;
        }

        public static IServiceCollection AddDSynthLoggingAndTelemetry(this IServiceCollection services, IConfiguration configuration, bool isHeadless)
        {
            // Disable adaptive sampling to allow all logs to be sent to App Insights
            if (isHeadless)
            {
                services.AddApplicationInsightsTelemetryWorkerService(c => { c.EnableAdaptiveSampling = false; });
            }
            else
            {
                services.AddApplicationInsightsTelemetry(c => { c.EnableAdaptiveSampling = false; });
            }

            services.Configure<TelemetryConfiguration>(c =>
            {
                string dSynthEnvironmentName = configuration.GetValue<string>("dSynthEnvironmentName");
                c.TelemetryInitializers.Add(new DSynthTelemetryInitializer(dSynthEnvironmentName));
            });

            // Disable Application Insights tracing messages
            Microsoft.ApplicationInsights.Extensibility.Implementation.TelemetryDebugWriter.IsTracingDisabled = true;

            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddApplicationInsights();
            });

            return services;
        }

        public static IServiceCollection AddEndpointDependencies(this IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddDefaultPolicy(p =>
                {
                    p.AllowAnyOrigin();
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                });
            });

            services.AddApiVersioning();

            object p = services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                "v1", new OpenApiInfo
                {
                    Title = "DSynth",
                    Version = "v1"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddControllers(
              options =>
              {
                  options.EnableEndpointRouting = false;
              }).AddNewtonsoftJson();

            // Add Newtonsoft Json support for swagger, this causes it to correctly interpret 
            // the [JsonProperty] attributes on types so the casing in the automatically generated
            // swagger documentation matches the responses.
            services.AddSwaggerGenNewtonsoftSupport();

            // Enable large profile uploads via API endpoint
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            return services;
        }
    }
}