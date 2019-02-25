
using MultiplayerEmotes.Framework.Constants;
using System.Linq;
using StardewValley;
using StardewValley.Network;
using StardewModdingAPI;
using System.IO;
using System.Collections.Generic;
using StardewValley.Buildings;

namespace MultiplayerEmotes.Extensions {

	public static class MultiplayerExtension {

		public static void BroadcastEmote(this Multiplayer multiplayer, int emoteIndex, Character character = null) {

			if(Game1.IsMultiplayer) {

				ModNetwork.MessageAction messageAction;
				string characterId = "";

				if(character != null && !(character is Farmer)) {

					messageAction = ModNetwork.MessageAction.CharacterEmoteBroadcast;

					if(character is NPC npc) {
						characterId = npc.Name;
					} else if(character is FarmAnimal farmAnimal) {
						characterId = farmAnimal.myID.Value.ToString();
					}

				} else {

					messageAction = ModNetwork.MessageAction.EmoteBroadcast;

				}

#if DEBUG
				TestFunc(characterId);
#endif

				using(MemoryStream memoryStream = new MemoryStream()) {
					using(BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {

						binaryWriter.Write(messageAction.ToString());
						binaryWriter.Write(emoteIndex);
						binaryWriter.Write(characterId);

						OutgoingMessage message = new OutgoingMessage(ModNetwork.MessageTypeID, Game1.player, memoryStream.ToArray());


						if(Game1.IsClient) {
							Game1.client.sendMessage(message);
						} else {
							foreach(long uniqueMultiplayerID in Game1.otherFarmers.Keys) {
								Game1.server.sendMessage(uniqueMultiplayerID, message);
							}
						}

					}
				}

			}

		}

#if DEBUG
		public static void TestFunc(string name, bool mustBeVillager = false) {

			if(Game1.currentLocation != null) {
				ModEntry.ModMonitor.Log($"- Loop1");
				foreach(NPC character in (IEnumerable<NPC>)Game1.currentLocation.getCharacters()) {
					ModEntry.ModMonitor.Log($"Character: {character.Name}. IsVillager: {character.isVillager()}");
					if(character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
						ModEntry.ModMonitor.Log($"### Found Character: {character}");
				}
			}
			ModEntry.ModMonitor.Log($"- Loop2");
			for(int index = 0; index < Game1.locations.Count; ++index) {
				foreach(NPC character in (IEnumerable<NPC>)Game1.locations[index].getCharacters()) {
					ModEntry.ModMonitor.Log($"Character: {character.Name}. IsVillager: {character.isVillager()}");
					if(character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
						ModEntry.ModMonitor.Log($"### Found Character: {character.Name}");
				}
			}
			if(Game1.getFarm() != null) {
				ModEntry.ModMonitor.Log($"- Loop3");
				foreach(Building building in Game1.getFarm().buildings) {
					if(building.indoors.Value != null) {
						foreach(NPC character in building.indoors.Value.characters) {
							ModEntry.ModMonitor.Log($"Character: {character.Name}. IsVillager: {character.isVillager()}");
							if(character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
								ModEntry.ModMonitor.Log($"### Found Character: {character.Name}");
						}
					}
				}
			}
		}
#endif

		public static void ReceiveEmoteBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(msg.Data.Length >= 0) {
				int emoteIndex = msg.Reader.ReadInt32();
				msg.SourceFarmer.IsEmoting = false;
				msg.SourceFarmer.doEmote(emoteIndex);

#if DEBUG
				ModEntry.ModMonitor.Log($"Received player emote broadcast. (Name: \"{msg.SourceFarmer.Name}\", Emote: {emoteIndex})");
#endif

			}
		}

		public static void ReceiveCharacterEmoteBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
			if(Context.IsPlayerFree && msg.Data.Length >= 0) {

				int emoteIndex = msg.Reader.ReadInt32();
				string characterId = msg.Reader.ReadString();

				Character sourceCharacter = null;

				if(long.TryParse(characterId, out long id)) {
					sourceCharacter = (Game1.currentLocation as AnimalHouse).animals.Values.FirstOrDefault(x => x.myID.Value == id);
				} else {
					sourceCharacter = Game1.getCharacterFromName(characterId);
				}

#if DEBUG
				TestFunc(characterId);
				ModEntry.ModMonitor.Log($"Received character emote broadcast. (Name: \"{sourceCharacter.Name}\", Emote: {emoteIndex})");
#endif

				if(sourceCharacter != null && !sourceCharacter.IsEmoting) {
					sourceCharacter.doEmote(emoteIndex, true);
				}

			}
		}

	}

}
