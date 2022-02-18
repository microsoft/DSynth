/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSynth.Sink.Options;
using System.Text;

namespace DSynth.Sink.Sinks
{
    public class Console : SinkBase<ConsoleOptions>
    {
        public Console(string providerName, ConsoleOptions options, ILogger logger, CancellationToken token)
            : base(providerName, options, logger, token)
        {
        }

        internal override async Task RunAsync(byte[] payload)
        {
            if (OptionsOverrides.TryGetValue(Resources.SinkBase.HeaderKey, out string header))
            {
                payload = Encoding.UTF8.GetBytes(header).Concat(payload).ToArray();
            }

            string stringPayload = Encoding.UTF8.GetString(payload);
            if (Options.WriteToLog)
            {
                await Task.Run(() => Logger.LogInformation(stringPayload)).ConfigureAwait(false);
            }
            if (Options.WriteToConsole)
            {
                await Task.Run(() => System.Console.WriteLine($"{stringPayload}\n")).ConfigureAwait(false);
            }
        }
    }
}