﻿using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ProfessionsCheatSetProfessionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ProfessionsCheatSetProfessionPatch()
		{
			Original = AccessTools.Method("CJBCheatsMenu.Framework.Cheats.Skills.ProfessionsCheat:SetProfession");
			Transpiler = new HarmonyMethod(GetType(), nameof(ProfessionsCheatSetProfessionTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ProfessionsCheatSetProfessionTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: case <defender_id>
			/// To: case <brute_id>

			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
					)
					.SetOperand(Util.Professions.IndexOf("Brute"));
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while moving CJB Profession Cheat health bonus from Defender to Brute.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}