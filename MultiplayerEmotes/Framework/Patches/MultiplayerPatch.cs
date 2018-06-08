﻿
using System.Reflection;
using MultiplayerEmotes.Extensions;
using Harmony;
using StardewValley;
using StardewValley.Network;

namespace MultiplayerEmotes.Patches {

	public class MultiplayerPatch {

		private static MethodInfo Original => AccessTools.Method(typeof(Multiplayer), "processIncomingMessage");

		private static MethodInfo Prefix => typeof(MultiplayerPatch).GetMethod("ProcessIncomingMessage_Prefix");

		internal static void Register(HarmonyInstance harmony) {
			harmony.Patch(Original, new HarmonyMethod(Prefix), null);
		}

		internal static void Remove(HarmonyInstance harmony) {
			harmony.RemovePatch(Original, HarmonyPatchType.Prefix, harmony.Id);
		}

		//TODO: Checking for ussed MessageTypes ids. Possible?
		public static bool ProcessIncomingMessage_Prefix(Multiplayer __instance, ref IncomingMessage msg) {
			if(msg.MessageType >= 20 && msg.Data.Length >= 0) {

				try {

					//Check that this isnt other mods message
					string key = msg.Reader.ReadString();
					if(key.Equals("EmoteBroadcast")) {
						switch(msg.MessageType) {
							//case 20:
							//	__instance.ProcessBroadcastTexture(msg);
							//	return false;
							//case 21:
							//	__instance.ProcessRequestTexture(msg);
							//	return false;
							case 22:
								__instance.ProcessBroadcastEmote(msg);
								return false;
						}
					}

				} catch(System.IO.EndOfStreamException) {
					// Do nothing if does not contain the key, it may be another mods custom message
				}

			}

			return true;

		}

	}

}
