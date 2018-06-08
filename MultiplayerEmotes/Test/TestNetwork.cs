
using StardewModdingAPI;
using StardewValley;
using MultiplayerEmotes.Patches;
using StardewModdingAPI.Events;
using System;
using MultiplayerEmotes.Extensions;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MultiplayerEmotes.Menus;
using MultiplayerEmotes.Events;
using Microsoft.Xna.Framework.Input;
using Netcode;

namespace MultiplayerEmotes.Test {

	public static class TestNetwork {

		public static ITestNetwork extension;

		////public static event EventHandler EmoteEvent;
		//public static event EventHandler FarmerEmotedEvent;

		static TestNetwork() {
			extension = new DynamicExtension<ITestNetwork>().Extend();
		}

		public static void doEmote(this Farmer farmer, int whichEmote) {
			//extension.FarmerEmotedEvent.Raise(farmer, new FarmerEmotedEventArgs(farmer.IsEmoting, whichEmote));
			farmer.doEmote(whichEmote);
		}

		public static void doEmote(this Character character, int whichEmote, bool playSound, bool nextEventCommand = true) {
			//extension.FarmerEmotedEvent.Raise(character, new FarmerEmotedEventArgs(character.IsEmoting, whichEmote));
			character.doEmote(whichEmote, playSound, nextEventCommand);
		}

		public static void doEmote(this Character character, int whichEmote, bool nextEventCommand = true) {
			//extension.FarmerEmotedEvent.Raise(character, new FarmerEmotedEventArgs(character.IsEmoting, whichEmote));
			character.doEmote(whichEmote, true, nextEventCommand);
		}

		//public static NetInt netTestNumber = new NetInt(0);

		//public static int TestNumber {
		//	get {
		//		//return extension.NumberVal.Value;
		//		return (int)((NetFieldBase<int, NetInt>)netTestNumber);
		//	}
		//	set {
		//		//extension.NumberVal.Value = value;
		//		netTestNumber.Value = value;
		//	}
		//}

		////public static ITestNetwork extension;

		////static TestNetwork() {
		////	extension = new DynamicExtension<ITestNetwork>().Extend();
		////}

		////public static ITestNetwork GetExtension(this Farmer farmer) {
		////	return extension;
		////}

		//public static void InitializeNetFields(this Farmer farmer) {
		//	//farmer.NetFields.AddFields((INetSerializable)extension.NumberVal);
		//	farmer.NetFields.AddFields((INetSerializable)netTestNumber);
		//}

		//public static void MarkValuesDirty(this Farmer farmer) {
		//	farmer.NetFields.MarkDirty();
		//}

		//public static int GetTestNumber(this Farmer farmer) {
		//	return TestNumber;
		//}

		//public static void SetTestNumber(this Farmer farmer, int num) {
		//	TestNumber = num;
		//}

	}

}
