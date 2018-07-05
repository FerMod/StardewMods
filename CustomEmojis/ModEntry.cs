
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using CustomEmojis.Framework.Patches;
using CustomEmojis.Framework;
using CustomEmojis.Patches;
using CustomEmojis.Framework.Constants;
using Microsoft.Xna.Framework.Graphics;

namespace CustomEmojis {

	public class ModEntry : Mod {

		private EmojiAssetsLoader emojiAssetsLoader;
		private ModConfig config;
		private ModData modData;

		// TODO: Remove. Used for debugging
		public static IMonitor ModMonitor { get; private set; }
		public static Logger ModLogger { get; private set; }

#if(DEBUG)
		private ModDebugData modDebugData;
#endif
		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;

#if(DEBUG)
			//Logger.InitLogger(helper.DirectoryPath + "\\logfile.txt", false, Monitor);
#endif
			ModPatchControl PatchControl = new ModPatchControl(helper);
			PatchControl.PatchList.Add(new MultiplayerPatch.ProcessIncomingMessagePatch());
			PatchControl.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Trace);
			this.config = helper.ReadConfig<ModConfig>();
#if !DEBUG
			this.Monitor.Log("Loading mod data...", LogLevel.Trace);
			this.modData = this.Helper.ReadJsonFile<ModData>(FilePaths.Data);
			if(this.modData == null) {
				this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
				this.modData = new ModData(helper, config.ImageExtensions) {
					WatchedPaths = new List<string>() {
						Assets.InputFolder
					}
				};
			} else {
				modData.FileExtensionsFilter = config.ImageExtensions;
				modData.ModHelper = helper;
			}
#endif
#if(DEBUG)
			this.Monitor.Log("Loading debug data file...", LogLevel.Trace);
			this.modDebugData = this.Helper.ReadJsonFile<ModDebugData>("debugData.json") ?? new ModDebugData();

			if(modDebugData.ActAsHost()) {

				this.Helper.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> HOST <====");

				ModLogger = new Logger(helper.DirectoryPath + "\\logfile.txt", false, Monitor);
				this.Monitor.Log("Loading mod data...", LogLevel.Trace);
				this.modData = this.Helper.ReadJsonFile<ModData>(FilePaths.Data);
				if(this.modData == null) {
					this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
					this.modData = new ModData(helper, config.ImageExtensions) {
						WatchedPaths = new List<string>() {
						Assets.InputFolder
					}
					};
				} else {
					modData.FileExtensionsFilter = config.ImageExtensions;
					modData.ModHelper = helper;
				}

				emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions);

				if(Game1.activeClickableMenu is CoopMenu coopMenu) {
					Helper.Reflection.GetField<CoopMenu.Tab>(coopMenu, "currentTab").SetValue(CoopMenu.Tab.HOST_TAB);
				}

			} else {
				this.Helper.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> CLIENT <====");
				ModLogger = new Logger(helper.DirectoryPath + "\\logfileClient.txt", false, Monitor);
				Assets.InputFolder = Assets.InputFolder + "CLIENT";
				Assets.OutputFolder = Assets.OutputFolder + "CLIENT";
				FilePaths.Data = "dataCLIENT.json";

				this.Monitor.Log("Loading mod data...", LogLevel.Trace);
				this.modData = this.Helper.ReadJsonFile<ModData>(FilePaths.Data);
				if(this.modData == null) {
					this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
					this.modData = new ModData(helper, config.ImageExtensions) {
						WatchedPaths = new List<string>() {
						Assets.InputFolder
					}
					};
				} else {
					modData.FileExtensionsFilter = config.ImageExtensions;
					modData.ModHelper = helper;
				}

				emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions);

				if(Game1.activeClickableMenu is CoopMenu coopMenu) {
					Helper.Reflection.GetField<CoopMenu.Tab>(coopMenu, "currentTab").SetValue(CoopMenu.Tab.JOIN_TAB);
				}

			}
#else
			emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions);
#endif

			helper.Content.AssetLoaders.Add(emojiAssetsLoader);

			helper.ConsoleCommands.Add("reload_emojis", "Reload the game emojis with the new ones found in the mod folder.", this.ReloadEmojis);

			SaveEvents.AfterLoad += this.OnAfterLoad;

			//MultiplayerExtension.OnReceiveEmojiTexture += MultiplayerExtension_OnRecieveEmojiTexture;
			MultiplayerExtension.OnReceiveEmojiTextureRequest += MultiplayerExtension_OnRecieveEmojiTextureRequest;
			//MultiplayerExtension.OnReceiveEmojiTextureData += MultiplayerExtension_OnReceiveEmojiTextureData;

			MultiplayerExtension.OnPlayerConnected += MultiplayerExtension_OnPlayerConnected;
			MultiplayerExtension.OnPlayerDisconnected += MultiplayerExtension_OnPlayerDisconnected;

		}

		private void MultiplayerExtension_OnPlayerConnected(object sender, PlayerConnectedEventArgs e) {
			Monitor.Log($"Connected player: {e.Player.Name}[{e.Player.UniqueMultiplayerID}]");
		}

		private void MultiplayerExtension_OnPlayerDisconnected(object sender, PlayerDisconnectedEventArgs e) {
			Monitor.Log($"Disconnected player: {e.Player.Name}[{e.Player.UniqueMultiplayerID}]");
		}

		private void MultiplayerExtension_OnRecieveEmojiTexture(object sender, ReceivedEmojiTextureEventArgs e) {
			emojiAssetsLoader.CurrentTexture = e.EmojiTexture;
			Helper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(e.EmojiTexture);
			ChatBox.emojiTexture = e.EmojiTexture;
			EmojiMenu.totalEmojis = e.NumberEmojis;
			//emojiAssetsLoader.UpdateTotalEmojis();
			//Monitor.Log($"AssetKey: {Helper.Content.GetActualAssetKey(Path.Combine(emojiAssetsLoader.OutputFolder, emojiAssetsLoader.OutputFile))}");
			//string assetKey = Helper.Content.GetActualAssetKey(Path.Combine(emojiAssetsLoader.OutputFolder, emojiAssetsLoader.OutputFile));
			//Helper.Content.InvalidateCache(assetKey);
			//Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(assetKey));
			//Helper.Content.InvalidateCache("LooseSprites\\emojis");
			//Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(@"LooseSprites\emojis.xnb"));
		}

		private void MultiplayerExtension_OnRecieveEmojiTextureRequest(object sender, ReceivedEmojiTextureRequestEventArgs e) {
			this.Monitor.Log("OnRecieveEmojiTextureRequest...");
			Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.ResponseEmojiTexture(e.SourceFarmer, emojiAssetsLoader.CustomTexture, EmojiMenu.totalEmojis);
		}

		private void MultiplayerExtension_OnReceiveEmojiTextureData(object sender, ReceivedEmojiTextureDataEventArgs e) {
			ModLogger.LogTrace();
			EmojiMenu.totalEmojis += e.TextureDataList.Count;
			emojiAssetsLoader.LoadedTextureData = e.TextureDataList;
			//emojiAssetsLoader.UpdateTotalEmojis();
			emojiAssetsLoader.CurrentTexture = emojiAssetsLoader.MergeTextures(emojiAssetsLoader.VanillaTexture, emojiAssetsLoader.LoadedTextureData);
			Helper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(emojiAssetsLoader.CurrentTexture);
			ChatBox.emojiTexture = emojiAssetsLoader.CurrentTexture;
			Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.BroadcastEmojiTexture(emojiAssetsLoader.CurrentTexture, EmojiMenu.totalEmojis);
		}

		/*********
		** Private methods
		*********/

		/// <summary>The method called after the player loads their save.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnAfterLoad(object sender, EventArgs e) {

#if(DEBUG)
			// Pause time and set it to 09:00
			Helper.ConsoleCommands.Trigger("world_freezetime", new string[] { "1" });
			Helper.ConsoleCommands.Trigger("world_settime", new string[] { "0900" });
#endif

			if(modData.ShouldSaveData()) {
				this.Monitor.Log("File changes detected. Saving mod data...", LogLevel.Trace);
				modData.FilesChanged = false;
				modData.DataChanged = false;
				this.Helper.WriteJsonFile(FilePaths.Data, modData);
				//emojiAssetsLoader.ReloadAsset(); // FIXME: Cache not invalidating properly
			} else {
				this.Monitor.Log("No file changes detected.", LogLevel.Trace);
			}

			this.Monitor.Log($"Custom emojis added: {emojiAssetsLoader.CustomTextureAdded}");
			if(emojiAssetsLoader.CustomTextureAdded) {
				this.Monitor.Log($"Custom emojis found: {emojiAssetsLoader.NumberCustomEmojisAdded}");
				this.Monitor.Log($"Total emojis counted by Stardew Valley: {EmojiMenu.totalEmojis}");
				emojiAssetsLoader.UpdateTotalEmojis();
				this.Monitor.Log($"Total emojis counted after ammount fix: {emojiAssetsLoader.TotalNumberEmojis}");
			}

			//if(!Context.IsMainPlayer && emojiAssetsLoader.CustomTextureAdded) {
			//	Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			//	multiplayer.RequestEmojiTexture();
			//}

			//if(!Context.IsMainPlayer && emojiAssetsLoader.CustomTextureAdded) {
			//	Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			//	//multiplayer.SendEmojisTextureDataList(Game1.MasterPlayer, emojiAssetsLoader.LoadedTextureData[Game1.player.UniqueMultiplayerID]);
			//}

			emojiAssetsLoader.SyncTextureData();

		}

		/// <summary>Reload the game emojis with the new ones found in the mod folder.</summary>
		/// <param name="command">The name of the command invoked.</param>
		/// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
		private void ReloadEmojis(string command, string[] args) {
			this.Monitor.Log($"Reloading emoji assets...");

			Assets.InputFolder = Assets.OutputFolder + "CLIENT";

			bool cacheInvalidated = Helper.Content.InvalidateCache("LooseSprites/emojis");
			//bool cacheInvalidated = Helper.Content.InvalidateCache(Helper.Content.GetActualAssetKey($"{Assets.OutputFolder}/{Assets.OutputFile}"));
			emojiAssetsLoader.UpdateTotalEmojis();
			//bool invalidated = Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(@"emojis"));
			this.Monitor.Log($"CacheInvalidated: {cacheInvalidated}");

			/*
			Helper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(Helper.Content.Load<Texture2D>($"{Assets.OutputFolder}/{Assets.OutputFile}"));
			ChatBox.emojiTexture = Helper.Content.Load<Texture2D>($"{Assets.OutputFolder}/{Assets.OutputFile}");
			*/


			//Helper.Content.InvalidateCache<Texture2D>();

			//this.Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(@"LooseSprites\\emojis.xnb"));
		}

	}

}
