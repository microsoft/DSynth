/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using Microsoft.Extensions.Logging;
using DSynth.Common.Options;
using DSynth.Engine;
using System.Threading;
using DSynth.Common.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using ChoETL;

namespace DSynth.Provider.Providers
{
    public class ParquetProvider : ProviderBase
    {
        public override PayloadPackage Package => PreparePayloadPackage();
        private readonly List<ExpandoObject> _payload;

        public ParquetProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.JSON, logger, token)
        {
            _payload = new List<ExpandoObject>();
        }

        private PayloadPackage PreparePayloadPackage()
        {
            try
            {
                _payload.Clear();
                object nextPayload = ProviderQueue.Dequeue(out long payloadCount);

                if (nextPayload is System.String)
                {
                    _payload.Add(JsonConvert.DeserializeObject<ExpandoObject>((string)nextPayload));
                }
                else if (nextPayload is List<object>)
                {
                    List<object> payload = (List<object>)nextPayload;
                    List<ExpandoObject> payloadStringCollection = payload
                        .Select(i => JsonConvert.DeserializeObject<ExpandoObject>(i.ToString()))
                        .ToList();

                    _payload.AddRange(payloadStringCollection);
                }

                return PayloadPackage.CreateNew(PackageParquet(_payload), payloadCount, "String format is not supported for Parquet");
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }

        public byte[] PackageParquet(List<ExpandoObject> payload)
        {
            ChoParquetRecordConfiguration config = new ChoParquetRecordConfiguration();
            config.CompressionMethod = Parquet.CompressionMethod.Snappy;

            return ChoParquetWriter.SerializeAll(payload, config);
        }
    }
}