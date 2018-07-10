
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using CustomEmojis.Framework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace CustomEmojis.Framework.Menu {

	public class CachedMessageEmojis : IClickableMenu {

		private readonly IReflectionHelper Reflection;
		//private ObjectCache CachedEmoji;

		public List<ChatMessage> CachedChatMessages;

		public CachedMessageEmojis(IReflectionHelper reflection) {

			Reflection = reflection;
			//CachedEmoji = MemoryCache.Default;
			//CachedChatMessages = MemoryCache.

			MultiplayerExtension.OnPlayerDisconnected += MultiplayerExtension_OnPlayerDisconnected;

		}

		private void MultiplayerExtension_OnPlayerDisconnected(object sender, Events.PlayerDisconnectedEventArgs e) {

			ObjectCache cache = MemoryCache.Default;

			if(!(cache[$"{e.Player.UniqueMultiplayerID}_messages"] is List<ChatMessage> chatMessages)) {
				chatMessages = Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
				cache[$"{e.Player.UniqueMultiplayerID}_messages"] = chatMessages;
			}

		}

		//FIXME: Improve method
		private Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle) {
			Color[] color = new Color[rectangle.Width * rectangle.Height];
			for(int x = 0; x < rectangle.Width; x++) {
				for(int y = 0; y < rectangle.Height; y++) {
					color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
				}
			}
			return color;
		}
		public bool withinBounds;
		public override void draw(SpriteBatch b) {
			if(Game1.chatBox != null && Game1.chatBox.isActive()) {
				//ModEntry.ModLogger.Log("draw");

				List<ChatMessage> chatMessageMessages = Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();

				ObjectCache cache = MemoryCache.Default;
				long uniqueMultiplayerID = Game1.player.UniqueMultiplayerID;

				if(!(cache[$"{uniqueMultiplayerID}_messages"] is List<ChatMessage> chatMessages)) {
					chatMessages = Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
					cache[$"{uniqueMultiplayerID}_messages"] = chatMessages;
				}



				int verticalSizeAcum = 0;
				for(int i = chatMessageMessages.Count - 1; i >= 0; --i) {

					ChatMessage message = chatMessageMessages[i];
					verticalSizeAcum += message.verticalSize;

					int x = 12 + 400; //TODO: Remove +400px
					int y = Game1.chatBox.yPositionOnScreen - verticalSizeAcum - 8 + (Game1.chatBox.chatBox.Selected ? 0 : Game1.chatBox.chatBox.Height);

					List<ChatSnippet> chatSnippetMessage = Reflection.GetField<List<ChatSnippet>>(message, "message").GetValue();

					float num1 = 0.0f;
					float num2 = 0.0f;
					for(int index = 0; index < chatSnippetMessage.Count; ++index) {

						int emojiIndex = chatSnippetMessage[index].emojiIndex;

						if(emojiIndex != -1) {

							Vector2 position = new Vector2(x + num1 + 1.0f, y + num2 - 4.0f);
							withinBounds = Game1.chatBox.isWithinBounds((int)position.X, (int)position.Y);

							if(emojiIndex > 196 && withinBounds) {
								ModEntry.ModMonitor.Log($"Is within bounds? {withinBounds}");
							}

							if(emojiIndex > 196 && Game1.chatBox.isWithinBounds((int)position.X, (int)position.Y)) { //TODO: get max emojis programatically

								if(!(cache[$"emoji_{emojiIndex}"] is Texture2D cachedEmojiTexture)) {

									Texture2D emojiTextureSheet = ChatBox.emojiTexture;

									Color[] imageData = new Color[emojiTextureSheet.Width * emojiTextureSheet.Height];
									emojiTextureSheet.GetData(imageData);

									Rectangle sourceRectangle = new Rectangle(emojiIndex * 9 % ChatBox.emojiTexture.Width, emojiIndex * 9 / ChatBox.emojiTexture.Width * 9, 9, 9);
									Color[] imagePiece = GetImageData(imageData, emojiTextureSheet.Width, sourceRectangle);

									cachedEmojiTexture = new Texture2D(Game1.graphics.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
									cachedEmojiTexture.SetData(imagePiece);

									cache[$"emoji_{emojiIndex}"] = cachedEmojiTexture;

									ModEntry.ModLogger.Log("Cached values:");
									foreach(KeyValuePair<string, object> entry in cache.ToList()) {
										ModEntry.ModLogger.Log($"Key: {entry.Key}, Value: {entry.Value}");
									}

								}

								b.Draw(cachedEmojiTexture, position, new Rectangle?(new Rectangle(0, 0, 9, 9)), Color.White * message.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);

							} else {
								b.Draw(ChatBox.emojiTexture, position, new Rectangle?(new Rectangle(emojiIndex * 9 % ChatBox.emojiTexture.Width, emojiIndex * 9 / ChatBox.emojiTexture.Width * 9, 9, 9)), Color.White * message.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
							}

						} else if(chatSnippetMessage[index].message != null && chatSnippetMessage[index].message.Equals(Environment.NewLine)) {
							num1 = 0.0f;
							num2 += ChatBox.messageFont(message.language).MeasureString("(").Y;
						}
						num1 += chatSnippetMessage[index].myLength;
						if(num1 >= 888.0) {
							num1 = 0.0f;
							num2 += ChatBox.messageFont(message.language).MeasureString("(").Y;
							if(chatSnippetMessage.Count > index + 1 && chatSnippetMessage[index + 1].message != null && chatSnippetMessage[index + 1].message.Equals(Environment.NewLine)) {
								++index;
							}
						}
					}

				}

			}

		}
	}
}
