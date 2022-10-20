﻿namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentUnapplyToPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentUnapplyToPatch"/> class.</summary>
    internal JadeEnchantmentUnapplyToPatch()
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Jade enchant.</summary>
    [HarmonyPrefix]
    private static bool JadeEnchantmentUnpplyToPrefix(JadeEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.critMultiplier.Value -= 0.5f * __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}