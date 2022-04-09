using System.Threading;
using Microsoft.Extensions.Logging;

namespace DSynth.Engine.Engines
{
    public class GremlinEngine : EngineBase
    {
        public GremlinEngine(string templateName, string callingProviderName, CancellationToken token, ILogger logger = null) 
            : base(templateName, callingProviderName, token, logger)
        {
        }

        public override object BuildPayload()
        {
            return GetTemplateWithReplacedTokens();
        }
    }
}