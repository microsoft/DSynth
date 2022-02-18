/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DSynth.Common.Utilities;
using DSynth.Engine.TokenHandlers;

namespace DSynth.Engine
{
    public class TemplateData
    {
        public IDictionary<string, string> Metadata { get; set; }
        public string Template { get; set; }
        public object Collection(string collectionsFileName)
            => GetCollectionByFileName(collectionsFileName);
        private readonly Regex _regex;
        private Dictionary<string, object> _collections { get; set; }
        private List<Func<string>> _templateSegments;

        public TemplateData(IDictionary<string, string> metadata, string template, ref Dictionary<string, object> collections)
        {
            Metadata = metadata;
            Template = template;
            _collections = collections;
            _regex = new Regex(Resources.EngineBase.ReplacementTokenRegexPattern);
        }

        /// <summary>
        /// Splices together the template segments, producing a completely built template
        /// </summary>
        public string RenderTemplate()
        {
            return String.Concat(_templateSegments.Select(x => x()));
        }

        private object GetCollectionByFileName(string collectionsFileName)
        {
            if (!_collections.TryGetValue(collectionsFileName, out object ret))
            {
                string formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                    Resources.TemplateData.ExUnableToGetCollectionByName,
                    collectionsFileName);

                throw new TemplateDataException(formattedExMessage);
            }

            return ret;
        }

        /// <summary>
        /// Breaks the template down into a list of functions. We replace template tokens
        /// with a function that will replace the token when BuildTemplate gets called.
        /// This keeps the template static and we do not have to parse it every time. This
        /// is a 1 time operation.
        /// </summary>
        public void BuildTemplateSegments(string callingProviderName)
        {
            // Prepare the template for splitting by wrapping the template tokens
            // with additional tokens that we can then split on. This will allow
            // us to construct the list of string and Func segments.
            var template = Template.Replace("{{", "|^|{{").Replace("}}", "}}|^|");
            var rawTemplateSegments = template.Split("|^|");
            _templateSegments = new List<Func<string>>();

            // Build template segments, check if segment matches the template token
            // pattern and if so, add a token replacement Func to the template
            // segments list, else just add a Func to return the segment string.
            foreach (var rawTemplateSegment in rawTemplateSegments)
            {
                if (_regex.IsMatch(rawTemplateSegment))
                {
                    _templateSegments.Add(() =>
                    {
                        using (TokenDescriptor tokenDescriptor = new TokenDescriptor(rawTemplateSegment))
                        using (ITokenHandler handler = TokenHandlerFactory.GetHandler(tokenDescriptor, callingProviderName, this))
                        {
                            return handler.GetReplacementValue();
                        }
                    });
                }
                else
                {
                    _templateSegments.Add(() => rawTemplateSegment);
                }
            }
        }
    }
}