
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.IO;
using System;

namespace CustomEmojis.Framework.Network {

	[Serializable]
	public class TextureData {

		public int Width { get; set; }
		public int Height { get; set; }
		public Color[] Data { get; set; }

		public TextureData() {
		}

		public TextureData(Texture2D texture) {
			InitData(texture);
		}

		public TextureData(Stream stream) {
			InitData(stream);
		}

		public void InitData(Stream stream) {
			InitData(Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream));
		}

		public void InitData(Texture2D texture) {

			Width = texture.Width;
			Height = texture.Height;

			Data = new Color[Width * Height];
			texture.GetData(Data);

		}

		/*
		 public void InitData(BinaryReader reader) {

			Width = reader.ReadInt32();
			Height = reader.ReadInt32();

			Data = new Color[Width * Height];

			for(int i = 0; i < Data.Length; i++) {
				int r = reader.ReadByte();
				int g = reader.ReadByte();
				int b = reader.ReadByte();
				int a = reader.ReadByte();
				Data[i] = new Color(r, g, b, a);
			}

			Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, Width, Height);
			texture.SetData(Data);

		}
		*/

		public Texture2D GetTexture() {
			Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, Width, Height);
			texture.SetData(Data);
			return texture;
		}

	}

}
