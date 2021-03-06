/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using DSynth.Common.Utilities;
using Newtonsoft.Json;

namespace DSynth.Engine.TokenHandlers
{
    public class CoordinateHandler : TokenHandlerBase
    {
        private double _size;
        private int _precision;
        private double _longMin;
        private double _longMax;
        private double _latMin;
        private double _latMax;
        private Format _format;

        public CoordinateHandler(TokenDescriptor tokenDescriptor, string providerName)
            : base(tokenDescriptor, providerName, null)
        {
            switch (SourceType)
            {
                case EngineSourceType.Polygon:
                    ValidateAndSetParameters(tokenDescriptor);
                    break;
                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }
        }

        public override string GetReplacementValue()
        {
            string ret = String.Empty;

            switch (SourceType)
            {
                case EngineSourceType.Polygon:
                    ret = GetPolygon();
                    break;
                default:
                    ThrowEngineSourceTypeNotSupportedForHandler();
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Logic used to generate a polygon. We generate the Polygon based on
        /// the random SW coordinates and from there we draw a square counter
        /// clockwise from the SW coordinate based on the _size value.
        /// </summary>
        public string GetPolygon()
        {
            List<double> sw = new List<double>() { GetNextRandomCoord(_longMin, _longMax, Direction.Long), GetNextRandomCoord(_latMin, _latMax, Direction.Lat) };

            var swLong = sw[0];
            var swLat = sw[1];
            List<double> se = new List<double>() { GetNextRandomCoord(swLong, (swLong + _size), Direction.Long), swLat };

            var seLong = se[0];
            var seLat = se[1];
            List<double> ne = new List<double>() { seLong, GetNextRandomCoord(seLat, (seLat + _size), Direction.Lat) };
            
            var neLat = ne[1];
            List<double> nw = new List<double>() { swLong, neLat };
            
            var coordinates = new List<List<double>> { sw, se, ne, nw, sw };

            return GetFormattedReturn(coordinates);
        }

        private string GetFormattedReturn(List<List<double>> coordinates)
        {
            switch (_format)
            {
                case Format.GeoJson:
                    return JsonConvert.SerializeObject(coordinates);
                case Format.String:
                    return String.Join(",", coordinates.Select(x => String.Join(",", x)));
                default:
                    return String.Empty;
            }
        }

        private double GetNextRandomCoord(double min, double max, Direction direction)
        {
            double ret;

            if (direction == Direction.Long && max > _longMax)
            {
                max = _longMax;
            }

            if (direction == Direction.Lat && max > _latMax)
            {
                max = _latMax;
            }

            ret = Math.Round(TokenHandlerHelpers.GetNextRandomDouble(min, max), _precision);

            return ret;
        }

        private void ValidateAndSetParameters(TokenDescriptor tokenDescriptor)
        {
            ValidateParameterCount(Resources.CoordinateHandler.ExpectedParameterCount);


            string boundingBoxString = tokenDescriptor.TokenParameters[2];
            List<double[]> boundingBoxArray = JsonConvert.DeserializeObject<List<double[]>>(boundingBoxString);
            if (boundingBoxArray.Count != 4)
            {
                ThrowParameterException(boundingBoxString);
            }
            else
            {
                _longMin = boundingBoxArray[0][0];
                _longMax = boundingBoxArray[1][0];
                _latMin = boundingBoxArray[0][1];
                _latMax = boundingBoxArray[2][1];
            }

            string size = tokenDescriptor.TokenParameters[3];
            if (!double.TryParse(size, out _size))
            {
                ThrowParameterException(size);
            }
            else
            {
                // We will determine precision based on the precision of the size parameter.
                // If size is a whole number, we set precision to 0.
                string[] precision = size.Split(".");
                _precision = precision.Length > 1 ? _precision = precision[1].Length : 0;
            }

            string format = tokenDescriptor.TokenParameters[4];
            try
            {
                _format = EnumUtilities.GetEnumValueFromString<Format>(tokenDescriptor.TokenParameters[4]);
            }
            catch (Exception)
            {
                var formattedExMessage = ExceptionUtilities.GetFormattedMessage(Resources.CoordinateHandler.ExInvalidFormat, format, EnumUtilities.GetAllTypesAsCSVString<Format>());
                ThrowParameterException(formattedExMessage);
            }
        }
    }

    internal enum Direction
    {
        Long,
        Lat
    }

    internal enum Format
    {
        GeoJson,
        String
    }
}