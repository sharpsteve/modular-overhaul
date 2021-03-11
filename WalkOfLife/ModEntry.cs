﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;
using System;
using TheLion.AwesomeProfessions.Framework;
using TheLion.AwesomeProfessions.Framework.Patches;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static ModConfig Config { get; set; }
		public static ModData Data { get; set; }
		public static IModRegistry Registry { get; set; }
		public static IReflectionHelper Reflection { get; set; }
		public static ITranslationHelper I18n { get; set; }

		public static int DemolitionistBuffMagnitude { get; set; } = 0;
		public static uint BruteKillStreak { get; set; } = 0;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get configs.json
			Config = helper.ReadConfig<ModConfig>();
			
			// get mod registry
			Registry = helper.ModRegistry;

			// get reflection interface
			Reflection = helper.Reflection;

			// get localized content
			I18n = helper.Translation;

			// add event hooks
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.Saved += OnSaved;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Player.Warped += OnWarped;

			// apply patches
			new Patcher(ModManifest.UniqueID).ApplyAll(
				new AnimalHouseAddNewHatchedAnimalPatch(Config, Monitor),
				new BobberBarCtorPatch(Config, Monitor),
				new BushShakePatch(Config, Monitor),
				new CaskPerformObjectDropInActionPatch(Config, Monitor),
				new CrabPotCheckForActionPatch(Config, Monitor),
				new CrabPotDayUpdatePatch(Config, Monitor),
				new CraftingRecipeCtorPatch(Config, Monitor),
				new CropHarvestPatch(Config, Monitor),
				new FarmAnimalDayUpdatePatch(Config, Monitor),
				new FarmAnimalGetSellPricePatch(Config, Monitor),
				new FarmAnimalPetPatch(Config, Monitor),
				new FishingRodStartMinigameEndFunctionPatch(Config, Monitor),
				new FishPondUpdateMaximumOccupancyPatch(Config, Monitor),
				new FruitTreeDayUpdatePatch(Config, Monitor),
				new Game1CreateItemDebrisPatch(Config, Monitor),
				new Game1CreateObjectDebrisPatch(Config, Monitor),
				new GameLocationBreakStonePatch(Config, Monitor),
				new GameLocationDamageMonsterPatch(Config, Monitor),
				new GameLocationGetFishPatch(Config, Monitor),
				new GameLocationExplodePatch(Config, Monitor),
				new GameLocationOnStoneDestroyedPatch(Config, Monitor),
				new HoeDirtApplySpeedIncreasesPatch(Config, Monitor),
				new LevelUpMenuAddProfessionDescriptionsPatch(Config, Monitor, I18n),
				new LevelUpMenuGetImmediateProfessionPerkPatch(Config, Monitor),
				new LevelUpMenuGetProfessionNamePatch(Config, Monitor),
				new LevelUpMenuGetProfessionTitleFromNumberPatch(Config, Monitor, I18n),
				new LevelUpMenuRemoveImmediateProfessionPerkPatch(Config, Monitor),
				new LevelUpMenuRevalidateHealthPatch(Config, Monitor),
				new MineShaftCheckStoneForItemsPatch(Config, Monitor),
				new ObjectCtorPatch(Config, Monitor),
				new ObjectGetMinutesForCrystalariumPatch(Config, Monitor),
				new ObjectGetPriceAfterMultipliersPatch(Config, Monitor),
				new PondQueryMenuDrawPatch(Config, Monitor, Reflection),
				new QuestionEventSetUpPatch(Config, Monitor),
				new TemporaryAnimatedSpriteCtorPatch(Config, Monitor),
				new TreeDayUpdatePatch(Config, Monitor),
				new TreeUpdateTapperProductPatch(Config, Monitor)
			);
		}

		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			Data = Helper.Data.ReadSaveData<ModData>("thelion.AwesomeProfessions") ?? new ModData();
		}

		/// <summary>Raised after the game writes data to save file (except the initial save creation).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaved(object sender, SavedEventArgs e)
		{
			Helper.Data.WriteSaveData("thelion.AwesomeProfessions", Data);
		}

		/// <summary>Raised after the game state is updated.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			GameLocation location = Game1.currentLocation;
			if (Utils.LocalPlayerHasProfession("spelunker") && location is MineShaft)
			{
				AddOrUpdateBuff(Utils.SpelunkerBuffUniqueID, 1, "spelunker");

				int currentMineLevel = (location as MineShaft).mineLevel;
				if (currentMineLevel > Data.LowestMineLevelReached)
				{
					Data.LowestMineLevelReached = currentMineLevel;
				}
			}

			if (DemolitionistBuffMagnitude > 0)
			{
				if (e.Ticks % 30 == 0)
				{
					int buffDecay = DemolitionistBuffMagnitude > 4 ? 2 : 1;
					DemolitionistBuffMagnitude = Math.Max(0, DemolitionistBuffMagnitude - buffDecay);
				}
				AddOrUpdateBuff(Utils.DemolitionistBuffUniqueID, DemolitionistBuffMagnitude, "demolitionist");
			}
		}

		/// <summary>Raised after the current player moves to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.OldLocation.NameOrUniqueName != e.NewLocation.NameOrUniqueName)
			{
				BruteKillStreak = 0;
			}
		}

		/// <summary>Add or update a buff.</summary>
		/// <param name="buffId">The unique id for the buff.</param>
		/// <param name="magnitude">The magnitude of the buff.</param>
		/// <param name="source">The source of the buff.</param>
		private void AddOrUpdateBuff(int buffId, int magnitude, string source)
		{
			buffId += magnitude;
			Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == buffId);
			if (buff == null)
			{
				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: magnitude, 0, 0, minutesDuration: 1, source: source, displaySource: I18n.Get(source + ".buff")) { which = buffId }
				);
				buff.millisecondsDuration = 50;
			}
		}
	}
}
