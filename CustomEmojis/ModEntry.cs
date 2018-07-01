
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Events;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Diagnostics;
using CustomEmojis.Framework.Patches;
using CustomEmojis.Framework;
using CustomEmojis.Patches;
using System.Collections.Generic;
using CustomEmojis.Framework.Constants;
using Newtonsoft.Json;
using System.IO;

namespace CustomEmojis {

	public class ModEntry : Mod {

		private EmojiAssetsLoader emojiAssetsLoader;
		private ModConfig config;
		private ModData modData;

		// TODO: Remove. Used for debugging
		public static IMonitor ModMonitor { get; private set; }

		CustomEmojiDrawer ced;

#if(DEBUG)
		private ModDebugData modDebugData;
#endif
		public override void Entry(IModHelper helper) {

			ModMonitor = Monitor;

#if(DEBUG)
			Logger.SetOutput(helper.DirectoryPath + "\\logfile.txt", Monitor);
#endif
			ModPatchControl PatchControl = new ModPatchControl(helper);
			PatchControl.PatchList.Add(new MultiplayerPatch.ProcessIncomingMessagePatch());
			PatchControl.ApplyPatch();

			this.Monitor.Log("Loading mod config...", LogLevel.Trace);
			this.config = helper.ReadConfig<ModConfig>();

			this.Monitor.Log("Loading mod data...", LogLevel.Trace);
			this.modData = this.Helper.ReadJsonFile<ModData>(FilePaths.Data);
			if(this.modData == null) {
				this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
				this.modData = new ModData(this.Helper.DirectoryPath) {
					WatchedPaths = new List<string>() {
						Assets.InputFolder
					}
				};
			}
			//modData.FilesChecksums.TryAdd("path", "hash");
			this.Helper.WriteJsonFile(FilePaths.Data, modData);

			Monitor.Log($"[ModEntry] Timer started!", LogLevel.Trace);
			Stopwatch sw = new Stopwatch();
			sw.Start();
			if(this.modData == null) {
				this.Monitor.Log("Mod data file not found. (harmless info)", LogLevel.Trace);
				this.modData = new ModData(this.Helper.DirectoryPath) {
					WatchedPaths = new List<string>() {
						Assets.InputFolder
					}
				};
			} else {
				this.Monitor.Log("Making checksum...", LogLevel.Trace);
				this.modData.Checksum(config.ImageExtensions);
			}
			sw.Stop();
			Monitor.Log($"[ModEntry] Timer Stoped! Elapsed time: {sw.Elapsed}");

#if(!DEBUG)
			this.Monitor.Log("Loading debug data file...", LogLevel.Trace);
			this.modDebugData = this.Helper.ReadJsonFile<ModDebugData>("debugData.json") ?? new ModDebugData();

			if(modDebugData.ActAsHost()) {
				this.Helper.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> HOST <====");
				emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions, this.modData.FilesChanged);
			} else {
				this.Helper.WriteJsonFile("debugData.json", modDebugData);
				Monitor.Log($"====> CLIENT <====");
				Assets.InputFolder = Assets.InputFolder + "CLIENT";
				Assets.OutputFolder = Assets.OutputFile + "CLIENT";
				emojiAssetsLoader = new EmojiAssetsLoader(helper, modData,  EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions, true);
			}
#else
			emojiAssetsLoader = new EmojiAssetsLoader(helper, modData, EmojiMenu.EMOJI_SIZE, this.config.ImageExtensions, this.modData.FilesChanged, true);
#endif

			helper.Content.AssetLoaders.Add(emojiAssetsLoader);

			helper.ConsoleCommands.Add("reload_emojis", "Reload the game emojis with the new ones found in the mod folder.", this.ReloadEmojis);

			SaveEvents.AfterLoad += this.OnAfterLoad;

			MultiplayerExtension.OnRecieveEmojiTexture += MultiplayerExtension_OnRecieveEmojiTexture;
			MultiplayerExtension.OnRecieveEmojiTextureRequest += MultiplayerExtension_OnRecieveEmojiTextureRequest;

			GraphicsEvents.OnPostRenderEvent += GraphicsEvents_OnPostRenderEvent;
			GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;

		}

		private void GameEvents_UpdateTick(object sender, EventArgs e) {
			if(Context.IsWorldReady && Game1.chatBox != null) {
				List<ChatMessage> messages = Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
				int num = 0;
				for(int i = messages.Count - 1; i >= 0; --i) {
					ChatMessage message = messages[i];
					num += message.verticalSize;
					//message.draw(b, 12, this.yPositionOnScreen - num - 8 + (this.chatBox.Selected ? 0 : this.chatBox.Height));
					Monitor.Log($"Draw custom emojis");
				}
				//ced = new CustomEmojiDrawer(Helper.Reflection, emojiAssetsLoader.VanillaEmojisTexture, EmojiMenu.totalEmojis);// emojiAssetsLoader.CustomEmojisTexture);
			}
		}

		private void GameEvents_FirstUpdateTick(object sender, EventArgs e) {

		}

		private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e) {
			//if(Context.IsWorldReady) {
			//	if(ced == null) {
			//		ced = new CustomEmojiDrawer(Helper.Reflection, emojiAssetsLoader.VanillaEmojisTexture, EmojiMenu.totalEmojis);// emojiAssetsLoader.CustomEmojisTexture);
			//	}
			//}
		}

		private void MultiplayerExtension_OnRecieveEmojiTexture(object sender, RecievedEmojiTextureEventArgs e) {
			emojiAssetsLoader.CustomTexture = e.EmojiTexture;
			//Helper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(e.EmojiTexture);
			//ChatBox.emojiTexture = e.EmojiTexture;
			//EmojiMenu.totalEmojis = e.NumberEmojis;
			//Monitor.Log($"AssetKey: {Helper.Content.GetActualAssetKey(Path.Combine(emojiAssetsLoader.OutputFolder, emojiAssetsLoader.OutputFile))}");
			//string assetKey = Helper.Content.GetActualAssetKey(Path.Combine(emojiAssetsLoader.OutputFolder, emojiAssetsLoader.OutputFile));
			//Helper.Content.InvalidateCache(assetKey);
			//Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(assetKey));
			Helper.Content.InvalidateCache("LooseSprites\\emojis");
			//Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && asset.AssetNameEquals(@"LooseSprites\emojis.xnb"));
		}

		private void MultiplayerExtension_OnRecieveEmojiTextureRequest(object sender, RecievedEmojiTextureRequestEventArgs e) {
			this.Monitor.Log("OnRecieveEmojiTextureRequest...");
			Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.ResponseEmojiTexture(e.SourceFarmer, emojiAssetsLoader.CustomTexture, EmojiMenu.totalEmojis);
		}

		/*********
		** Private methods
		*********/

		/// <summary>The method called after the player loads their save.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnAfterLoad(object sender, EventArgs e) {
			if(Game1.chatBox != null) {
				// Check if is exact type
				//if(Game1.chatBox.GetType() == typeof(ChatBox)) {
				//string typeName = "System.String";
				//Type typeArgument = Type.GetType(Game1.chatBox.GetType().FullName);

				//Type genericClass = typeof(ChatBoxOverride<>);
				//// MakeGenericType is badly named
				//Type constructedClass = genericClass.MakeGenericType(Game1.chatBox.GetType());

				//object created = Activator.CreateInstance(constructedClass);

				//Type type = typeof(Game1).MakeGenericType(Game1.chatBox.GetType());
				//var context = Activator.CreateInstance(type);
				//object o = Activator.CreateInstance(constructedClass);
				//chatBoxOverride = ModUtilities.Construct<ChatBoxOverride<>>(typeof(ChatBox));
				//Game1.chatBox = chatBoxOverride;
				//Game1.chatBox = ModUtilities.Construct<ChatBox, ChatBoxOverride>(Game1.chatBox);
				//var chatbox = ModUtilities.Construct<ChatBoxOverride>(typeof(ChatBox), Game1.chatBox);
				//Game1.chatBox = chatbox;
				//} else {
				//	Monitor.Log($"[{Game1.chatBox}]: This isn't ChatBox Type :(");
				//}
			}
#if(DEBUG)
			// Pause time and set it to 09:00
			Helper.ConsoleCommands.Trigger("world_freezetime", new string[] { "1" });
			Helper.ConsoleCommands.Trigger("world_settime", new string[] { "0900" });
#endif

			Stopwatch sw = new Stopwatch();
			Monitor.Log($"Timer started!");
			sw.Start();
			//modData.UpdateFilesChecksum(this.Helper.DirectoryPath, config.ImageExtensions);
			if(modData.Checksum(config.ImageExtensions)) {
				this.Monitor.Log("File changes detected. Saving mod data...", LogLevel.Trace);
				//emojiAssetsLoader.UpdateTotalEmojis();
				//modData.EmojisAdded = emojiAssetsLoader.NumberCustomEmojisAdded;
				//emojiAssetsLoader.ShouldGenerateTexture = true;
				emojiAssetsLoader.ReloadAsset(); // FIXME: Cache not invalidating properly
			} else {
				this.Monitor.Log("No file changes detected.", LogLevel.Trace);
				emojiAssetsLoader.NumberCustomEmojisAdded = modData.FilesChecksums.Count;
			}

			Monitor.Log($"[After checksum] Time elapsed: {sw.Elapsed}");
			sw.Reset();

			this.Monitor.Log($"Custom emojis added: {emojiAssetsLoader.CustomTextureAdded}");
			if(emojiAssetsLoader.CustomTextureAdded) {
				this.Monitor.Log($"Custom emojis found: {emojiAssetsLoader.NumberCustomEmojisAdded}");
				emojiAssetsLoader.UpdateTotalEmojis();
				this.Monitor.Log($"Total emojis counted by Stardew Valley: {EmojiMenu.totalEmojis}");
				this.Monitor.Log($"Total emojis counted after ammount fix: {emojiAssetsLoader.TotalNumberEmojis}");
			}

			if(modData.ShouldSaveData()) {
				modData.FilesChanged = false;
				modData.DataChanged = false;
				this.Helper.WriteJsonFile(FilePaths.Data, modData);
			}

			//if(emojiAssetsLoader.CustomEmojisAdded) {
			//	this.Monitor.Log($"Custom emojis found: {emojiAssetsLoader.NumberCustomEmojisAdded}");
			//	this.Monitor.Log($"Total emojis counted by Stardew Valley: {EmojiMenu.totalEmojis}");
			//	//int a = (int)Math.Ceiling((double)emojiAssetsLoader.NumberEmojisAdded / 14);
			//	//int emojiPerRow = (emojiAssetsLoader.VanillaEmojisTexture.Width / emojiAssetsLoader.EmojisSize);
			//	//int numberCustomEmojis = (emojiAssetsLoader.NumberEmojisAdded - emojiPerRow * a);
			//	//EmojiMenu.totalEmojis += numberCustomEmojis;
			//	emojiAssetsLoader.UpdateTotalEmojis();
			//	this.Monitor.Log($"Total emojis counted after ammount fix: {emojiAssetsLoader.TotalNumberEmojis}");

			//}
			Monitor.Log($"[After UpdateTotalEmojis]Time elapsed: {sw.Elapsed}");
			sw.Reset();


			if(!Context.IsMainPlayer && emojiAssetsLoader.CustomTextureAdded) {
				Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
				multiplayer.RequestEmojiTexture();
			}
			sw.Stop();
			Monitor.Log($"[After Request] Time elapsed: {sw.Elapsed}");
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
