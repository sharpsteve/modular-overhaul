﻿namespace DaLion.Redux.Professions.Patches.Combat;

#region using directives

using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using StardewValley.Projectiles;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ProjectileBehaviorOnCollisionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ProjectileBehaviorOnCollisionPatch"/> class.</summary>
    internal ProjectileBehaviorOnCollisionPatch()
    {
        this.Target = this.RequireMethod<Projectile>("behaviorOnCollision");
    }

    #region harmony patches

    /// <summary>Patch for Rascal chance to recover ammunition + Piper charge Ultimate with Slime ammo.</summary>
    [HarmonyPostfix]
    private static void ProjectileBehaviorOnCollisionPostfix(
        Projectile __instance,
        NetInt ___currentTileSheetIndex,
        NetPosition ___position,
        NetCharacterRef ___theOneWhoFiredMe,
        GameLocation location)
    {
        if (__instance is not ReduxProjectile { CanBeRecovered: true } projectile)
        {
            return;
        }

        var firer = ___theOneWhoFiredMe.Get(location) as Farmer ?? Game1.player;
        if (projectile.IsSlimeProjectile && firer.Get_Ultimate() is Concerto { IsActive: false } concerto)
        {
            concerto.ChargeValue += Game1.random.Next(5);
        }

        if (projectile.Ammo is null || projectile.IsSquishyProjectile || !firer.HasProfession(Profession.Rascal))
        {
            return;
        }

        var chance = projectile.Ammo.ParentSheetIndex is SObject.wood or SObject.coal ? 0.175f : 0.35f;
        if (firer.HasProfession(Profession.Rascal, true))
        {
            chance *= 2f;
        }

        if (Game1.random.NextDouble() > chance)
        {
            return;
        }

        location.debris.Add(
            new Debris(
                projectile.Ammo.ParentSheetIndex,
                new Vector2((int)___position.X, (int)___position.Y),
                firer.getStandingPosition()));
    }

    #endregion harmony patches
}