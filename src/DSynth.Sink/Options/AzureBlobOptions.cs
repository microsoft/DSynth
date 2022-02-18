/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DSynth.Sink.Options
{
    public class AzureBlobOptions : SinkOptionsBase, IValidatableObject
    {
        [JsonProperty("connectionString")]
        private string _connectionString { get; set; }

        [JsonIgnore]
        public string ConnectionString => EvaluateAndRetrieveValue(_connectionString);

        [JsonProperty("storageEndpoint")]
        public string StorageEndpoint { get; set; }

        [JsonProperty("managedIdentityClientId")]
        public string ManagedIdentityClientId { get; set; }

        [JsonProperty("blobContainerName")]
        public string BlobContainerName { get; set; }

        [JsonProperty("subfolderPattern")]
        public string SubfolderPattern { get; set; }

        [JsonProperty("filenamePattern")]
        public string FilenamePattern { get; set; }

        [JsonProperty("filenameSuffix")]
        public string FilenameSuffix { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
            {
                yield return result;
            }

            if (String.IsNullOrEmpty(ConnectionString) && (String.IsNullOrEmpty(StorageEndpoint) || String.IsNullOrEmpty(ManagedIdentityClientId)))
            {
                yield return new ValidationResult($"ConnectionString or (StorageEndpoint and ManagedIdentityClientId) or env var {_connectionString} must be provided");
            }
            if (String.IsNullOrEmpty(BlobContainerName))
            {
                yield return new ValidationResult("BlobContainerName cannot be null or empty");
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