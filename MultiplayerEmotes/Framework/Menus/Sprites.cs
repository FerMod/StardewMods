using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Menus {

	/// <summary>Simplifies access to the game's sprite sheets.</summary>
	/// <remarks>Each sprite is represented by a rectangle, which specifies the coordinates and dimensions of the image in the sprite sheet.</remarks>
	internal static class Sprites {

		/*********
        ** Accessors
        *********/

		/// <summary>Sprites used to draw a letter.</summary>
		public static class Menu {

			/// <summary>The sprite sheet containing the letter sprites.</summary>
			public static Texture2D Texture => Game1.content.Load<Texture2D>("LooseSprites\\chatBox");

			/// <summary>The menu background.</summary>
			public static readonly Rectangle Panel = new Rectangle(0, 0, 244, 300);

			/// <summary>The menu background border width and heigth</summary>
			public static readonly int BorderSize = 12;

		}

		/// <summary>Sprite containing the emotes.</summary>
		public static class Emotes {

			/// <summary>The sprite texture containing the emotes sprites.</summary>
			public static Texture2D Texture => Game1.content.Load<Texture2D>("TileSheets\\emotes");

		}

		/// <summary>Sprites used to draw icons.</summary>
		public static class Icons {

			/// <summary>The sprite sheet containing the icon sprites.</summary>
			public static Texture2D Texture => Game1.mouseCursors;

			/// <summary>Button for opening the emote menu.</summary>
			public static readonly Rectangle EmoteButton = new Rectangle(301, 288, 15, 15);

			/// <summary>An up arrow for scrolling content.</summary>
			public static readonly Rectangle UpArrow = new Rectangle(256, 20, 32, 20);

			/// <summary>A down arrow for scrolling content.</summary>
			public static readonly Rectangle DownArrow = new Rectangle(256, 200, 32, 20);
			
		}

	}

}
