﻿using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RocketLite.Optimizations;
using RocketLite.Patches;
using Verse;

namespace RocketLite
{
    public static class Patcher
    {
        public static void PatchAll()
        {
            foreach (var m in Pawn_Notify_Dirty.Pawn_ApparelTracker_Dirty.GetTargetMethods())
            {
                if (!m.IsValidTarget()) continue;
                try
                {
                    Finder.harmony.Patch(m, postfix: new HarmonyMethod(AccessTools.Method(
                        typeof(Pawn_Notify_Dirty.Pawn_ApparelTracker_Dirty),
                        nameof(Pawn_Notify_Dirty.Pawn_ApparelTracker_Dirty.Postfix))));
                }
                catch
                {
                    Log.Warning(string.Format("ROCKETLITE: failed to patch {0}", m.ToString()));
                }
            }
            foreach (var m in Pawn_Notify_Dirty.Pawn_Dirty.GetTargetMethods())
            {
                if (!m.IsValidTarget()) continue;
                try
                {
                    Finder.harmony.Patch(m, postfix: new HarmonyMethod(AccessTools.Method(
                        typeof(Pawn_Notify_Dirty.Pawn_Dirty),
                        nameof(Pawn_Notify_Dirty.Pawn_Dirty.Postfix))));
                }
                catch
                {
                    Log.Warning(string.Format("ROCKETLITE: failed to patch {0}", m.ToString()));
                }
            }
            foreach (var m in Pawn_Notify_Dirty.Pawn_EquipmentTracker_Dirty.GetTargetMethods())
            {
                if (!m.IsValidTarget()) continue;
                try
                {
                    Finder.harmony.Patch(m, postfix: new HarmonyMethod(AccessTools.Method(
                        typeof(Pawn_Notify_Dirty.Pawn_EquipmentTracker_Dirty),
                        nameof(Pawn_Notify_Dirty.Pawn_EquipmentTracker_Dirty.Postfix))));
                }
                catch
                {
                    Log.Warning(string.Format("ROCKETLITE: failed to patch {0}", m.ToString()));
                }
            }
            foreach (var m in Pawn_Notify_Dirty.Pawn_HealthTracker_Dirty.GetTargetMethods())
            {
                if (!m.IsValidTarget()) continue;
                try
                {
                    Finder.harmony.Patch(m, postfix: new HarmonyMethod(AccessTools.Method(
                        typeof(Pawn_Notify_Dirty.Pawn_HealthTracker_Dirty),
                        nameof(Pawn_Notify_Dirty.Pawn_HealthTracker_Dirty.Postfix))));
                }
                catch
                {
                    Log.Warning(string.Format("ROCKETLITE: failed to patch {0}", m.ToString()));
                }
            }
            foreach (var m in StatWorker_GetValueUnfinalized_Hijacked_Patch.GetTargetMethods())
            {
                if (!m.IsValidTarget()) continue;
                try
                {
                    Finder.harmony.Patch(m, transpiler: new HarmonyMethod(AccessTools.Method(
                        typeof(StatWorker_GetValueUnfinalized_Hijacked_Patch),
                        nameof(StatWorker_GetValueUnfinalized_Hijacked_Patch.Transpiler))));
                }
                catch
                {
                    Log.Warning(string.Format("ROCKETLITE: failed to patch {0}", m.ToString()));
                }
            }
            try
            {
                MethodBase targetMethod = StatPart_ApparelStatOffSet_Skipper_Patch.GetTargetMethod();
                if (targetMethod.IsValidTarget())
                {
                    Finder.harmony.Patch(targetMethod, transpiler: new HarmonyMethod(AccessTools.Method(
                        typeof(StatWorker_GetValueUnfinalized_Hijacked_Patch),
                        nameof(StatWorker_GetValueUnfinalized_Hijacked_Patch.Transpiler))));
                }
            }
            catch
            {
                Log.Warning(string.Format("ROCKETLITE: failed to patch {0}", StatPart_ApparelStatOffSet_Skipper_Patch.GetTargetMethod().ToString()));
            }
        }
    }
}
