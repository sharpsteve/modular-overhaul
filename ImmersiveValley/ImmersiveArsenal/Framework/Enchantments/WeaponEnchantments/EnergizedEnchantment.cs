﻿namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using Events;
using StardewValley.Monsters;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Moving and attacking generates Energize stacks, up to 100. When fully Energized, the next attack causes an electric discharge.</summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_EnergizedEnchantment")]
public class EnergizedEnchantment : BaseWeaponEnchantment
{
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (ModEntry.EnergizeStacks.Value >= 100)
        {
            // do energized...

            ModEntry.EnergizeStacks.Value = 0;
        }
        else
        {
            ++ModEntry.EnergizeStacks.Value;
        }
    }

    protected override void _OnEquip(Farmer who)
    {
        ModEntry.EnergizeStacks.Value = 0;
        ModEntry.Manager.Enable<EnergizedUpdateTickedEvent>();
    }

    protected override void _OnUnequip(Farmer who)
    {
        ModEntry.EnergizeStacks.Value = -1;
        ModEntry.Manager.Disable<EnergizedUpdateTickedEvent>();
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.energized");
}