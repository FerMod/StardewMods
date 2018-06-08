
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System.Collections.Generic;
using System.Threading;

namespace MultiplayerEmotes {

	public class SyncEmojiTextures : NetSynchronizer {

		private IModHelper modHelper;
		private NetArray<byte, NetByte> emojiTexture;

		public SyncEmojiTextures(IModHelper helper, Texture2D texture) {
			this.modHelper = helper;
			emojiTexture = new NetArray<byte, NetByte>(6);
			//this.sendVar<NetArray<byte, NetByte>, byte[]>("hostEmojiTexture", emojiTexture);
		}

		public void testValTrue() {
			Multiplayer multiplayer = modHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.UpdateEarly();
			if (Game1.IsServer) {
				this.sendVar<NetBool, bool>("testVal", true);
			}
		}
		public void testVal(bool b) {
			if (Game1.IsServer) {
				this.sendVar<NetBool, bool>("testVal", b);
			}
			Multiplayer multiplayer = modHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.UpdateLate(false);
		}

		public override void processMessages() {
			Multiplayer multiplayer = modHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			multiplayer.UpdateLate(false);
			Thread.Sleep(16);
			Program.sdk.Update();
			multiplayer.UpdateEarly();
		}

		protected override void sendMessage(params object[] data) {
			Multiplayer multiplayer = modHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			OutgoingMessage message = new OutgoingMessage((byte)20, Game1.player, data);
			if (Game1.IsServer) {
				foreach (Farmer farmer in (IEnumerable<Farmer>)Game1.otherFarmers.Values)
					Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
			} else {
				if (!Game1.IsClient)
					return;
				Game1.client.sendMessage(message);
			}
		}

	}

}
