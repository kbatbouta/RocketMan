﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HugsLib;
using RocketMan.Tabs;
using Verse;

namespace RocketMan
{
    [StaticConstructorOnStartup]
    public class Main : ModBase
    {
        private int debugging = 0;

        private static List<Action> onClearCache;
        private static List<Action> onDefsLoaded;
        private static List<Action> onWorldLoaded;
        private static List<Action> onMapLoaded;
        private static List<Action> onMapComponentsInitializing;
        private static List<Action> onTick;
        private static List<Action> onTickLong;

        public static List<Action> onStaticConstructors;
        public static List<Action> onInitialization;
        public static List<Action> onScribe;

        public static List<Func<ITabContent>> yieldTabContent;

        public static List<Action> onDebugginEnabled;
        public static List<Action> onDebugginDisabled;

        public static void ReloadActions()
        {
            onClearCache = FunctionsUtility.GetActions<OnClearCache>().ToList();
            onDefsLoaded = FunctionsUtility.GetActions<OnDefsLoaded>().ToList();
            onWorldLoaded = FunctionsUtility.GetActions<OnWorldLoaded>().ToList();
            onMapLoaded = FunctionsUtility.GetActions<OnMapLoaded>().ToList();
            onMapComponentsInitializing = FunctionsUtility.GetActions<OnMapComponentsInitializing>().ToList();
            onTick = FunctionsUtility.GetActions<OnTick>().ToList();
            onDebugginEnabled = FunctionsUtility.GetActions<OnDebugginEnabled>().ToList();
            onDebugginDisabled = FunctionsUtility.GetActions<OnDebugginDisabled>().ToList();
            onTickLong = FunctionsUtility.GetActions<OnTickLong>().ToList();
            yieldTabContent = FunctionsUtility.GetFunctions<YieldTabContent, ITabContent>().ToList();
            onScribe = FunctionsUtility.GetActions<Main.OnScribe>().ToList();
            onStaticConstructors = FunctionsUtility.GetActions<Main.OnStaticConstructor>().ToList();
            onInitialization = FunctionsUtility.GetActions<Main.OnInitialization>().ToList();
            Finder.settingsFields = FieldsUtility.GetFields<SettingsField>().ToArray();
        }

        static Main()
        {
            Log.Message($"<color=orange>ROCKETMAN:</color> Version { RocketAssembliesInfo.Version }");
            // ----------------------
            // TODO more stylizations.
            // this is used to stylize the log output of rocketman.
            EditWindow_Log_DoMessagesListing_Patch.PatchEditWindow_Log();
            // -------------------------
            // Offical start of the code.            
            onStaticConstructors = FunctionsUtility.GetActions<OnStaticConstructor>().ToList();
            for (var i = 0; i < onStaticConstructors.Count; i++) onStaticConstructors[i].Invoke();
            // ---------------------------------------
            // TODO implement compatiblity xml support
            //foreach (var mod in ModsConfig.ActiveModsInLoadOrder)
            //{
            //    Log.Message($"{mod.PackageId}, {mod.Name}, {mod.PackageIdPlayerFacing}");
            //}
        }

        public override void MapLoaded(Map map)
        {
            base.MapLoaded(map);
            for (var i = 0; i < onMapLoaded.Count; i++) onMapLoaded[i].Invoke();
        }

        public override void WorldLoaded()
        {
            base.WorldLoaded();
            for (var i = 0; i < onWorldLoaded.Count; i++) onWorldLoaded[i].Invoke();
        }

        public override void MapComponentsInitializing(Map map)
        {
            base.MapComponentsInitializing(map);
            for (var i = 0; i < onMapComponentsInitializing.Count; i++) onMapComponentsInitializing[i].Invoke();
        }

        public override void DefsLoaded()
        {
            for (var i = 0; i < onDefsLoaded.Count; i++) onDefsLoaded[i].Invoke();
            base.DefsLoaded();
            // --------------
            // start loading xml data
            XMLParser.ParseXML();
            // --------------
            // load xml data and parse it
            IgnoreMeDatabase.ParsePrepare();
            IncompatibilityHelper.Prepare();
            NotificationsManager.HookAll();
            // --------------
            // start patching
            RocketPatcher.PatchAll();
            Finder.rocket.PatchAll();
        }

        public override void Tick(int currentTick)
        {
            base.Tick(currentTick);
            CheckDebugging();

            if (currentTick % Finder.universalCacheAge != 0) return;

            for (var i = 0; i < onTick.Count; i++) onTick[i].Invoke();

            if (currentTick % (Finder.universalCacheAge * 5) != 0) return;

            for (var i = 0; i < onTickLong.Count; i++) onTickLong[i].Invoke();
        }

        public void ClearCache()
        {
            for (var i = 0; i < onClearCache.Count; i++) onClearCache[i].Invoke();
        }

        private void CheckDebugging()
        {
            bool changed = false;
            switch (debugging)
            {
                case 0:
                    if (Finder.debug == true)
                        changed = true;
                    else return;
                    break;
                case 1:
                    if (Finder.debug == false)
                        return;
                    debugging = 2;
                    changed = true;
                    break;
                case 2:
                    if (Finder.debug == true)
                        return;
                    debugging = 1;
                    changed = true;
                    break;
            }
            if (!changed)
                return;
            if (debugging == 1)
            {
                for (var i = 0; i < onDebugginDisabled.Count; i++) onDebugginDisabled[i].Invoke();
            }
            else if (debugging == 2)
            {
                for (var i = 0; i < onDebugginEnabled.Count; i++) onDebugginEnabled[i].Invoke();
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnDefsLoaded : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnTickLong : Attribute
        {
        }


        [AttributeUsage(AttributeTargets.Method)]
        public class OnTick : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnWorldLoaded : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnMapLoaded : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnMapComponentsInitializing : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnClearCache : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnStaticConstructor : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnScribe : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnInitialization : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class YieldTabContent : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnDebugginEnabled : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OnDebugginDisabled : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class SettingsField : Attribute
        {
            public object warmUpValue;

            public SettingsField(object warmUpValue)
            {
                this.warmUpValue = warmUpValue;
            }
        }
    }
}