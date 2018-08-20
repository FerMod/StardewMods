
using System.Reflection;
using MultiplayerEmotes.Extensions;
using Harmony;
using StardewValley;
using StardewValley.Network;
using System;
using System.IO;
using MultiplayerEmotes.Framework.Constants;

namespace MultiplayerEmotes.Framework.Patches {

	internal static class MultiplayerPatch {

		/*
		 MessageTypes:

		 0	farmerDelta				(ClientBroadcast type)
		 1	serverIntroduction		
		 2	playerIntroduction		(ClientBroadcast type)
		 3	locationIntroduction
		 4	forceEvent				(ClientBroadcast type)
		 5	warpFarmer
		 6	locationDelta			(ClientBroadcast type)
		 7	locationSprites			(ClientBroadcast type)
		 8	characterWarp
		 9	availableFarmhands
		 10	chatMessage				(ClientBroadcast type)
		 11	connectionMessage
		 12	worldDelta
		 13	teamDelta				(ClientBroadcast type)
		 14	newDaySync				(ClientBroadcast type)
		 15	chatInfoMessage			(ClientBroadcast type)
		 16	userNameUpdate
		 17	farmerGainExperience
		 18	serverToClientsMessage
		 19 disconnecting			(ClientBroadcast type)
		
		 Handled by GameServer:
		 2 (playerIntroduction)		(call to Multiplayer)
		 5 (warpFarmer)
		 defult handled in: Multiplayer

		 Handled by Client:
		 1 (serverIntroduction)
		 2 (playerIntroduction)		(call to Multiplayer)
		 3 (locationIntroduction)	(call to Multiplayer)
		 9 (availableFarmhands)
		 11 (connectionMessage) 
		 16 (userNameUpdate)
		 defult handled in: Multiplayer

		*/

		internal class ProcessIncomingMessagePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.processIncomingMessage), new Type[] { typeof(IncomingMessage) });
			public override MethodInfo Prefix => AccessTools.Method(this.GetType(), nameof(ProcessIncomingMessagePatch.ProcessIncomingMessage_Prefix));

			//TODO: Checking for ussed MessageTypes ids. Possible?
			private static bool ProcessIncomingMessage_Prefix(Multiplayer __instance, ref IncomingMessage msg) {

				if(msg.MessageType == ModNetwork.MessageTypeID && msg.Data.Length > 0) {

					String keyword = ModNetwork.MessageAction.None.ToString();

					try {
						//Check that this isnt other mods message by trying to read a 'key'
						keyword = msg.Reader.ReadString();
					} catch(EndOfStreamException) {
						// Do nothing. If it does not contain the key, it may be anothers mod custom message or something went wrong
					}

					if(Enum.TryParse(keyword, out ModNetwork.MessageAction action)) {
						if(Enum.IsDefined(typeof(ModNetwork.MessageAction), action)) {
							switch(action) {
								case ModNetwork.MessageAction.EmoteBroadcast:
									__instance.ReceiveEmoteBroadcast(msg);
									// Dont let to execute the vanilla method
									return false;
								case ModNetwork.MessageAction.CharacterEmoteBroadcast:
									__instance.ReceiveCharacterEmoteBroadcast(msg);
									// Dont let to execute the vanilla method
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
