/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Reflection;
using DSynth.Common.JsonSerializers;
using Newtonsoft.Json;

namespace DSynth.Models
{
    public class DSynthStatus
    {
        [JsonProperty("version")]
        public string Version;

        [JsonProperty("startTime")]
        [JsonConverter(typeof(ISO8601DateTimeConverter))]
        public DateTimeOffset? StartTime;

        [JsonProperty("operationalState")]
        public string OperationalState => GetState();

        private OperationalState _state;

        [JsonProperty("uptime")]
        public string Uptime => GetUptime();

        [JsonProperty("activeProfile")]
        public string ActiveProfile;

        public DSynthStatus()
        {
            Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            StartTime = DateTimeOffset.UtcNow;
            _state = Models.OperationalState.Running;
        }

        public string GetUptime()
        {
            string ret = String.Empty;
            if (_state == Models.OperationalState.Running)
            {
                var diffTime = StartTime ?? DateTimeOffset.UtcNow;
                var diff = DateTimeOffset.UtcNow - diffTime;
                ret = diff.ToString(@"d\.hh\:mm\:ss");
            }
            else
            {
                ret = @"0.00:00:00";
            }

            return ret;
        }

        public void Start()
        {
            StartTime = DateTimeOffset.UtcNow;
            _state = Models.OperationalState.Running;
        }

        public void Stop()
        {
            _state = Models.OperationalState.Stopped;
            StartTime = null;
        }

        public string GetState()
        {
            return _state.ToString();
        }

        public void SetRunningProfile(string profileName)
        {
            ActiveProfile = profileName;
        }
    }

    public enum OperationalState
    {
        Running,
        Stopped
    }
}