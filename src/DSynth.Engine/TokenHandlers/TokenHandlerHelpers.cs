/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading;

namespace DSynth.Engine.TokenHandlers
{
    public class TokenHandlerHelpers
    {
        private static readonly ThreadLocal<Random> _random
            = new ThreadLocal<Random>(() => new Random());

        public static int GetNextRandomInt(int min, int max)
        {
            return _random.Value.Next(min, max);
        }

        public static double GetNextRandomDouble(double min, double max)
        {
            return _random.Value.NextDouble() * (max - min) + min;
        }

        public static double GetNextRandomNumber(double min, double max)
        {
            if ((min % 1) == 0 && (max % 1) == 0)
            {
                return GetNextRandomInt((int)min, (int)max);
            }
            else
            {
                return GetNextRandomDouble(min, max);
            }
        }

        public static void GetNextBytes(byte[] buffer)
        {
            _random.Value.NextBytes(buffer);
        }

        public static bool ShouldDeviate(int replacementWeight)
        {
            if (replacementWeight == 1)
                return true;

            double diceRoll = TokenHandlerHelpers.GetNextRandomInt(0, 100);
            return diceRoll <= replacementWeight;
        }
    }
}