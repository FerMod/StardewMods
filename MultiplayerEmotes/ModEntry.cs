
using StardewModdingAPI;
using StardewValley;
using MultiplayerEmotes.Patches;
using StardewModdingAPI.Events;
using System;
using Microsoft.Xna.Framework;
using MultiplayerEmotes.Menus;
using MultiplayerEmotes.Events;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MultiplayerEmotes {

	public class ModEntry : Mod {

		public static ModConfig Config { get; private set; }
		public static ModData Data { get; private set; }

		private EmoteMenuButton emoteMenuButton;

		/*
		 Emotes only visible by others with the mod.
		 The host needs to have the mod, to others with the mod use it.
		 If the host does not have the mod, it will not work.
		 */
		public override void Entry(IModHelper helper) {
			ModPatchControl PatchManager = new ModPatchControl(helper);
			PatchManager.PatchList.Add(new FarmerPatch());
			PatchManager.PatchList.Add(new MultiplayerPatch());
			PatchManager.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Debug);
			Config = helper.ReadConfig<ModConfig>();

			this.Monitor.Log("Loading mod data...", LogLevel.Debug);
			Data = this.Helper.ReadJsonFile<ModData>("data.json") ?? new ModData();

			SaveEvents.AfterLoad += this.AfterLoad;
			SaveEvents.AfterReturnToTitle += this.AfterReturnToTitle;
			InputEvents.ButtonPressed += this.ButtonPressed;
			//GraphicsEvents.OnPostRenderEvent += this.OnPostRenderEvent;

			helper.ConsoleCommands.Add("emote", "Play the emote animation with the passed id.\n\nUsage: emote <value>\n- value: a integer representing the animation id.", this.Emote);
			helper.ConsoleCommands.Add("stop_emote", "Stop any playing emote.\n\nUsage: stop_emote", this.StopEmote);
			helper.ConsoleCommands.Add("stop_all_emotes", "Stop any playing emote by players.\n\nUsage: stop_all_emotes", this.StopAllEmotes);

			//ModHelp = Helper;

		}

		private void AfterReturnToTitle(object sender, EventArgs e) {
			Data.MenuPosition = new Vector2(emoteMenuButton.xPositionOnScreen, emoteMenuButton.yPositionOnScreen);
			this.Helper.WriteJsonFile("data.json", Data);
		}

		private void ButtonPressed(object sender, EventArgsInput e) {
			if(Context.IsWorldReady) {
				if(emoteMenuButton.IsBeingDragged && e.Button == SButton.MouseRight) {
					e.SuppressButton();
				}
			}
		}
		/*
		

		private void OnPostRenderEvent(object sender, EventArgs e) {
			if(emoteStart != null || emoteEnding != null) {

				Vector2 emotePosition = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160);

				if(emoteStart != null) {
					emoteStart.position = emotePosition;
				}

				if(emoteEnding != null) {
					emoteEnding.position = emotePosition;
				}

			}
		}
		public static IModHelper ModHelp;
		public static bool Flag { get; set; }

		public static TemporaryAnimatedEmote emoteStart, emoteEnding;

		public static void BroadcastSpritesTest(int whichEmote) {

			Multiplayer multiplayer = ModHelp.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

			TemporaryAnimatedEmote emoteStart = new TemporaryAnimatedEmote("TileSheets\\emotes", new Rectangle(0, 0, 16, 16), new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160), false, 0f, Color.White) {
				interval = 100f,
				animationLength = 4,
				scale = 4f,
				layerDepth = 1f,
				local = false,
				attachedCharacter = Game1.getFarmer(Game1.player.UniqueMultiplayerID),
				overrideLocationDestroy = true
			};

			TemporaryAnimatedEmote emoteEnding = new TemporaryAnimatedEmote("TileSheets\\emotes", new Rectangle(0, whichEmote * 4, 16, 16), new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 160), false, 0f, Color.White) {
				delayBeforeAnimationStart = 400,
				interval = 100f,
				animationLength = 4,
				scale = 4f,
				layerDepth = 1f,
				local = false,
				attachedCharacter = Game1.getFarmer(Game1.player.UniqueMultiplayerID),
				overrideLocationDestroy = true
			};

			List<TemporaryAnimatedSprite> tempAnimSpriteList = new List<TemporaryAnimatedSprite>() {
				emoteStart,
				emoteEnding
			};
			multiplayer.broadcastSprites(Game1.getFarmer(Game1.player.UniqueMultiplayerID).currentLocation, tempAnimSpriteList);

		}*/

		/*********
		** Private methods
		*********/
		private void AfterLoad(object sender, EventArgs e) {

			emoteMenuButton = new EmoteMenuButton(Helper, Config, Data.MenuPosition);

			// Remove any duplicated EmoteMenuButton. If for some reason there is one.
			foreach(var screenMenu in Game1.onScreenMenus) {
				if(screenMenu is EmoteMenuButton) {
					Game1.onScreenMenus.Remove(screenMenu);
				}
			}

			// Add EmoteMenuButton to the screen menus
			Game1.onScreenMenus.Add(emoteMenuButton);

#if(DEBUG)
			Helper.ConsoleCommands.Trigger("world_freezetime", new string[] { "1" });
			Helper.ConsoleCommands.Trigger("world_settime", new string[] { "0900" });
#endif
		}

		private void Emote(string command, string[] args) {

			if(args.Length > 0) {

				if(int.TryParse(args[0], out int id)) {
					if(id > 0) {
						Game1.player.doEmote(id * 4);
						this.Monitor.Log($"Playing emote: {args[0]}");
					} else {
						this.Monitor.Log($"The emote id value must be grater than 0.");
					}
				}

			} else {
				this.Monitor.Log($"Missing parameters.\n\nUsage: emote <value>\n- value: a integer representing the emote id.");
			}

		}

		private void StopEmote(string command, string[] args) {

			if(Game1.player.IsEmoting) {
				this.Monitor.Log($"Stoping playing emote...");
				Game1.player.IsEmoting = false;
			} else {
				this.Monitor.Log($"No emote is playing.");
			}

		}

		private void StopAllEmotes(string command, string[] args) {

			this.Monitor.Log($"Stoping any player emotes...");
			foreach(Farmer farmer in Game1.getAllFarmers()) {
				if(farmer.IsEmoting) {
					farmer.IsEmoting = false;
				}
			}

		}

	}

}
