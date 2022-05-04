﻿namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using StardewValley.Objects;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Chest"/> class.</summary>
public static class ChestExtensions
{
    /// <summary>Remove a specified ring from the chest's inventory.</summary>
    /// <param name="index">The ring index.</param>
    /// <param name="amount">How many rings should be consumed.</param>
    /// <returns>Returns the leftover amount, if not enough were consumed.</returns>
    public static int ConsumeRing(this Chest chest, int index, int amount)
    {
        var list = chest.items;
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i] is not Ring || list[i].ParentSheetIndex != index) continue;

            --amount;
            list[i] = null;
            if (amount > 0) continue;

            return 0;
        }

        return amount;
    }

    /// <summary>Remove a specified object from the chest's inventory.</summary>
    /// <param name="index">The object index.</param>
    /// <param name="amount">How many rings should be consumed.</param>
    /// <returns>Returns the leftover amount, if not enough were consumed.</returns>
    public static int ConsumeObject(this Chest chest, int index, int amount)
    {
        var list = chest.items;
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i] is not SObject || list[i].ParentSheetIndex != index) continue;

            var toRemove = amount;
            amount -= list[i].Stack;
            list[i].Stack -= toRemove;
            if (list[i].Stack <= 0)
                list[i] = null;

            if (amount > 0) continue;

            return 0;
        }

        return amount;
    }
}