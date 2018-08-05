
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace MultiplayerEmotes.Framework.Constants {

	internal static class Sprites {

		public static class Emotes {

			public static string AssetName = "TileSheets/emotes";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.GameContent);

		}

		public static class MenuButton {

			public static string AssetName = "LooseSprites/Cursors";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.GameContent);

			public static Rectangle SourceRectangle = new Rectangle(301, 288, 15, 15);
		}

		public static class MenuBox {

			public static string PrototypeAssetName = "assets/emoteBoxPrototype.png";

			public static Texture2D PrototypeTexture => ModEntry.ModHelper.Content.Load<Texture2D>(PrototypeAssetName, ContentSource.ModFolder);

			public static string AssetName = "assets/emoteBox.png";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.ModFolder);

			public static int Width = 300;

			public static int Height = 250;

			public static readonly Rectangle EmotesBox = new Rectangle(0, 0, 228, 300);

			public static readonly Rectangle TopArrow = new Rectangle(228, 0, 28, 28 - 8);

			public static readonly Rectangle DownArrow = new Rectangle(228, 28 + 8, 28, 28 - 8);

			public static readonly Rectangle LeftArrow = new Rectangle(228, 56, 28 - 8, 28);

			public static readonly Rectangle RightArrow = new Rectangle(228 + 8, 84, 28 - 8, 28);

		}

		public static class MenuArrow {

			public static string AssetName = "LooseSprites/chatBox";

			public static Texture2D Texture => ModEntry.ModHelper.Content.Load<Texture2D>(AssetName, ContentSource.GameContent);

			public static Rectangle Up = new Rectangle(256, 20, 32, 20);

			public static Rectangle Down = new Rectangle(256, 200, 32, 20);

		}

	}

}
