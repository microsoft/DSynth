/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DSynth.Common.JsonSerializers
{
    public class ISO8601DateTimeConverter : IsoDateTimeConverter
    {
        private const string _iso8601TimeFormatWithMsPrecision = "yyyy-MM-ddTHH:mm:ss.fff";

        public ISO8601DateTimeConverter()
        {
            base.DateTimeFormat = _iso8601TimeFormatWithMsPrecision;
        }
    }

    public class ISO8601DateTime : JsonConverter
    {
        private const string _iso8601TimeFormatWithMsPrecision = "yyyy-MM-ddTHH:mm:ss.fff";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var dt = (DateTime)value;
            var tsString = dt.ToString(_iso8601TimeFormatWithMsPrecision,
                CultureInfo.InvariantCulture);
            serializer.Serialize(writer, tsString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = serializer.Deserialize<String>(reader);
            return DateTime.ParseExact(value,
                                   _iso8601TimeFormatWithMsPrecision,
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.RoundtripKind);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }
    }
}