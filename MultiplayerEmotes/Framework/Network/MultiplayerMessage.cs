using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MultiplayerEmotes.Framework.Network {

	public class MultiplayerModMessage {

		private readonly IModHelper helper;

		public MultiplayerModMessage(IModHelper helper) {
			this.helper = helper;
			SubscribeEvents();
		}

		public void SubscribeEvents() {
			this.helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
		}

		public void UnsubscribeEvents() {
			this.helper.Events.Multiplayer.ModMessageReceived -= this.OnModMessageReceived;
		}

		public void Send(EmoteMessage message) {

			Type messageType = typeof(EmoteMessage);

			ModEntry.ModMonitor.Log($"Sending message. (FromPlayer: \"{Game1.player.UniqueMultiplayerID}\", FromMod: {helper.ModRegistry.ModID}, Type: {messageType})", LogLevel.Info);
			helper.Multiplayer.SendMessage(message, messageType.ToString(), modIDs: new[] { helper.ModRegistry.ModID });

		}

		public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {

			ModEntry.ModMonitor.Log($"Received message. (FromPlayer: \"{e.FromPlayerID}\", FromMod: {e.FromModID}, Type: {e.Type})", LogLevel.Info);

			Type messageType = typeof(EmoteMessage);
			if(e.FromModID == helper.ModRegistry.ModID && e.Type == messageType.ToString()) {

				EmoteMessage message = e.ReadAs<EmoteMessage>();

				Character sourceCharacter = null;
				switch(message.EmoteSourceType) {
					case CharacterType.Farmer:
						if(long.TryParse(message.EmoteSourceId, out long farmerId)) {
							sourceCharacter = Game1.getFarmer(farmerId);
						}
						break;
					case CharacterType.NPC:
						sourceCharacter = Game1.getCharacterFromName(message.EmoteSourceId);
						break;
					case CharacterType.FarmAnimal:
						if(long.TryParse(message.EmoteSourceId, out long farmAnimalId)) {
							sourceCharacter = (Game1.currentLocation as AnimalHouse).animals.Values.FirstOrDefault(x => x.myID.Value == farmAnimalId);
						}
						break;
					case CharacterType.Unknown:
					default:
						break;
				}

				if(sourceCharacter != null) {
					//sourceCharacter.IsEmoting = false;
					sourceCharacter.doEmote(message.EmoteIndex, true);
				}

			}

		}

	}

}
