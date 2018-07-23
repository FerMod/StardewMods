
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using System.IO;

namespace MapPings.Framework.Constants {

	internal static class Sprites {

		public static class Map {

			public static string AssetName = "LooseSprites\\map";

			public static Texture2D Texture => Game1.content.Load<Texture2D>(AssetName);

            public static Rectangle SourceRectangle = new Rectangle(0, 0, 300, 180);

		}

		public static class PingArrow {

			public static string AssetName = "LooseSprites\\Cursors";

			public static Texture2D Texture => Game1.content.Load<Texture2D>(AssetName);

			public static Rectangle SourceRectangle = new Rectangle(232, 346, 9, 9);

		}

	}

}
