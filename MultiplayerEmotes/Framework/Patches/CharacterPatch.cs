using Harmony;
using MultiplayerEmotes.Extensions;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;

namespace MultiplayerEmotes.Framework.Patches {

	internal static class CharacterPatch {

		internal class DoEmotePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Character), nameof(Character.doEmote), new Type[] { typeof(int), typeof(bool), typeof(bool) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(DoEmotePatch.DoEmote_Postfix));

			private static IReflectionHelper Reflection;

			public DoEmotePatch(IReflectionHelper reflection) {
				Reflection = reflection;
			}

			private static void DoEmote_Postfix(Character __instance, int whichEmote) {
				if(Context.IsMultiplayer && !(__instance is Farmer)) {
					// Traverse.Create(typeof(Game1)).Field("multiplayer").GetValue<Multiplayer>().BroadcastEmote(whichEmote);
					Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().BroadcastEmote(whichEmote, __instance);
				}
			}

		}

	}
}
