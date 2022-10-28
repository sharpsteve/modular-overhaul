﻿namespace DaLion.Redux.Professions.Patches.Combat;

#region using directives

using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeCollisionWithFarmerBehaviorPatch : HarmonyPatch
{
    private const int FarmerInvincibilityFrames = 72;

    /// <summary>Initializes a new instance of the <see cref="GreenSlimeCollisionWithFarmerBehaviorPatch"/> class.</summary>
    internal GreenSlimeCollisionWithFarmerBehaviorPatch()
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
    }

    #region harmony patches

    /// <summary>Patch to increment Piper Ultimate meter on contact with Slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        if (!__instance.currentLocation.IsDungeon())
        {
            return;
        }

        var who = __instance.Player;
        if (!who.IsLocalPlayer || who.Get_Ultimate() is not Concerto { IsActive: false } concerto ||
            ModEntry.State.Professions.SlimeContactTimer > 0)
        {
            return;
        }

        concerto.ChargeValue += Game1.random.Next(1, 4);
        ModEntry.State.Professions.SlimeContactTimer = FarmerInvincibilityFrames;
    }

    #endregion harmony patches
}