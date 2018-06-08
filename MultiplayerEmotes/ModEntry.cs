
using StardewModdingAPI;
using StardewValley;
using MultiplayerEmotes.Patches;
using StardewModdingAPI.Events;
using System;
using Microsoft.Xna.Framework;
using MultiplayerEmotes.Menus;
using MultiplayerEmotes.Events;

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
			PatchManager.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Trace);
			Config = helper.ReadConfig<ModConfig>();

			this.Monitor.Log("Loading mod data...", LogLevel.Trace);
			Data = this.Helper.ReadJsonFile<ModData>("data.json") ?? new ModData();

			SaveEvents.AfterLoad += this.AfterLoad;
			SaveEvents.AfterReturnToTitle += this.AfterReturnToTitle;
			GameEvents.UpdateTick += MouseStateMonitor.UpdateMouseState;
			InputEvents.ButtonPressed += this.ButtonPressed;

			helper.ConsoleCommands.Add("emote", "Play the emote animation with the passed id.\n\nUsage: emote <value>\n- value: a integer representing the animation id.", this.Emote);
			helper.ConsoleCommands.Add("stop_emote", "Stop any playing emote.\n\nUsage: stop_emote", this.StopEmote);
			helper.ConsoleCommands.Add("stop_all_emotes", "Stop any playing emote by players.\n\nUsage: stop_all_emotes", this.StopAllEmotes);

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


		/*********
		** Private methods
		*********/

		private void AfterLoad(object sender, EventArgs e) {

			emoteMenuButton = new EmoteMenuButton(Helper, Data.MenuPosition, Config.AnimatedIcon);
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
