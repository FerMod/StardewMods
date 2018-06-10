
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
		//public override MethodInfo Prefix => typeof(FarmerPatch).GetMethod("DoEmote_Prefix");
		public override MethodInfo Postfix => typeof(FarmerPatch).GetMethod("DoEmote_Postfix");

		public static void DoEmote_Postfix(Farmer __instance, int whichEmote) {
			if(Context.IsMultiplayer && __instance.IsLocalPlayer && __instance.IsEmoting) {
				//reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().broadcastEmote(whichEmote);
				Traverse.Create(typeof(Game1)).Field("multiplayer").GetValue<Multiplayer>().BroadcastEmote(whichEmote);
			}
		}

		public static bool DoEmote_Prefix(Farmer __instance, int whichEmote) {

			TemporaryAnimatedSprite emoteStart = new TemporaryAnimatedSprite("TileSheets\\emotes", new Rectangle(0, 0, 16, 16), new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160), false, 0f, Color.White) {
				interval = 100f,
				animationLength = 4,
				scale = 4f,
				layerDepth = 1f,
				local = false,
				attachedCharacter = Game1.getFarmer(Game1.player.UniqueMultiplayerID)
			};

			TemporaryAnimatedSprite emoteEnding = new TemporaryAnimatedSprite("TileSheets\\emotes", new Rectangle(0, whichEmote * 4, 16, 16), new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160), false, 0f, Color.White) {
				delayBeforeAnimationStart = 400,
				interval = 100f,
				animationLength = 4,
				scale = 4f,
				layerDepth = 1f,
				local = false,
				attachedCharacter = Game1.getFarmer(Game1.player.UniqueMultiplayerID)
			};

			List<TemporaryAnimatedSprite> tempAnimSpriteList = new List<TemporaryAnimatedSprite>() {
				emoteStart,
				emoteEnding
			};

			Multiplayer multiplayer = Traverse.Create(typeof(Game1)).Field("multiplayer").GetValue<Multiplayer>();
			multiplayer.broadcastSprites(Game1.getFarmer(Game1.player.UniqueMultiplayerID).currentLocation, tempAnimSpriteList);

			return false;
		}

	}

}
