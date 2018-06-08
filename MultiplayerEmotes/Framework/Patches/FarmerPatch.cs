
using System.Reflection;
using MultiplayerEmotes.Extensions;
using Harmony;
using StardewValley;
using StardewModdingAPI;
using System;

namespace MultiplayerEmotes.Patches {

	public class FarmerPatch {

		private static MethodInfo Original => AccessTools.Method(typeof(Farmer), "doEmote", new Type[] { typeof(int) });

		private static MethodInfo Postfix => typeof(FarmerPatch).GetMethod("DoEmote_Postfix");

		private static IReflectionHelper Reflection;

		internal static void Register(HarmonyInstance harmony, IReflectionHelper reflection) {
			harmony.Patch(Original, null, new HarmonyMethod(Postfix));
			Reflection = reflection;
		}

		internal static void Remove(HarmonyInstance harmony) {
			harmony.RemovePatch(Original, HarmonyPatchType.Postfix, harmony.Id);
		}

		public static void DoEmote_Postfix(Farmer __instance, int whichEmote) {
			if(Context.IsMultiplayer && __instance.IsLocalPlayer && __instance.IsEmoting) {
				Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().broadcastEmote(whichEmote);
			}
		}

	}

}
