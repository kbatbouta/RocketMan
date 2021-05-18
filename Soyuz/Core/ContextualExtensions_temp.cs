﻿using System;
using RimWorld;
using RimWorld.Planet;
using RocketMan;
using Verse;

namespace Soyuz
{
    public static partial class ContextualExtensions
    {
        public static bool IsValidWildlifeOrWorldPawnInternal_newtemp(this Pawn pawn)
        {
            if (pawn?.def == null)
                return false;
            if (!Finder.enabled || !Finder.timeDilation)
                return false;
            if (!Context.dilationEnabled[pawn.def.index] || IgnoreMeDatabase.ShouldIgnore(pawn.def))
                return false;
            if (WorldPawnsTicker.isActive)
            {
                if (!Finder.timeDilationCaravans && pawn.IsCaravanMember() && pawn.GetCaravan().IsPlayerControlled)
                    return false;
                if (!Finder.timeDilationWorldPawns)
                    return false;
                return true;
            }
            if (pawn.IsBleeding() || (!Finder.timeDilationCriticalHediffs && pawn.HasCriticalHediff()))
                return false;
            if (pawn.def.race.Humanlike)
            {
                Faction playerFaction = Faction.OfPlayer;
                if (pawn.factionInt == playerFaction)
                    return false;
                if (pawn.guest?.isPrisonerInt ?? false && pawn.guest?.hostFactionInt == playerFaction)
                    return false;
                if (Finder.timeDilationVisitors)
                {
                    JobDef jobDef = pawn.jobs?.curJob?.def;
                    if (jobDef == null)
                        return false;
                    if (IgnoreMeDatabase.ShouldIgnore(jobDef))
                        return false;
                    if (jobDef == JobDefOf.Wait_Wander)
                        return true;
                    if (jobDef == JobDefOf.Wait)
                        return true;
                    if (jobDef == JobDefOf.SocialRelax)
                        return true;
                    if (jobDef == JobDefOf.LayDown)
                        return true;
                    if (jobDef == JobDefOf.Follow)
                        return true;
                }
                return WorldPawnsTicker.isActive;
            }
            RaceSettings raceSettings = pawn.GetRaceSettings();
            if (pawn.factionInt == Faction.OfPlayer)
                return !raceSettings.ignorePlayerFaction;
            if (pawn.factionInt != null)
                return !raceSettings.ignoreFactions;
            return true;
        }

        public static bool IsSkippingTicks_newtemp(this Pawn pawn)
        {
            if (!pawn.Spawned && WorldPawnsTicker.isActive)
                return true;
            if (pawn.OffScreen())
                return true;
            if (Context.zoomRange == CameraZoomRange.Far || Context.zoomRange == CameraZoomRange.Furthest || Context.zoomRange == CameraZoomRange.Middle)
                return true;
            return false;
        }
    }
}
