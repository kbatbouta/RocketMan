using UnityEngine;
using Verse;

namespace RocketMan.Tabs
{
    public class TabContent_Settings : ITabContent
    {
        public override string Label => "Home";
        public override bool ShouldShow => true;

        private Texture2D graphic = ContentFinder<Texture2D>.Get("RocketMan/UI/rocketman_main_nobackground", true);

        public override void DoContent(Rect rect)
        {
            GUIUtility.ExecuteSafeGUIAction(() =>
            {
                if (Finder.WarmingUp)
                {
                    GUIUtility.ExecuteSafeGUIAction(() =>
                    {
                        Text.Font = GameFont.Medium;
                        Text.Anchor = TextAnchor.MiddleCenter;
                        if (Find.TickManager.Paused)
                            Widgets.Label(rect, "Please unpause the game... RocketMan is warming up!");
                        else
                            Widgets.Label(rect, "Please wait... RocketMan is warming up!");
                    });
                }
                else
                {
                    Rect imageRect = rect.TopPartPixels(200);
                    imageRect.width = 685 - 180;
                    Widgets.DrawTextureFitted(imageRect, graphic, 1.0f);
                    rect.yMin += 215;
                    RocketMod.DoSettings(rect);
                }
            });
        }

        public override void OnSelect()
        {
        }

        public override void OnDeselect()
        {
        }
    }
}