﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Events.Display.RenderedHud;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

namespace TheLion.Stardew.Professions.Framework.Events.Input.ButtonsChanged;

internal class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.Modkey.JustPressed())
        {
            ModEntry.Subscriber.Subscribe(new ArrowPointerUpdateTickedEvent(), new TrackerRenderedHudEvent());
        }
        else if (ModEntry.Config.Modkey.GetState() == SButtonState.Released)
        {
            ModEntry.Subscriber.Unsubscribe(typeof(TrackerRenderedHudEvent));
            if (!(ModEntry.Subscriber.IsSubscribed(typeof(ProspectorHuntRenderedHudEvent)) ||
                  ModEntry.Subscriber.IsSubscribed(typeof(ScavengerHuntRenderedHudEvent))))
                ModEntry.Subscriber.Unsubscribe(typeof(ArrowPointerUpdateTickedEvent));
        }
    }
}