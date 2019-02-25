namespace MultiplayerEmotes.Framework.Constants {

	internal static class ModNetwork {

		public static byte MessageTypeID = 50;

		public enum MessageAction {
			None,
			EmoteBroadcast,
			CharacterEmoteBroadcast
		};
	}

}
