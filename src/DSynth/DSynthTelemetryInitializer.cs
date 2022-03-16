/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Reflection;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace DSynth
{
    public class DSynthTelemetryInitializer : ITelemetryInitializer
    {
        private const string _languageKey = "Language";
        private const string _environmentNameKey = "EnvironmentName";
        private const string _languageValue = "C#";
        private const string _languageVersionKey = "LanguageVersion";
        private readonly string _environmentNameValue;
        private static readonly string _roleName = $"{Assembly.GetEntryAssembly().GetName().Name}-{Assembly.GetEntryAssembly().GetName().Version}";
        private static readonly string _version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        public DSynthTelemetryInitializer(string dSynthEnvironmentName)
        {
            dSynthEnvironmentName = String.IsNullOrWhiteSpace(dSynthEnvironmentName) ? null : dSynthEnvironmentName;

            _environmentNameValue = Environment.GetEnvironmentVariable("DATASYNTH_ENVIRONMENT_NAME")
                ?? Environment.GetEnvironmentVariable("HOSTNAME")
                ?? dSynthEnvironmentName ?? "EnvironmentNameNotSet";
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry == null)
            {
                throw new ArgumentNullException(nameof(telemetry));
            }

            telemetry.Context.GlobalProperties[_languageKey] = _languageValue;
            telemetry.Context.GlobalProperties[_environmentNameKey] = _environmentNameValue;
            telemetry.Context.GlobalProperties[_languageVersionKey] = Environment.Version.ToString();
            telemetry.Context.Cloud.RoleName = _roleName;
            telemetry.Context.Component.Version = _version;
        }
    }
}