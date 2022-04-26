﻿namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Integrations;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.GameLaunched"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class GameLaunchedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
        Log.D("[Arsenal] Hooked GameLaunched event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.GameLaunched -= OnGameLaunched;
        Log.D("[Arsenal] Unhooked GameLaunched event.");
    }

    /// <summary>The event called after the first game update, once all mods are loaded.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForImmersiveArsenal(
            getConfig: () => ModEntry.Config,
            reset: () =>
            {
                ModEntry.Config = new();
                ModEntry.ModHelper.WriteConfig(ModEntry.Config);
            },
            saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
            log: ModEntry.Log,
            modRegistry: ModEntry.ModHelper.ModRegistry,
            manifest: ModEntry.Manifest
        ).Register();
    }
}