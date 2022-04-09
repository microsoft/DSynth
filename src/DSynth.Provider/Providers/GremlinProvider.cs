using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DSynth.Common.Models;
using DSynth.Common.Options;
using DSynth.Engine;
using Microsoft.Azure.CosmosDB.BulkExecutor.Graph.Element;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DSynth.Provider.Providers
{
    public class GremlinProvider : ProviderBase
    {
        private readonly List<object> _payloadObject;
        public override PayloadPackage Package => PreparePayloadPackage();

        public GremlinProvider(DSynthProviderOptions options, ILogger logger, CancellationToken token)
            : base(options, EngineType.Gremlin, logger, token)
        {
            _payloadObject = new List<object>();
        }

        private PayloadPackage PreparePayloadPackage()
        {
            try
            {
                _payloadObject.Clear();
                object nextPayload = ProviderQueue.Dequeue(out long payloadCount);

                if (nextPayload is System.String)
                {
                    string payloadString = (string)nextPayload;
                    dynamic payloadDynamic = JsonConvert.DeserializeObject<dynamic>(payloadString);
                    
                    _payloadObject.Add(GetGremlinObject(payloadDynamic));
                }
                else if (nextPayload is List<object>)
                {
                    List<object> payload = (List<object>)nextPayload;
                    List<object> payloadObject = payload.Select(i => GetGremlinObject(JsonConvert.DeserializeObject<dynamic>(i.ToString()))).ToList();

                    _payloadObject.AddRange(payloadObject);
                }
                
                var serializedPayload = JsonConvert.SerializeObject(_payloadObject);
                return PayloadPackage.CreateNew(Encoding.UTF8.GetBytes(serializedPayload), payloadCount, null, _payloadObject);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
        }

        private object GetGremlinObject(dynamic val)
        {
            switch (val.type.Value)
            {
                case "vertex":
                    return ToGremlinVertex(val);
                case "edge":
                    return ToGremlinEdge(val);
                default:
                    return new {};
            }
        }

        private GremlinVertex ToGremlinVertex(dynamic val)
        {
            GremlinVertex gv = new GremlinVertex(val.id.Value, val.label.Value);
            foreach (KeyValuePair<string, string> kvp in val.properties.ToObject<Dictionary<string, string>>())
            {
                gv.AddProperty(kvp.Key, kvp.Value);
            }

            return gv;
        }

        private GremlinEdge ToGremlinEdge(dynamic val)
        {
            GremlinEdge ge = new GremlinEdge(val.id.Value, val.label.Value, val.outVertexId.Value, val.inVertexId.Value, val.outVertexLabel.Value, val.inVertexLabel.Value, val.outVertexPartitionKey.Value, val.inVertexPartitionKey.Value);
            foreach (KeyValuePair<string, string> kvp in val.properties.ToObject<Dictionary<string, string>>())
            {
                ge.AddProperty(kvp.Key, kvp.Value);
            }

            return ge;
        }
    }
}