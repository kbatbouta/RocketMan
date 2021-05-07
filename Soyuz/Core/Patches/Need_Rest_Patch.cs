using HarmonyLib;
using RimWorld;

namespace Soyuz.Patches
{
    [SoyuzPatch(typeof(Need_Rest), nameof(Need_Rest.TickResting))]
    public class Need_Rest_TickResting_Patch
    {
        public static void Postfix(Need_Rest __instance)
        {
            if (true
                && __instance.pawn.IsValidWildlifeOrWorldPawn()
                && __instance.pawn.IsSkippingTicks())
                __instance.lastRestTick += __instance.pawn.GetDeltaT();
        }
    }
}