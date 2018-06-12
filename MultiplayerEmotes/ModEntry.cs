
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

		// TODO: Remove. Used for debugging
		public static IMonitor ModMonitor { get; private set; }

		/*
		 Emotes only visible by others with the mod.
		 The host needs to have the mod, to others with the mod use it.
		 If the host does not have the mod, it will not work.
		 */
		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;

			ModPatchControl PatchManager = new ModPatchControl(helper);
			PatchManager.PatchList.Add(new FarmerPatch());
			PatchManager.PatchList.Add(new MultiplayerPatch());
			PatchManager.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Debug);
			Config = helper.ReadConfig<ModConfig>();

			this.Monitor.Log("Loading mod data...", LogLevel.Debug);
			Data = this.Helper.ReadJsonFile<ModData>("data.json") ?? new ModData();

			SaveEvents.AfterLoad += this.AfterLoad;

			helper.ConsoleCommands.Add("emote", "Play the emote animation with the passed id.\n\nUsage: emote <value>\n- value: a integer representing the animation id.", this.Emote);
			helper.ConsoleCommands.Add("stop_emote", "Stop any playing emote.\n\nUsage: stop_emote", this.StopEmote);
			helper.ConsoleCommands.Add("stop_all_emotes", "Stop any playing emote by players.\n\nUsage: stop_all_emotes", this.StopAllEmotes);

		}

		/*********
		** Private methods
		*********/
		private void AfterLoad(object sender, EventArgs e) {

			emoteMenuButton = new EmoteMenuButton(Helper, Config, Data);

			// Remove any duplicated EmoteMenuButton. If for some reason there is one.
			foreach(var screenMenu in Game1.onScreenMenus) {
				if(screenMenu is EmoteMenuButton) {
					Game1.onScreenMenus.Remove(screenMenu);
				}
			}

			// Add EmoteMenuButton to the screen menus
			Game1.onScreenMenus.Add(emoteMenuButton);

#if(DEBUG)
			// Pause time and set it to 09:00
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
