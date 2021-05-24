﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using RimWorld;
using RocketMan;
using RocketMan.Tabs;
using UnityEngine;
using Verse;

namespace Soyuz.Tabs
{
    public class TabContent_Soyuz : ITabContent
    {
        private Listing_Standard standard = new Listing_Standard();
        private Listing_Standard standard_extras = new Listing_Standard();

        private static Vector2 scrollPosition = Vector2.zero;
        private static Rect viewRect = Rect.zero;
        private static string searchString;
        private static RaceSettings curSelection;

        public override string Label => "Time dilation controls";
        public override bool ShouldShow => Finder.enabled;

        public override void DoContent(Rect rect)
        {
            standard.Begin(rect.TopPartPixels(95 + (RocketDebugPrefs.debug ? 120 : 0)));
            var font = Text.Font;
            Text.Font = GameFont.Tiny;
            standard.CheckboxLabeled("Enable time dilation", ref Finder.timeDilation, "Experimental.");
            standard.CheckboxLabeled("Enable time dilation for caravans (Can cause issues)", ref Finder.timeDilationCaravans, "Disable this in case your caravans are consuming food too quickly.");
            standard.CheckboxLabeled("Enable time dilation for visitor pawns.", ref Finder.timeDilationVisitors, "Experimental: Can cause a lot of bugs.");
            standard.CheckboxLabeled("Enable time dilation for world pawns", ref Finder.timeDilationWorldPawns, "Throttle ticking for world pawns.");
            //standard.CheckboxLabeled("Enable time dilation for pawns with critical hediffs", ref Finder.timeDilationCriticalHediffs, "This will enable dilation for pawns with critical hediffs such as pregnant pawns or bleeding pawns. (Disable this in case of a hediff problem)");
            if (RocketDebugPrefs.debug)
            {
                standard.CheckboxLabeled("Enable data logging", ref RocketDebugPrefs.logData, "For debugging only.");
                standard.CheckboxLabeled("Set tick multiplier to 150", ref RocketDebugPrefs.debug150MTPS, "Dangerous!");
                standard.GapLine();
                standard.CheckboxLabeled("Enable flashing dilated pawns",
                    ref RocketDebugPrefs.flashDilatedPawns);
                standard.CheckboxLabeled("Simulate offscreen behavior", ref RocketDebugPrefs.alwaysDilating);
            }
            Text.Font = font;
            standard.End();
            rect.yMin += 85 + (RocketDebugPrefs.debug ? 120 : 0);
            DoExtras(rect);
        }

        public void DoExtras(Rect rect)
        {
            var stage = 0;
            rect.yMin += 5;
            Text.CurFontStyle.fontStyle = FontStyle.Bold;
            Widgets.Label(rect.TopPartPixels(25), "Dilated races");
            Text.CurFontStyle.fontStyle = FontStyle.Normal;
            if (Context.settings == null || Find.Selector == null)
            {
                return;
            }
            var font = Text.Font;
            var anchor = Text.Anchor;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleLeft;
            rect.yMin += 25;
            var searchRect = rect.TopPartPixels(25);
            string oldSearchString = searchString;
            searchString = Widgets.TextField(searchRect, searchString)?.ToLower()?.Trim() ?? string.Empty;
            if (oldSearchString != searchString)
                scrollPosition = Vector2.zero;
            rect.yMin += 30;
            if (curSelection != null)
            {
                var height = 128 + (RocketDebugPrefs.debug ? 25 : 0);
                var selectionRect = rect.TopPartPixels(height);
                Widgets.DrawMenuSection(selectionRect);
                Text.Font = GameFont.Tiny;
                Widgets.DefLabelWithIcon(selectionRect.TopPartPixels(54), curSelection.pawnDef);
                if (Widgets.ButtonImage(selectionRect.RightPartPixels(30).TopPartPixels(30).ContractedBy(5),
                    TexButton.CloseXSmall))
                {
                    curSelection = null;
                    return;
                }
                selectionRect.yMin += 54;
                standard_extras.Begin(selectionRect.ContractedBy(3));
                Text.Font = GameFont.Tiny;
                if (!IgnoreMeDatabase.ShouldIgnore(curSelection.pawnDef))
                {
                    standard_extras.CheckboxLabeled($"Enable dilation for {curSelection.pawnDef?.label ?? "_"}", ref curSelection.enabled, tooltip: "Used to control which races are dilated/throttled in case of a problem.");
                    standard_extras.CheckboxLabeled($"Disable dilation for all factions except the player faction", ref curSelection.ignoreFactions);
                    standard_extras.CheckboxLabeled($"Disable dilation for the player faction", ref curSelection.ignorePlayerFaction);
                }
                else
                {
                    standard_extras.Label($"This race will be ignored due to a mod <color=red>incompability issues or by a modder request!</color>");
                }
                if (RocketDebugPrefs.debug)
                {
                    if (curSelection.pawnDef.StatBaseDefined(StatDefOf.MoveSpeed))
                        standard_extras.Label($"Base race move speed is {curSelection.pawnDef.GetStatValueAbstract(StatDefOf.MoveSpeed)}:{Context.dilationFastMovingRace[curSelection.pawnDef.index]}");
                    else standard_extras.Label("Base race move speed is not defined");
                }
                standard_extras.End();
                rect.yMin += height + 8;
            }
            else if (Find.Selector.selected.Count == 1 && Find.Selector.selected.First() is Pawn pawn && pawn != null && searchString.NullOrEmpty())
            {
                var height = 128;
                var selectionRect = rect.TopPartPixels(height);
                var model = pawn.GetPerformanceModel();
                if (RocketDebugPrefs.debug) Log.Message($"SOYUZ: UI stage is {stage}:{1}");
                if (model != null)
                {
                    model.DrawGraph(selectionRect, 2000);
                    rect.yMin += height + 8;
                }
            }
            Widgets.DrawMenuSection(rect);
            rect = rect.ContractedBy(2);
            viewRect.size = rect.size;
            viewRect.height = 60 * Context.settings.raceSettings.Count;
            viewRect.width -= 15;
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect.AtZero());
            Rect curRect = viewRect.TopPartPixels(58);
            curRect.width -= 15;
            var counter = 0;
            foreach (var element in Context.settings.raceSettings)
            {
                string defLabel = element?.pawnDef?.label?.ToLower();
                if (defLabel == null)
                    continue;
                if (!searchString.NullOrEmpty() && !(defLabel?.Contains(searchString) ?? false))
                    continue;
                counter++;
                if (counter % 2 == 0)
                    Widgets.DrawBoxSolid(curRect, new Color(0.2f, 0.2f, 0.2f));
                //Widgets.DrawHighlightIfMouseover(curRect);
                Widgets.DefLabelWithIcon(curRect, element.pawnDef);
                if (Widgets.ButtonInvisible(curRect))
                {
                    curSelection = element;
                    break;
                }
                curRect.y += 58;
            }
            Widgets.EndScrollView();
            Text.Font = font;
            Text.Anchor = anchor;
            Finder.rocketMod.WriteSettings();
            SoyuzSettingsUtility.CacheSettings();
        }

        public override void OnSelect()
        {
        }

        public override void OnDeselect()
        {
        }

        [Main.YieldTabContent]
        public static ITabContent YieldTab() => new TabContent_Soyuz();
    }
}
