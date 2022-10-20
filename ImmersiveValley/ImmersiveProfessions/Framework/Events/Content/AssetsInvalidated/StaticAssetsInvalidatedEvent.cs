﻿namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Common.Events;
using DaLion.Common.Extensions.SMAPI;
using DaLion.Stardew.Professions.Framework.Textures;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StaticAssetsInvalidatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticAssetsInvalidatedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
    }

    /// <inheritdoc />
    public override bool Enable()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool Disable()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        Textures.Refresh(e.NamesWithoutLocale.Where(name => name.IsEquivalentToAnyOf(
            $"{ModEntry.Manifest.UniqueID}/SkillBars",
            $"{ModEntry.Manifest.UniqueID}/UltimateMeter",
            $"{ModEntry.Manifest.UniqueID}/PrestigeProgression"))
            .ToImmutableHashSet());
    }
}