﻿using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class PondQueryMenuDrawPatch : BasePatch
	{
		private static IReflectionHelper _reflection;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal PondQueryMenuDrawPatch(ModConfig config, IMonitor monitor, IReflectionHelper reflection)
		: base(config, monitor)
		{
			_reflection = reflection;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(PondQueryMenu), nameof(PondQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
				prefix: new HarmonyMethod(GetType(), nameof(PondQueryMenuDrawPrefix))
			);
		}

		/// <summary>Patch to adjust fish pond UI for Aquarist increased max capacity.</summary>
		protected static bool PondQueryMenuDrawPrefix(ref PondQueryMenu __instance, ref float ____age, ref Rectangle ____confirmationBoxRectangle, ref string ____confirmationText, ref SObject ____fishItem, ref FishPond ____pond, ref bool ___confirmingEmpty, ref string ___hoverText, SpriteBatch b)
		{
			if (____pond.FishCount <= 10)
			{
				return true; // run original logic;
			}

			if (!Game1.globalFade)
			{
				b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
				bool has_unresolved_needs = ____pond.neededItem.Value != null && ____pond.HasUnresolvedNeeds() && !____pond.hasCompletedRequest.Value;
				string pond_name_text = Game1.content.LoadString("Strings\\UI:PondQuery_Name", ____fishItem.DisplayName);
				Vector2 text_size = Game1.smallFont.MeasureString(pond_name_text);
				Game1.DrawBox((int)((Game1.uiViewport.Width / 2) - (text_size.X + 64f) * 0.5f), __instance.yPositionOnScreen - 4 + 128, (int)(text_size.X + 64f), 64);
				Utility.drawTextWithShadow(b, pond_name_text, Game1.smallFont, new Vector2((Game1.uiViewport.Width / 2) - text_size.X * 0.5f, (float)(__instance.yPositionOnScreen - 4) + 160f - text_size.Y * 0.5f), Color.Black);
				string displayed_text = _reflection.GetMethod(__instance, "getDisplayedText").Invoke<string>();
				int extraHeight = 0;
				if (has_unresolved_needs)
				{
					extraHeight += 116;
				}

				int extraTextHeight = _reflection.GetMethod(__instance, "measureExtraTextHeight").Invoke<int>(displayed_text);
				Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen + 128, PondQueryMenu.width, PondQueryMenu.height - 128 + extraHeight + extraTextHeight, speaker: false, drawOnlyBox: true);
				string population_text = Game1.content.LoadString("Strings\\UI:PondQuery_Population", string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
				text_size = Game1.smallFont.MeasureString(population_text);
				Utility.drawTextWithShadow(b, population_text, Game1.smallFont, new Vector2((__instance.xPositionOnScreen + PondQueryMenu.width / 2) - text_size.X * 0.5f, __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128), Game1.textColor);
				int slots_to_draw = ____pond.maxOccupants.Value;
				float slot_spacing = 11f;
				int x = 0;
				int y = 0;
				for (int i = 0; i < slots_to_draw; i++)
				{
					float y_offset = (float)Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
					if (i < ____pond.FishCount)
					{
						____fishItem.drawInMenu(b, new Vector2((__instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2) - slot_spacing * Math.Min(slots_to_draw, 5) * 4f * 0.5f + slot_spacing * 4f * x - 12f, (__instance.yPositionOnScreen + (int)(y_offset * 4f)) + (y * 4) * slot_spacing + 275.2f), 0.75f, 1f, 0f, StackDrawType.Hide, Color.White, drawShadow: false);
					}
					else
					{
						____fishItem.drawInMenu(b, new Vector2((__instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2) - slot_spacing * Math.Min(slots_to_draw, 5) * 4f * 0.5f + slot_spacing * 4f * x - 12f, (__instance.yPositionOnScreen + (int)(y_offset * 4f)) + (y * 4) * slot_spacing + 275.2f), 0.75f, 0.35f, 0f, StackDrawType.Hide, Color.Black, drawShadow: false);
					}
					x++;
					if (x == 6)
					{
						x = 0;
						y++;
					}
				}

				text_size = Game1.smallFont.MeasureString(displayed_text);
				Utility.drawTextWithShadow(b, displayed_text, Game1.smallFont, new Vector2((__instance.xPositionOnScreen + PondQueryMenu.width / 2) - text_size.X * 0.5f, (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - (has_unresolved_needs ? 32 : 48)) - text_size.Y), Game1.textColor);
				if (has_unresolved_needs)
				{
					_reflection.GetMethod(__instance, "drawHorizontalPartition").Invoke(b, (int)((__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight) - 48f));
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((__instance.xPositionOnScreen + 60) + 8f * Game1.dialogueButtonScale / 10f, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28), new Rectangle(412, 495, 5, 4), Color.White, (float)Math.PI / 2f, Vector2.Zero);
					string bring_text = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequest_Bring");
					text_size = Game1.smallFont.MeasureString(bring_text);
					int left_x = __instance.xPositionOnScreen + 88;
					float text_x = left_x;
					float icon_x = text_x + text_size.X + 4f;
					if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
					{
						icon_x = left_x - 8;
						text_x = left_x + 76;
					}
					Utility.drawTextWithShadow(b, bring_text, Game1.smallFont, new Vector2(text_x, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 24), Game1.textColor);
					b.Draw(Game1.objectSpriteSheet, new Vector2(icon_x, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ____pond.neededItem.Value.ParentSheetIndex, 16, 16), Color.Black * 0.4f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
					b.Draw(Game1.objectSpriteSheet, new Vector2(icon_x + 4f, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ____pond.neededItem.Value.ParentSheetIndex, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
					if ((int)____pond.neededItemCount.Value > 1)
					{
						Utility.drawTinyDigits(____pond.neededItemCount.Value, b, new Vector2(icon_x + 48f, __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 48), 3f, 1f, Color.White);
					}
				}

				__instance.okButton.draw(b);
				__instance.emptyButton.draw(b);
				__instance.changeNettingButton.draw(b);
				if (___confirmingEmpty)
				{
					b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
					int padding = 16;
					____confirmationBoxRectangle.Width += padding;
					____confirmationBoxRectangle.Height += padding;
					____confirmationBoxRectangle.X -= padding / 2;
					____confirmationBoxRectangle.Y -= padding / 2;
					Game1.DrawBox(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y, ____confirmationBoxRectangle.Width, ____confirmationBoxRectangle.Height);
					____confirmationBoxRectangle.Width -= padding;
					____confirmationBoxRectangle.Height -= padding;
					____confirmationBoxRectangle.X += padding / 2;
					____confirmationBoxRectangle.Y += padding / 2;
					b.DrawString(Game1.smallFont, ____confirmationText, new Vector2(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y), Game1.textColor);
					__instance.yesButton.draw(b);
					__instance.noButton.draw(b);
				}
				else if (___hoverText != null && ___hoverText.Length > 0)
				{
					IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont);
				}
			}
			__instance.drawMouse(b);

			return false; // don't run original logic
		}
	}
}