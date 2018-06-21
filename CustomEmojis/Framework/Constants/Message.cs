
namespace CustomEmojis.Framework.Constants {

	internal static class Message {

		internal static byte TypeID = 50;

		internal enum Action {
			None,
			EmojiTextureRequest,
			EmojiTextureResponse,
			EmojiTextureBroadcast
		};
	}

}
