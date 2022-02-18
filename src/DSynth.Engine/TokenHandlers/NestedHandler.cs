/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DSynth.Engine.TokenHandlers
{
    public class NestedHandler : TokenHandlerBase
    {
        private bool _disposed;
        private int _minRange;
        private int _maxRange;
        private TemplateData _templateData;
        private readonly Regex _regex;

        public NestedHandler(TokenDescriptor tokenDescriptor, string providerName)
            : base(tokenDescriptor, providerName, templateData: null)
        {
            _regex = new Regex(Resources.EngineBase.ReplacementTokenRegexPattern);

            switch (SourceType)
            {
                case EngineSourceType.Json:
                    ValidateAndSetParameters(tokenDescriptor);
                    break;

                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }
        }

        public override string GetReplacementValue()
        {
            string ret;
            switch (SourceType)
            {
                case EngineSourceType.Json:
                    ret = GetNextNestedValue();
                    break;

                default:
                    ret = default;
                    break;
            }

            return ret;
        }
        private string GetNextNestedValue()
        {
            int nestedCount = TokenHandlerHelpers.GetNextRandomInt(_minRange, _maxRange);
            List<string> nestedValues = new List<string>();

            for (int n = 0; n < nestedCount; n++)
            {                
                nestedValues.Add(_templateData.RenderTemplate());
            }

            return String.Join(",", nestedValues);
        }

        private void ValidateAndSetParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.NestedHandler.ExpectedParameterCount);

            var nestedTemplateName = tokenDescriptor.TokenParameters[2];
            if (String.IsNullOrWhiteSpace(nestedTemplateName))
            {
                ThrowParameterException(nestedTemplateName);
            }

            _templateData = TemplateDataProvider.Instance.GetTemplateData(ProviderName, nestedTemplateName);

            string range = tokenDescriptor.TokenParameters[3];
            string[] ranges = range.Split(Resources.TokenHandlerBase.RangeDelimeter);
            if (ranges.Length != 2)
            {
                ThrowParameterException(range);
            }
            try
            {
                _minRange = Convert.ToInt32(ranges[0]);
                _maxRange = Convert.ToInt32(ranges[1]);
            }
            catch (Exception)
            {
                ThrowParameterException(range);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _templateData = null;
            _disposed = true;

            base.Dispose(disposing);
        }
    }
}