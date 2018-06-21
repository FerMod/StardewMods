
using StardewValley;
using StardewValley.Network;
using Microsoft.Xna.Framework.Graphics;
using System;
using CustomEmojis.Framework.Events;
using CustomEmojis.Framework.Constants;
using CustomEmojis.Framework.Network;

namespace CustomEmojis.Framework.Extensions {

	public static class MultiplayerExtension {

		public static event EventHandler<RecievedEmojiTextureEventArgs> OnRecieveEmojiTexture = delegate { };
		public static event EventHandler<RecievedEmojiTextureRequestEventArgs> OnRecieveEmojiTextureRequest = delegate { };

		public static void BroadcastEmojiTexture(this Multiplayer multiplayer, Texture2D texture, int numberEmojis) {

			if(Game1.IsMultiplayer) {
				object[] objArray = new object[3] {
					Message.Action.EmojiTextureBroadcast.ToString(),
					numberEmojis,
					//TextureConverter.TextureToByteArray(texture)
					DataSerialization.Serialize(new TextureData(texture))
				};
				OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);
				if(Game1.IsServer) {
					foreach(Farmer farmer in Game1.getAllFarmers()) {
						if(farmer != Game1.player) {
							Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
						}
					}
				} else if(Game1.IsClient) {
					foreach(Farmer farmer in Game1.getAllFarmers()) {
						if(farmer != Game1.player) {
							Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
						}
					}
				}

			}

		}

		public static void RecieveEmojiTextureBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Game1.IsMultiplayer && msg.Data.Length > 0) {
				RecievedEmojiTextureEventArgs args = new RecievedEmojiTextureEventArgs {
					SourceFarmer = msg.SourceFarmer,
					NumberEmojis = msg.Reader.ReadInt32(),
					//EmojiTexture = TextureConverter.ByteArrayToTexture2D(msg.Reader)
					EmojiTexture = DataSerialization.Deserialize<TextureData>(msg.Reader.BaseStream).GetTexture()
				};
				OnRecieveEmojiTexture(null, args);
			}
		}

		public static void RequestEmojiTexture(this Multiplayer multiplayer) {
			if(Game1.IsMultiplayer) {
				foreach(Farmer farmer in Game1.getAllFarmers()) {
					if(farmer.IsMainPlayer) {
						multiplayer.RequestEmojiTexture(farmer);
					}
				}
			}
		}

		public static void RequestEmojiTexture(this Multiplayer multiplayer, Farmer farmer) {
			multiplayer.RequestEmojiTexture(farmer.UniqueMultiplayerID);
		}

		public static void RequestEmojiTexture(this Multiplayer multiplayer, long peerId) {
			if(Game1.IsMultiplayer) {
				object[] objArray = new object[1] {
					Message.Action.EmojiTextureRequest.ToString()
				};
				OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);
				if(Game1.IsClient) {
					Game1.client.sendMessage(message);
				} else if(Game1.IsServer) {
					Game1.server.sendMessage(peerId, message);
				}
			}
		}

		public static void RecieveEmojiTextureRequest(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Game1.IsMultiplayer && msg.Data.Length > 0) {
				RecievedEmojiTextureRequestEventArgs args = new RecievedEmojiTextureRequestEventArgs {
					SourceFarmer = msg.SourceFarmer
				};
				OnRecieveEmojiTextureRequest(null, args);
			}
		}

		public static void ResponseEmojiTexture(this Multiplayer multiplayer, Farmer farmer, Texture2D texture, int numberEmojis) {
			multiplayer.ResponseEmojiTexture(farmer.UniqueMultiplayerID, texture, numberEmojis);
		}

		public static void ResponseEmojiTexture(this Multiplayer multiplayer, long peerId, Texture2D texture, int numberEmojis) {
			if(Game1.IsMultiplayer) {
				object[] objArray = new object[3] {
					Message.Action.EmojiTextureResponse.ToString(),
					numberEmojis,
					//TextureConverter.TextureToByteArray(texture)
					DataSerialization.Serialize(new TextureData(texture))
				};

				OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);
				if(Game1.IsClient) {
					Game1.client.sendMessage(message);
				} else if(Game1.IsServer) {
					Game1.server.sendMessage(peerId, message);
				}
			}
		}

		public static void RecieveEmojiTextureResponse(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Game1.IsMultiplayer && msg.Data.Length > 0) {
				RecievedEmojiTextureEventArgs args = new RecievedEmojiTextureEventArgs {
					SourceFarmer = msg.SourceFarmer,
					NumberEmojis = msg.Reader.ReadInt32(),
					//EmojiTexture = TextureConverter.ByteArrayToTexture2D(msg.Reader)
					EmojiTexture = DataSerialization.Deserialize<TextureData>(msg.Reader.BaseStream).GetTexture()
				};
				OnRecieveEmojiTexture(null, args);
			}
		}

	}

}
