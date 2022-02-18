/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DSynth.Common.Utilities;
using DSynth.Common;
using DSynth.Common.Extensions;
using CsvHelper;
using System.Globalization;
using System.Collections.Concurrent;

namespace DSynth.Engine
{
    public class TemplateDataProvider : IDisposable
    {
        private static TemplateDataProvider _instance = null;
        private static ConcurrentDictionary<string, TemplateData> _templateDataDict;
        private static Random _rand = new Random();
        private static Dictionary<string, string> _templatesDict;
        private static Dictionary<string, object> _collectionsDict;
        private static readonly Object lockObject = new object();

        public static TemplateDataProvider Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new TemplateDataProvider();
                    }
                    return _instance;
                }
            }
        }

        public static void Clear()
        {
            lock (lockObject)
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                }
            }
        }

        private TemplateDataProvider()
        {
            Initialize();
        }

        private static void Initialize()
        {
            _templateDataDict = new ConcurrentDictionary<string, TemplateData>();
            _templatesDict = LoadTemplates();
            _collectionsDict = LoadCollections();
        }

        public TemplateDataProvider BuildTemplateSegments(string callingProviderName)
        {
            foreach (var template in _templatesDict)
            {
                var metaData = ParseTemplateStructure(template.Value, out string templateWithoutMetadata);
                var templateData = new TemplateData(metaData, templateWithoutMetadata, ref _collectionsDict);
                templateData.BuildTemplateSegments(callingProviderName);
                _templateDataDict.TryAdd(GetTemplateDataDictKey(callingProviderName, template.Key), templateData);
            }

            return this;
        }

        public TemplateData GetTemplateData(string callingProviderName, string templateName)
        {
            return _templateDataDict[GetTemplateDataDictKey(callingProviderName, templateName)];
        }

        private static Dictionary<string, object> LoadCollections()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            List<Exception> exceptions = new List<Exception>();

            // Load JSON
            var files = Directory.GetFiles(CommonResources.CollectionsRootFolderPath)
                .Where(f => f.Contains(Resources.TemplateDataProvider.JsonCollectionPattern, StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in files)
            {
                try
                {
                    var content = JsonUtilities.ReadFileAsync(file).Result.JObjectContents;
                    ret.Add(ParseDictKeyFromFilePath(file), content);
                }
                catch (Exception ex)
                {
                    var formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                        Resources.TemplateDataProvider.ExUnableToLoadCollections,
                        file,
                        ex.Message);

                    TemplateDataException exception = new TemplateDataException(formattedExMessage);
                    exceptions.Add(exception);
                }
            }

            // Load CSV
            files = Directory.GetFiles(CommonResources.CollectionsRootFolderPath)
                .Where(f => f.Contains(Resources.TemplateDataProvider.CsvCollectionPattern, StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in files)
            {
                try
                {
                    using (var streamReader = new StreamReader(file))
                    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        var records = csvReader.GetRecords<object>().ToArray();
                        Shuffle(records);
                        ret.Add(ParseDictKeyFromFilePath(file), records);
                    }
                }
                catch (Exception ex)
                {
                    var formattedExMessage = ExceptionUtilities.GetFormattedMessage(
                        Resources.TemplateDataProvider.ExUnableToLoadCollections,
                        file,
                        ex.Message);

                    TemplateDataException exception = new TemplateDataException(formattedExMessage);
                    exceptions.Add(exception);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return ret;
        }

        private static string GetTemplateDataDictKey(string callingProviderName, string templateName)
        {
            return $"{callingProviderName}-{templateName}";
        }

        public static void Shuffle(object[] list)
        {
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = _rand.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private static Dictionary<string, string> LoadTemplates()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            var files = Directory.GetFiles(CommonResources.TemplatesRootFolderPath)
                .Where(f => f.Contains(Resources.TemplateDataProvider.TemplatePattern, StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in files)
            {
                var content = File.ReadAllTextAsync(file).Result;
                ret.Add(ParseDictKeyFromFilePath(file), content);
            }

            return ret;
        }

        private static string ParseDictKeyFromFilePath(string filePath)
        {
            var nameSplit = filePath.FixDirectoryPathDelimeter().Split(CommonResources.DirectorySeparatorChar);
            return nameSplit[nameSplit.Length - 1];
        }

        private static IDictionary<string, string> ParseTemplateStructure(string templateString, out string templateWithoutMetadata)
        {
            IDictionary<string, string> templateMetadata = new Dictionary<string, string>();
            MatchCollection rawMetadataLines = Regex.Matches(templateString, Resources.TemplateDataProvider.TemplateMetadataPattern);

            // Remove metadata line from the template, leaving just the template
            Regex regex = new Regex(Resources.TemplateDataProvider.TemplateMetadataPattern);
            templateWithoutMetadata = regex.Replace(templateString, String.Empty);

            foreach (Match rawMetadataLine in rawMetadataLines)
            {
                // Extract metadata, removing MetadataLineToken, splitting
                // on templateMetadataDelimeter and trimming any white space
                string metadataLine = rawMetadataLine.Value.Substring(2);
                string[] splitMetadataLine = metadataLine.Split(Resources.TemplateData.TemplateMetadataDelimeter);
                templateMetadata[splitMetadataLine[0].Trim()] = splitMetadataLine[1].Trim();
            }

            return templateMetadata;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _instance = null;
            _templateDataDict = null;
        }
    }
}