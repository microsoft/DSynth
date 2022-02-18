/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;

namespace DSynth.Sink.Options
{
    public class FileOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonIgnore]
        private string _baseFolderPath { get; set; }

        [JsonProperty("baseFolderPath")]
        public string BaseFolderPath
        {
            get { return _baseFolderPath; }
            set { _baseFolderPath = value.Trim('/'); }
        }

        [JsonProperty("subfolderPattern")]
        public string SubfolderPattern { get; set; }

        [JsonProperty("filenamePattern")]
        public string FilenamePattern { get; set; }

        [JsonProperty("filenameSuffix")]
        public string FilenameSuffix { get; set; }

        [JsonProperty("fileMode")]
        public FileMode FileMode { get; set; } = FileMode.Create;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (String.IsNullOrEmpty(BaseFolderPath))
            {
                yield return new ValidationResult("FolderPath cannot be null or empty");
            }
            if (String.IsNullOrEmpty(FilenamePattern))
            {
                yield return new ValidationResult("FileNamePattern cannot be null or empty");
            }
            if (String.IsNullOrEmpty(FilenameSuffix))
            {
                yield return new ValidationResult("FileNameSuffix cannot be null or empty");
            }
        }
    }
}