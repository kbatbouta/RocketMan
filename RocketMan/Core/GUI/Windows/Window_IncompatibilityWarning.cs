﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;

namespace RocketMan
{
    public class Window_IncompatibilityWarning : Window
    {
        private const int COUNTDOWN_LENGTH = 10;

        private string message;
        private bool failed = false;
        private bool finished = false;
        private Action finishedAction;
        private Stopwatch stopwatch = new Stopwatch();
        private Listing_Standard standard = new Listing_Standard();

        public bool ShouldExpire
        {
            get => stopwatch.IsRunning && (stopwatch.Elapsed.Seconds > 40) || failed;
        }

        public override Vector2 InitialSize
        {
            get => new Vector2(Math.Min(UI.screenWidth / 3f, 400), Math.Min(UI.screenHeight / 2f, 400));
        }

        public int SecondsElapsed
        {
            get => stopwatch.Elapsed.Seconds;
        }

        public Window_IncompatibilityWarning(Action finishedAction = null)
        {
            draggable = false;
            absorbInputAroundWindow = false;
            preventCameraMotion = true;
            resizeable = false;
            drawShadow = true;
            doCloseButton = false;
            doCloseX = false;
            layer = WindowLayer.Super;
            this.finishedAction = finishedAction;
            this.message = "You are running incompatibile mods with RocketMan. This type of incompatilibity will nuke/destroy your save! " +
                "You can Disable RocketMan or You continue on your own responsibility. these mods are:\n";
            int counter = 0;
            foreach (string m in IncompatibilityHelper.incompatibleMods)
            {
                this.message += $"<color=orange>{counter}.</color> <color=red>{m.CapitalizeFirst()}</color>\n";
                counter++;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (failed || finished)
            {
                Close();
                return;
            }
            if (!stopwatch.IsRunning)
            {
                Log.Message("ROCKETMAN: compatibility warning started!");
                stopwatch.Start();
            }
            GUIUtility.ExecuteSafeGUIAction(() =>
            {
                standard.Begin(inRect.ContractedBy(2));
                Text.Anchor = TextAnchor.UpperCenter;
                Text.CurFontStyle.fontStyle = FontStyle.BoldAndItalic;
                Text.Font = GameFont.Medium;
                standard.Label(SecondsElapsed % 2 == 0 ? "<color=red>WARNING!!</color>" : "<color=yellow>WARNING!!</color>");
                standard.GapLine();
                Text.CurFontStyle.fontStyle = FontStyle.Normal;
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperLeft;
                Text.CurFontStyle.fontStyle = FontStyle.Normal;
                standard.Label(message);
                standard.GapLine();
                Text.CurFontStyle.fontStyle = FontStyle.BoldAndItalic;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                standard.Label("Are you <color=red>sure</color> you understand what can happen if you <color=red>continue</color>?");
                if (SecondsElapsed > COUNTDOWN_LENGTH)
                {
                    if (standard.ButtonText("Yes, I understand! <color=red>Continue!</color>"))
                    {
                        stopwatch.Stop();
                        finished = true;
                        Finder.incompatibilityUnresolved = false;
                        Close();
                    }
                    standard.Gap();
                    Text.Font = GameFont.Tiny;
                    Text.CurFontStyle.fontStyle = FontStyle.Normal;
                    standard.Label("<color=gray>You could always change your mod list and remove RocketMan!</color>");
                    Text.Font = GameFont.Small;
                    Text.CurFontStyle.fontStyle = FontStyle.BoldAndItalic;
                    standard.Gap();
                    if (standard.ButtonText("Yes, I understand! Open the modlist manager!"))
                    {
                        stopwatch.Stop();
                        finished = true;
                        Close();
                        Finder.incompatibilityUnresolved = false;
                        Find.WindowStack.Add(new Page_ModsConfig());
                    }
                    standard.Gap();
                    Text.Font = GameFont.Tiny;
                    Text.Anchor = TextAnchor.UpperLeft;
                    standard.Label("<color=gray>The RocketMan team will provide no support if you choose to ignore this message!</color>");
                }
                else
                {
                    Text.CurFontStyle.fontStyle = FontStyle.Normal;
                    standard.Label($"Please read the warning!\n<color=orange>You can continue in {COUNTDOWN_LENGTH - SecondsElapsed}</color>");
                }
                standard.End();
            }, fallbackAction: () =>
            {
                failed = true;
                AbortOnError();
            }, catchExceptions: true);
        }

        public override void Close(bool doCloseSound = true)
        {
            base.Close(doCloseSound);
            Finder.incompatibilityUnresolved = false;
            finishedAction?.Invoke();
        }

        private void AbortOnError() => Close();
    }
}
