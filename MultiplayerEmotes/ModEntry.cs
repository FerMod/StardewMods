
using System;
using MultiplayerEmotes.Framework;
using MultiplayerEmotes.Framework.Network;
using MultiplayerEmotes.Framework.Patches;
using MultiplayerEmotes.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MultiplayerEmotes {

	public class ModEntry : Mod {

		public static ModConfig Config { get; private set; }
		public static ModData Data { get; private set; }
		public static MultiplayerModMessage MultiplayerMessage { get; private set; }

		private EmotesMenuButton emoteMenuButton;

		public static IMonitor ModMonitor { get; private set; }
		public static IModHelper ModHelper { get; private set; }

		/*
		 Emotes only visible by others with the mod.
		 The host needs to have the mod, to others with the mod use it.
		 If the host does not have the mod, it will not work.
		 */
		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;
			ModHelper = Helper;
			MultiplayerMessage = new MultiplayerModMessage(helper);

			ModPatchControl PatchContol = new ModPatchControl(helper);
			PatchContol.PatchList.Add(new FarmerPatch.DoEmotePatch(helper.Reflection));
			PatchContol.PatchList.Add(new CharacterPatch.DoEmotePatch(helper.Reflection));
			PatchContol.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Debug);
			Config = helper.ReadConfig<ModConfig>();

			this.Monitor.Log("Loading mod data...", LogLevel.Debug);
			Data = this.Helper.Data.ReadJsonFile<ModData>("data.json") ?? new ModData();

			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

			// TODO: Command to stop emotes from NPC and FarmAnimals
			helper.ConsoleCommands.Add("emote", "Play the emote animation with the passed id.\n\nUsage: emote <value>\n- value: a integer representing the animation id.", this.Emote);
			helper.ConsoleCommands.Add("stop_emote", "Stop any emote being played by you.\n\nUsage: stop_emote", this.StopEmote);
			helper.ConsoleCommands.Add("stop_all_emotes", "Stop any emote being played.\n\nUsage: stop_all_emotes", this.StopAllEmotes);

		}

		/*********
		** Private methods
		*********/
		/// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {

			emoteMenuButton = new EmotesMenuButton(Helper, Config, Data);

			// Add EmoteMenuButton to the screen menus
			Game1.onScreenMenus.Insert(0, emoteMenuButton);

#if DEBUG
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
						this.Monitor.Log($"Playing emote: {id}");
#if DEBUG
					} else if(id < 0) {
						EmoteTemporaryAnimation emoteTempAnim = new EmoteTemporaryAnimation(Helper.Reflection, Helper.Events);
						emoteTempAnim.BroadcastEmote(id * -1);
						this.Monitor.Log($"Playing emote (workarround): {id * -1}");
#endif
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
