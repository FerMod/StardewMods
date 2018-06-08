
using StardewValley;
using StardewValley.Network;

//// Define an interface named IMyInterface.
//namespace DefineIMultiplayerExtension {

//	using System;

//	public interface IMultiplayerExtension {

//		// Any class that implements IMyInterface must define a method
//		// that matches the following signature.
//		void processIncomingMessage(IncomingMessage msg);

//		string testMethod();

//	}
//}

namespace MultiplayerEmotes.Extensions {

	using Microsoft.Xna.Framework.Graphics;
	using StardewValley.Menus;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;

	public static class MultiplayerExtension {

		public static void broadcastTexture(this Multiplayer multiplayer, Texture2D texture) {

			if(texture != null && Game1.IsMultiplayer) {

				//using (MemoryStream memoryStream = new MemoryStream()) {

				OutgoingMessage message = new OutgoingMessage(20, Game1.player, ToByteArray(texture));
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

		public static void requestTexture(this Multiplayer multiplayer, string key) {

			if(!String.IsNullOrWhiteSpace(key) && Game1.IsMultiplayer) {

				OutgoingMessage message = new OutgoingMessage(21, Game1.player, key);
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

		public static void broadcastEmote(this Multiplayer multiplayer, int emoteIndex) {

			if(Game1.IsMultiplayer) {
				object[] objArray = new object[2] {
					"EmoteBroadcast",
					emoteIndex
				};
				OutgoingMessage message = new OutgoingMessage(22, Game1.player, objArray);
				if(Game1.IsClient) {
					Game1.client.sendMessage(message);
				} else {
					foreach(Farmer farmer in Game1.getAllFarmers()) {
						if(farmer != Game1.player) {
							Game1.server.sendMessage(farmer.UniqueMultiplayerID, new OutgoingMessage(22, Game1.player, objArray));
						}
					}

				}

			}

		}

		public static void ProcessBroadcastTexture(this Multiplayer multiplayer, IncomingMessage msg) {
			if(msg.Data.Length != 0) {
				using(MemoryStream ms = new MemoryStream(msg.Data)) {
					foreach(Farmer farmer in Game1.getAllFarmers()) {
						if(farmer == Game1.player) {
							Image image = Image.FromStream(ms);
							ChatBox.emojiTexture = new Texture2D(ChatBox.emojiTexture.GraphicsDevice, image.Width, image.Height);
						}
					}
				}
			}
		}

		public static void ProcessRequestTexture(this Multiplayer multiplayer, IncomingMessage msg) {
			if(msg.Data.Length != 0) {
				using(MemoryStream ms = new MemoryStream(msg.Data)) {
					if(Game1.IsClient && Game1.player.UniqueMultiplayerID == msg.FarmerID) {
						// Client recieve texture
						Image image = Image.FromStream(ms);
						Farmer f = Game1.getFarmer(msg.FarmerID);
						ChatBox.emojiTexture = new Texture2D(ChatBox.emojiTexture.GraphicsDevice, image.Width, image.Height);
					} else {
						// Server send texture						
						Farmer farmer = Game1.getFarmer(msg.FarmerID);
						Game1.server.sendMessage(farmer.UniqueMultiplayerID, new OutgoingMessage(21, farmer, ToByteArray(ChatBox.emojiTexture)));
					}
				}
			}
		}

		public static void ProcessBroadcastEmote(this Multiplayer multiplayer, IncomingMessage msg) {
			if(msg.Data.Length != 0) {
				Farmer farmer = Game1.getFarmer(msg.FarmerID);
				int emoteIndex = msg.Reader.ReadInt32();
				farmer.IsEmoting = false;
				farmer.doEmote(emoteIndex);
			}
		}

		public static Image ToImage(Texture2D texture) {
			using(MemoryStream ms = new MemoryStream()) {
				texture.SaveAsPng(ms, texture.Width, texture.Height);
				//Go To the  beginning of the stream.
				ms.Seek(0, SeekOrigin.Begin);
				//Create the image based on the stream.
				return Bitmap.FromStream(ms);
			}
		}

		public static byte[] ToByteArray(Texture2D texture) {
			return ToByteArray(ToImage(texture));
		}

		public static byte[] ToByteArray(Image image) {
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, image.RawFormat);
				return ms.ToArray();
			}
		}

		public static void broadcastTextures(this Multiplayer multiplayer, List<Texture2D> textures) {
			broadcastTextures(multiplayer, textures.ToArray());
		}

		public static void broadcastTextures(this Multiplayer multiplayer, Texture2D[] textures) {

			if(textures.Length != 0 && Game1.IsMultiplayer) {

				using(MemoryStream memoryStream = new MemoryStream()) {

					OutgoingMessage message = new OutgoingMessage(20, Game1.player, memoryStream.ToArray());
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

		}

		private static BinaryWriter CreateWriter(this Multiplayer multiplayer, Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			if(multiplayer.logging.IsLogging) {
				writer = new LoggingBinaryWriter(writer);
			}
			return writer;
		}
	}

}

//public static class Extension {

//	public static  void processIncomingMessage(this Multiplayer c, IncomingMessage msg) {
//		if(msg.MessageType == 20) {

//		} else {
//			c.processIncomingMessage(msg);
//		}
//	}

//}

