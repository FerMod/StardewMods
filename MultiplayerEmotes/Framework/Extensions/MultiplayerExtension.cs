﻿
using MultiplayerEmotes.Framework;
using StardewValley;
using StardewValley.Network;

namespace MultiplayerEmotes.Extensions {

	public static class MultiplayerExtension {

		public static void BroadcastEmote(this Multiplayer multiplayer, int emoteIndex) {

			if(Game1.IsMultiplayer) {
				ModEntry.ModMonitor.Log($"Extens: {Constants.Network.MessageAction.EmoteBroadcast.ToString()}");
				object[] objArray = new object[2] {
					Constants.Network.MessageAction.EmoteBroadcast.ToString(),
					emoteIndex
				};
				OutgoingMessage message = new OutgoingMessage(Constants.Network.MessageTypeID, Game1.player, objArray);
				if(Game1.IsClient) {
					Game1.client.sendMessage(message);
				} else {
					foreach(Farmer farmer in Game1.getAllFarmers()) {
						if(farmer != Game1.player) {
							Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
						}
					}

				}

			}

		}

		public static void ProcessBroadcastEmote(this Multiplayer multiplayer, IncomingMessage msg) {
			if(msg.Data.Length >= 0) {
				int emoteIndex = msg.Reader.ReadInt32();
				msg.SourceFarmer.IsEmoting = false;
				msg.SourceFarmer.doEmote(emoteIndex);
			}
		}

	}

}
