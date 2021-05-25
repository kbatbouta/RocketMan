﻿using System;
using System.IO;
using Verse;

namespace RocketMan
{
    public static class RocketEnvironmentInfo
    {
        private static bool isDevEnvInitialized = false;
        private static bool isDevEnv = false;

        public static bool IsDevEnv
        {
            get
            {
                if (!isDevEnvInitialized)
                {
                    isDevEnvInitialized = true;
                    isDevEnv = File.Exists(Path.Combine(GenFilePaths.ConfigFolderPath, "rocketeer.0102.txt"));
                    Log.Message($"ROCKETMAN: dev environment detected!");
                }
                return isDevEnv;
            }
        }
    }
}
