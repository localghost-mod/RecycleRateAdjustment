using UnityEngine;
using Verse;

namespace RecycleRate
{
    public class Settings : ModSettings
    {
        public float recycleRate = 0.25f;

        public override void ExposeData() => Scribe_Values.Look(ref recycleRate, "recycleRate");
    }

    public class RecycleRateMod : Mod
    {
        public static Settings settings;

        public RecycleRateMod(ModContentPack content)
            : base(content)
        {
            settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(inRect);
            ls.SliderLabeled(
                "RR.RecycleRate".Translate(),
                ref settings.recycleRate,
                settings.recycleRate.ToStringPercent(),
                0.01f,
                1.00f
            );
            if (ls.ButtonText("RR.Reset".Translate()))
                settings.recycleRate = 0.25f;
            ls.End();
        }

        public override string SettingsCategory() => "RR.RecycleRateAdjustment".Translate();
    }

    public static class Listing_StandardExtension
    {
        public static void SliderLabeled(
            this Listing_Standard ls,
            string label,
            ref float val,
            string format,
            float min = 0f,
            float max = 1f,
            string tooltip = null
        )
        {
            Rect rect = ls.GetRect(Text.LineHeight);
            Rect rect2 = GenUI.Rounded(GenUI.LeftPart(rect, 0.5f));
            Rect rect3 = GenUI.Rounded(
                GenUI.LeftPart(GenUI.Rounded(GenUI.RightPart(rect, 0.3f)), 0.67f)
            );
            Rect rect4 = GenUI.Rounded(GenUI.RightPart(rect, 0.1f));
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, label);
            float num = Widgets.HorizontalSlider_NewTemp(rect3, val, min, max);
            val = num;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect4, string.Format(format, val));
            if (!GenText.NullOrEmpty(tooltip))
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }
            Text.Anchor = anchor;
            ls.Gap(ls.verticalSpacing);
        }
    }
}
