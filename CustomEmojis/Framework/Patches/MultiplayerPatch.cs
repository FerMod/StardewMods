
using System.Reflection;
using Harmony;
using StardewValley;
using StardewValley.Network;
using System;
using System.IO;
using CustomEmojis.Framework.Patches;
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Constants;

namespace CustomEmojis.Patches {

	internal static class MultiplayerPatch {

		internal class ProcessIncomingMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.processIncomingMessage), new Type[] { typeof(IncomingMessage) });
			public override MethodInfo Prefix => AccessTools.Method(this.GetType(), nameof(ProcessIncomingMessagePatch.ProcessIncomingMessage_Prefix));

			//TODO: Checking for ussed MessageTypes ids. Possible?
			private static bool ProcessIncomingMessage_Prefix(Multiplayer __instance, ref IncomingMessage msg) {

				// Vanilla message types
				switch(msg.MessageType) {
					/*
					// Not being used in the class 'Multiplayer'
					case 2:
						__instance.PlayerConnected(msg);
						break;					
					 */
					case 19:
						__instance.PlayerDisconnected(msg);
						break;
				}

				if(msg.MessageType == Message.TypeID && msg.Data.Length > 0) {

					String keyword = Message.Action.None.ToString();

					try {
						//Check that this isnt other mods message by trying to read a 'key'
						keyword = msg.Reader.ReadString();
					} catch(EndOfStreamException) {
						// Do nothing. If it does not contain the key, it may be anothers mod custom message or something went wrong
					}

					if(Enum.TryParse(keyword, out Message.Action action)) {
						if(Enum.IsDefined(typeof(Message.Action), action)) {
							switch(action) {
								case Message.Action.RequestEmojiTexture:
									__instance.ReceiveEmojiTextureRequest(msg);
									return false;
								case Message.Action.SendEmojiTexture:
									__instance.ReceiveEmojiTexture(msg);
									return false;
								case Message.Action.BroadcastEmojiTexture:
									__instance.ReceiveEmojiTextureBroadcast(msg);
									return false;
								case Message.Action.SendEmojisTextureDataList:
									__instance.ReceiveEmojisTextureDataList(msg);
									return false;
							}
						}
					}

				}

				// Allow to execute the vanilla method
				return true;
			}

		}

	}

}
