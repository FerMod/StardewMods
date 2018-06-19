﻿
using System.Reflection;
using MultiplayerEmotes.Extensions;
using Harmony;
using StardewValley;
using StardewValley.Network;
using System;
using System.IO;
using MultiplayerEmotes.Framework;

namespace MultiplayerEmotes.Patches {

	internal static class MultiplayerPatch {


		internal class ProcessIncomingMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.processIncomingMessage), new Type[] { typeof(IncomingMessage) });
			public override MethodInfo Prefix => AccessTools.Method(this.GetType(), nameof(ProcessIncomingMessagePatch.ProcessIncomingMessage_Prefix));

			//TODO: Checking for ussed MessageTypes ids. Possible?
			private static bool ProcessIncomingMessage_Prefix(Multiplayer __instance, ref IncomingMessage msg) {

				if(msg.MessageType == ModConstants.Network.MessageTypeID && msg.Data.Length >= 0) {

					try {
						ModConstants.Network.MessageAction action = (ModConstants.Network.MessageAction)Enum.ToObject(typeof(ModConstants.Network.MessageAction), msg.MessageType);
						//Check that this isnt other mods message by trying to read a 'key'
						String keyword = msg.Reader.ReadString();
						if(keyword.Equals(ModConstants.Network.MessageAction.EmoteBroadcast.ToString())) {
							__instance.ProcessBroadcastEmote(msg);
							// Dont let to execute the vanilla method
							return false;
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

}
