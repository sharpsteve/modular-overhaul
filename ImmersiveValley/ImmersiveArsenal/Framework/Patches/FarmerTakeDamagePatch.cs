﻿namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatch"/> class.</summary>
    internal FarmerTakeDamagePatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Grant i-frames during stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool FarmerTakeDamagePrefix(Farmer __instance)
    {
        return __instance.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.stabbingSword, isOnSpecial: true };
    }

    /// <summary>Removes damage mitigation soft cap.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (ModEntry.Config.RemoveDefenseSoftCap)
        //     skip
        //     {
        //         effectiveResilience >= damage * 0.5f)
        //         effectiveResilience -= (int) (effectiveResilience * Game1.random.Next(3) / 10f);
        //     }
        try
        {
            var skipSoftCap = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f))
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.RemoveFarmerDefenseSoftCap))),
                    new CodeInstruction(OpCodes.Brtrue_S, skipSoftCap))
                .AdvanceUntil(new CodeInstruction(OpCodes.Stloc_3))
                .Advance()
                .AddLabels(skipSoftCap);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla defense cap.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}