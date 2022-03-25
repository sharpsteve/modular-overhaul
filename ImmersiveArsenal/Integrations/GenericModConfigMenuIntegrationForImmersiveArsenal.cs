namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Arsenal.</summary>
internal class GenericModConfigMenuIntegrationForImmersiveArsenal
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public GenericModConfigMenuIntegrationForImmersiveArsenal(IModRegistry modRegistry, IManifest manifest,
        Func<ModConfig> getConfig, Action reset, Action saveAndApply, Action<string, LogLevel> log)
    {
        _configMenu = new(modRegistry, manifest, log, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!_configMenu.IsLoaded)
            return;

        // register
        _configMenu
            .Register(true);
    }
}