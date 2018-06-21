
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Events;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using CustomEmojis.Framework.Patches;
using CustomEmojis.Framework;
using System.IO;
using CustomEmojis.Patches;

namespace CustomEmojis {

	public class ModEntry : Mod {

		private EmojiAssetsLoader emojiAssetsLoader;
		private ModConfig config;
		private ModData modData;

		// TODO: Remove. Used for debugging
		public static IMonitor ModMonitor { get; private set; }

#if(DEBUG)
		private ModDebugData modDebugData;
#endif
		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;

			ModPatchControl PatchControl = new ModPatchControl(helper);
			PatchControl.PatchList.Add(new MultiplayerPatch.ProcessIncomingMessagePatch());
			PatchControl.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Trace);
			this.config = helper.ReadConfig<ModConfig>();

			this.Monitor.Log("Loading mod data...", LogLevel.Trace);
			this.modData = this.Helper.ReadJsonFile<ModData>("data.json");

			if(this.modData == null) {
				this.Monitor.Log("Mod data file not found. (harmless info)", StardewModdingAPI.LogLevel.Trace);
				this.modData = new ModData();
			} else {
				this.Monitor.Log("Making checksum...", LogLevel.Trace);
				this.modData.Checksum();
			}

#if(DEBUG)
			this.Monitor.Log("Loading debug data file...", LogLevel.Trace);
			this.modDebugData = this.Helper.ReadJsonFile<ModDebugData>("debugData.json") ?? new ModDebugData();

			if(modDebugData.ActAsHost()) {
				this.Helper.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> HOST <====");
				emojiAssetsLoader = new EmojiAssetsLoader(helper, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions, this.modData.FilesChanged) {
					VanillaEmojisTexture = helper.Content.Load<Texture2D>("LooseSprites\\emojis", ContentSource.GameContent)
				};
			} else {
				this.Helper.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> CLIENT <====");
				emojiAssetsLoader = new EmojiAssetsLoader(helper, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions, true) {
					VanillaEmojisTexture = helper.Content.Load<Texture2D>("LooseSprites\\emojis", ContentSource.GameContent),
					InputFolder = "spritesCLIENT",
					OutputFolder = "mergedSpriteCLIENT"
				};
			}
#else
			emojiAssetsLoader = new EmojiAssetsLoader(helper, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions, this.modData.FilesChanged) {
				VanillaEmojisTexture = helper.Content.Load<Texture2D>("LooseSprites\\emojis", ContentSource.GameContent)
			};
#endif

			helper.Content.AssetLoaders.Add(emojiAssetsLoader);

			SaveEvents.AfterLoad += this.OnAfterLoad;

			MultiplayerExtension.OnRecieveEmojiTexture += MultiplayerExtension_OnRecieveEmojiTexture;
			MultiplayerExtension.OnRecieveEmojiTextureRequest += MultiplayerExtension_OnRecieveEmojiTextureRequest;
		}

		private void MultiplayerExtension_OnRecieveEmojiTexture(object sender, RecievedEmojiTextureEventArgs e) {
			emojiAssetsLoader.CustomEmojisTexture = e.EmojiTexture;
			Helper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(e.EmojiTexture);
			ChatBox.emojiTexture = e.EmojiTexture;
			EmojiMenu.totalEmojis = e.NumberEmojis;
			//Monitor.Log($"AssetKey: {Helper.Content.GetActualAssetKey(Path.Combine(emojiAssetsLoader.OutputFolder, emojiAssetsLoader.OutputFile))}");
			//string assetKey = Helper.Content.GetActualAssetKey(Path.Combine(emojiAssetsLoader.OutputFolder, emojiAssetsLoader.OutputFile));
			//Helper.Content.InvalidateCache(assetKey);
			//Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(assetKey));
			//Helper.Content.InvalidateCache(@"LooseSprites\emojis.xnb");
			//Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(@"LooseSprites\emojis.xnb"));
		}

		private void MultiplayerExtension_OnRecieveEmojiTextureRequest(object sender, RecievedEmojiTextureRequestEventArgs e) {
			this.Monitor.Log("OnRecieveEmojiTextureRequest...");
			Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.ResponseEmojiTexture(e.SourceFarmer, emojiAssetsLoader.CustomEmojisTexture, EmojiMenu.totalEmojis);
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


			if(modData.FilesChanged) {
				this.Monitor.Log("File changes detected. Saving mod data...", LogLevel.Trace);
				modData.EmojisAdded = emojiAssetsLoader.NumberCustomEmojisAdded;
				modData.UpdateFilesChecksum(this.Helper.DirectoryPath, config.ImageExtensions);
				modData.FilesChanged = false;
				this.Helper.WriteJsonFile("data.json", modData);
			} else {
				this.Monitor.Log("No file changes detected.", LogLevel.Trace);
				emojiAssetsLoader.NumberCustomEmojisAdded = modData.EmojisAdded;
			}

			this.Monitor.Log($"Custom emojis added: {emojiAssetsLoader.CustomEmojisAdded}");
			if(emojiAssetsLoader.CustomEmojisAdded) {
				this.Monitor.Log($"Custom emojis found: {emojiAssetsLoader.NumberCustomEmojisAdded}");
				this.Monitor.Log($"Total emojis counted by Stardew Valley: {EmojiMenu.totalEmojis}");
				//int a = (int)Math.Ceiling((double)emojiAssetsLoader.NumberEmojisAdded / 14);
				//int emojiPerRow = (emojiAssetsLoader.VanillaEmojisTexture.Width / emojiAssetsLoader.EmojisSize);
				//int numberCustomEmojis = (emojiAssetsLoader.NumberEmojisAdded - emojiPerRow * a);
				//EmojiMenu.totalEmojis += numberCustomEmojis;
				EmojiMenu.totalEmojis += (emojiAssetsLoader.NumberCustomEmojisAdded - ((emojiAssetsLoader.VanillaEmojisTexture.Width / emojiAssetsLoader.EmojisSize) * (int)Math.Ceiling((double)emojiAssetsLoader.NumberCustomEmojisAdded / 14)));
				this.Monitor.Log($"Total emojis counted after ammount fix: {EmojiMenu.totalEmojis}");

			}

			if(!Context.IsMainPlayer && emojiAssetsLoader.CustomEmojisAdded) {
				Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
				multiplayer.RequestEmojiTexture();
			}

		}

		/// <summary>Reload the game emojis with new ones found in the 'CustomEmojis\sprites\' folder when the 'reloadEmojis' command is invoked.</summary>
		/// <param name="command">The name of the command invoked.</param>
		/// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
		private void ReloadEmojis(string command, string[] args) {
			this.Monitor.Log($"Reloading emoji assets...");
			this.Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(@"LooseSprites\\emojis.xnb"));
		}

	}

}
