﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RocketMan
{
    public static class StringUtility
    {
        private const char _A = 'A';
        private const char _Z = 'Z';

        private static readonly Dictionary<string, string> splitingCache = new Dictionary<string, string>();

        public static string SplitStringByCapitalLetters(this string inputString)
        {
            if (splitingCache.TryGetValue(inputString, out string outputString))
            {
                return outputString;
            }
            outputString = string.Empty;
            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString[i] >= _A && inputString[i] <= _Z)
                {
                    outputString += " ";
                    outputString += inputString[i];
                    i++;
                    while (i < inputString.Length && inputString[i] >= _A && inputString[i] <= _Z)
                    {
                        outputString += inputString[i];
                        i++;
                    }
                }
                outputString += inputString[i];
            }
            return splitingCache[inputString] = outputString;
        }
    }
}
