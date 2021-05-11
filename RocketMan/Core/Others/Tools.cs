﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RocketMan.Optimizations;
using Verse;

namespace RocketMan
{
    public static class Tools
    {
        public static byte PredictValueFromString(this string name)
        {
            if (false
                || name.Contains("Combat")
                || name.Contains("Melee")
                || name.Contains("Range")
                || name.Contains("Ability")
                || name.Contains("Gain"))
                return 0;
            if (false
                || name.Contains("Stuff")
                || name.Contains("Cold")
                || name.Contains("Hot")
                || name.Contains("Insulation")
                || name.Contains("WorkSpeed")
                || name.Contains("Beauty")
                || name.Contains("Comfort")
                || name.Contains("Max")
                || name.Contains("Min"))
                return 128;
            return 32;
        }

        public static void Notify_Dirty(this Pawn pawn)
        {
            try
            {
                if (pawn != null)
                {
                    StatPart_ApparelStatOffSet_Skipper_Patch.Dirty(pawn);
                    StatWorker_GetValueUnfinalized_Hijacked_Patch.Dirty(pawn);
                }
            }
            catch (Exception er)
            {
                if (Finder.debug) Log.Warning(string.Format("ROCKETMAN: Notify_Dirty error of {0} at {1}",
                        er.Message, er.StackTrace));
            }
        }

        public static int GetKey(StatWorker statWorker, StatRequest req, bool applyPostProcess)
        {
            unchecked
            {
                int hash;
                hash = HashUtility.HashOne(statWorker.stat.shortHash);
                hash = HashUtility.HashOne(req.thingInt?.thingIDNumber ?? 0, hash);
                hash = HashUtility.HashOne(req.stuffDefInt?.shortHash ?? 0, hash);
                hash = HashUtility.HashOne((int)req.qualityCategoryInt, hash);
                hash = HashUtility.HashOne(req.defInt?.shortHash ?? 0, hash);
                hash = HashUtility.HashOne(req.faction?.loadID ?? 0, hash);
                hash = HashUtility.HashOne(req.pawn?.thingIDNumber ?? 0, hash);
                hash = HashUtility.HashOne(applyPostProcess ? 1 : 0, hash);
                return hash;
            }
        }

        public static int GetKey(TraverseParms traverseParms, LocalTargetInfo dest)
        {
            int hash;
            unchecked
            {
                hash = traverseParms.pawn?.thingIDNumber.GetHashCode() ?? 0;
                hash = hash ^ (dest.Cell.GetHashCode() << 1);
            }

            return hash;
        }

        public static int GetKey(ThoughtDef def, Pawn pawn)
        {
            var hash = 0;
            unchecked
            {
                hash = HashUtility.HashOne(def.shortHash);
                hash = HashUtility.HashOne(pawn.thingIDNumber.GetHashCode(), hash);
            }

            return hash;
        }

        public static int GetKey(StatRequest req)
        {
            unchecked
            {
                int hash;
                hash = HashUtility.HashOne(req.thingInt?.thingIDNumber ?? 0);
                hash = HashUtility.HashOne(req.stuffDefInt?.shortHash ?? 0, hash);
                hash = HashUtility.HashOne((int)req.qualityCategoryInt, hash);
                hash = HashUtility.HashOne(req.defInt?.shortHash ?? 0, hash);
                hash = HashUtility.HashOne(req.faction?.loadID ?? 0, hash);
                hash = HashUtility.HashOne(req.pawn?.thingIDNumber ?? 0, hash);

                return hash;
            }
        }

        public static Dictionary<A, B> DeepCopy<A, B>(this Dictionary<A, B> dict)
        {
            var other = new Dictionary<A, B>();
            foreach (var unit in dict)
                other[unit.Key] = unit.Value;
            return other;
        }
    }
}