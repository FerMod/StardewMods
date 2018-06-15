
using System.Reflection;
using MultiplayerEmotes.Extensions;
using Harmony;
using StardewValley;
using StardewValley.Network;
using System;
using System.IO;
using MultiplayerEmotes.Framework;

namespace MultiplayerEmotes.Patches {

	public class MultiplayerPatch : ClassPatch {

		public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), "processIncomingMessage", new Type[] { typeof(IncomingMessage) });
		public override MethodInfo Prefix => typeof(MultiplayerPatch).GetMethod("ProcessIncomingMessage_Prefix");

		//TODO: Checking for ussed MessageTypes ids. Possible?
		public static bool ProcessIncomingMessage_Prefix(Multiplayer __instance, ref IncomingMessage msg) {

			if(msg.MessageType == Constants.Network.MessageTypeID && msg.Data.Length >= 0) {

				try {

					using(BinaryReader reader = msg.Reader) {
						Constants.Network.MessageAction action = (Constants.Network.MessageAction)Enum.ToObject(typeof(Constants.Network.MessageAction), msg.MessageType);
						//Check that this isnt other mods message by trying to read a 'key'
						String keyword = reader.ReadString();
						if(keyword.Equals(Constants.Network.MessageAction.EmoteBroadcast.ToString())) {
							__instance.ProcessBroadcastEmote(msg);
							// Dont let to execute the vanilla method
							return false;
						}
					}

				} catch(EndOfStreamException) {
					// Do nothing. If it does not contain the key, it may be another mods custom message or something went wrong
				}

			}

			// Allow to execute the vanilla method
			return true;
		}

	}

}
