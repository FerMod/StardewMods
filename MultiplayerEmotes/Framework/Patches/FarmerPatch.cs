
using System.Reflection;
using MultiplayerEmotes.Extensions;
using Harmony;
using StardewValley;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MultiplayerEmotes.Patches {

	public class FarmerPatch : ClassPatch {

		public override MethodInfo Original => AccessTools.Method(typeof(Farmer), "doEmote", new Type[] { typeof(int) });
		public override MethodInfo Postfix => typeof(FarmerPatch).GetMethod("DoEmote_Postfix");

		public static void DoEmote_Postfix(Farmer __instance, int whichEmote) {
			if(Context.IsMultiplayer && __instance.IsLocalPlayer && __instance.IsEmoting) {
				Traverse.Create(typeof(Game1)).Field("multiplayer").GetValue<Multiplayer>().BroadcastEmote(whichEmote);
			}
		}

	}

}
