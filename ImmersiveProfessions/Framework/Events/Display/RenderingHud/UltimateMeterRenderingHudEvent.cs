﻿namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class UltimateMeterRenderingHudEvent : RenderingHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object sender, RenderingHudEventArgs e)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Disable();
            return;
        }

        if (!Game1.eventUp) ModEntry.PlayerState.RegisteredUltimate.Meter.Draw(e.SpriteBatch);
    }
}