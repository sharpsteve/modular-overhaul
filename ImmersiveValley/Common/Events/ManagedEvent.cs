﻿namespace DaLion.Common.Events;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Common.Extensions.Collections;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Base implementation of an event wrapper allowing dynamic enabling / disabling.</summary>
internal abstract class ManagedEvent : IManagedEvent, IEquatable<ManagedEvent>
{
    private readonly PerScreen<bool> _enabled = new(() => false);

    /// <summary>Initializes a new instance of the <see cref="ManagedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ManagedEvent(EventManager manager)
    {
        this.Manager = manager;
    }

    /// <inheritdoc />
    public virtual bool IsEnabled => this._enabled.Value || this.AlwaysEnabled;

    /// <summary>Gets the <see cref="EventManager"/> instance that manages this event.</summary>
    protected EventManager Manager { get; }

    /// <summary>Gets a value indicating whether allow this event to be raised even when disabled.</summary>
    protected bool AlwaysEnabled { get; init; } = false;

    public static bool operator ==(ManagedEvent? left, ManagedEvent? right) => (object?)left == null ? (object?)right == null : left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ManagedEvent? left, ManagedEvent? right) => !(left == right);

    /// <summary>Determines whether two <see cref="ManagedEvent"/> instances are equal.</summary>
    /// <param name="other">A <see cref="ManagedEvent"/> to compare to this instance.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="other"/> has the same type as this instance, otherwise
    ///     <see langword="false"/>.
    /// </returns>
    public virtual bool Equals(ManagedEvent? other)
    {
        // ReSharper disable once CheckForReferenceEqualityInstead.1
        return this.GetType().Equals(other?.GetType());
    }

    /// <inheritdoc />
    /// <remarks>Ignored the <see cref="AlwaysEnabled"/> flag.</remarks>
    public bool IsEnabledForScreen(int screenId)
    {
        return this._enabled.GetValueForScreen(screenId);
    }

    /// <inheritdoc />
    public virtual bool Enable()
    {
        if (this._enabled.Value || !(this._enabled.Value = true))
        {
            return false;
        }

        this.OnEnabled();
        return true;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnEnabled"/> callback.</remarks>
    public bool EnableForScreen(int screenId)
    {
        if (!Context.IsMainPlayer || !Context.IsSplitScreen)
        {
            return false;
        }

        if (this._enabled.GetValueForScreen(screenId))
        {
            return false;
        }

        this._enabled.SetValueForScreen(screenId, true);
        return true;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnEnabled"/> callback.</remarks>
    public void EnableForAllScreens()
    {
        this._enabled.GetActiveValues().ForEach(pair => this._enabled.SetValueForScreen(pair.Key, true));
    }

    /// <inheritdoc />
    public virtual bool Disable()
    {
        if (!this._enabled.Value || (this._enabled.Value = false))
        {
            return false;
        }

        this.OnDisabled();
        return true;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnDisabled"/> callback.</remarks>
    public bool DisableForScreen(int screenId)
    {
        if (!Context.IsMainPlayer || !Context.IsSplitScreen)
        {
            return false;
        }

        if (!this._enabled.GetValueForScreen(screenId))
        {
            return false;
        }

        this._enabled.SetValueForScreen(screenId, false);
        return true;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnDisabled"/> callback.</remarks>
    public void DisableForAllScreens()
    {
        this._enabled.GetActiveValues().ForEach(pair => this._enabled.SetValueForScreen(pair.Key, false));
    }

    /// <inheritdoc />
    public void Reset()
    {
        this._enabled.ResetAllScreens();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.GetType().Name;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return this.GetType().GetHashCode();
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ManagedEvent other && this.Equals(other);
    }

    /// <summary>Invoked once when the event is enabled.</summary>
    protected virtual void OnEnabled()
    {
    }

    /// <summary>Invoked once when the event is disabled.</summary>
    protected virtual void OnDisabled()
    {
    }
}