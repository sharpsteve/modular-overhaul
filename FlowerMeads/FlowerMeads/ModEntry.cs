﻿#nullable enable
using System.Collections;
using StardewModdingAPI;

namespace FlowerMeads;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /// <summary>Construct an instance.</summary>
    public ModEntry()
    {
        // add mead entry to BAGI's ContentSourceManager dictionary
        // this will fix a likely KeyNotFoundException
        var artisanGoodToSourceTypeDict = (IDictionary)"BetterArtisanGoodIcons.Content.ContentSourceManager".ToType()
            .RequireField("artisanGoodToSourceType").GetValue(null)!;
        artisanGoodToSourceTypeDict.Add(Globals.MeadAsArtisanGoodEnum, "Flowers");

        // apply patches
        HarmonyPatcher.Apply(new("DaLion.Meads"));
    }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        if (helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            HarmonyPatcher.ApplyAutomate(new("DaLion.Meads.Automate"));
    }
}