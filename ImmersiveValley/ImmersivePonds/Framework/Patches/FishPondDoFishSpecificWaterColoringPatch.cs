﻿namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Xna;
using DaLion.Stardew.Ponds.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoFishSpecificWaterColoringPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDoFishSpecificWaterColoringPatch"/> class.</summary>
    internal FishPondDoFishSpecificWaterColoringPatch()
    {
        this.Target = this.RequireMethod<FishPond>("doFishSpecificWaterColoring");
    }

    #region harmony patches

    /// <summary>Recolor for algae/seaweed.</summary>
    [HarmonyPostfix]
    private static void FishPondDoFishSpecificWaterColoringPostfix(FishPond __instance)
    {
        if (__instance.fishType.Value.IsAlgaeIndex())
        {
            var shift = -5 - (3 * __instance.FishCount);
            __instance.overrideWaterColor.Value = new Color(60, 126, 150).ShiftHue(shift);
        }
        else if (__instance.GetFishObject().Name.ContainsAnyOf("Mutant", "Radioactive"))
        {
            __instance.overrideWaterColor.Value = new Color(40, 255, 40);
        }
    }

    #endregion harmony patches
}