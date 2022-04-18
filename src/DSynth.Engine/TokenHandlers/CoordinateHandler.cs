using System;
using System.Collections.Generic;
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
            List<double> sw = new List<double>() { GetNextRandomCoord(_longMin, _longMax, Direction.Long, Position.SW), GetNextRandomCoord(_latMin, _latMax, Direction.Lat, Position.SW) };
            List<double> se = new List<double>() { GetNextRandomCoord(sw[0], double.MaxValue, Direction.Long, Position.SE), sw[1] };
            List<double> ne = new List<double>() { se[0], GetNextRandomCoord(se[1], double.MaxValue, Direction.Lat, Position.NE) };
            List<double> nw = new List<double>() { sw[0], ne[1] };
            var coordinates = new List<List<double>> { sw, se, ne, nw, sw };
            
            return JsonConvert.SerializeObject(coordinates);
        }

        private double GetNextRandomCoord(double min, double max, Direction direction, Position position)
        {
            double ret = double.MinValue;

            if (max == double.MaxValue)
            {
                max = min + _size;

                if (direction == Direction.Long && max > _longMax)
                {
                    max = _longMax;
                }

                if (direction == Direction.Lat && max > _latMax)
                {
                    max = _latMax;
                }
            }

            if (min == double.MinValue)
            {
                min = max - _size;

                if (direction == Direction.Lat && min > _latMax)
                {
                    min = _latMax;
                }
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

            string precision = tokenDescriptor.TokenParameters[4];
            if (!int.TryParse(precision, out _precision))
            {
                ThrowParameterException(precision);
            }
        }
    }

    internal enum Direction
    {
        Long,
        Lat
    }

    internal enum Position
    {
        SE,
        SW,
        NE,
        NW
    }
}