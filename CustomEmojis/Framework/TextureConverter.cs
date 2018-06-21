using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEmojis.Framework {

	public static class TextureConverter {

		/// <summary>
		/// Writes the Texture2D contents to a byte array. The returned byte array contains:
		/// <para>width (<code>int</code>), height (<code>int</code>), data (<code>Color[]</code>)</para>
		/// </summary>
		/// <param name="texture"></param>
		/// <returns>A new byte array.</returns>
		public static byte[] TextureToByteArray(Texture2D texture) {

			int width = texture.Width;
			int height = texture.Height;

			Color[] textureData = new Color[width * height];
			texture.GetData(textureData);

			using(MemoryStream stream = new MemoryStream()) {
				using(BinaryWriter writer = new BinaryWriter(stream)) {

					writer.Write(width);
					writer.Write(height);

					for(int i = 0; i < textureData.Length; i++) {
						writer.Write(textureData[i].R);
						writer.Write(textureData[i].G);
						writer.Write(textureData[i].B);
						writer.Write(textureData[i].A);
					}

					return stream.ToArray();
				}
			}

		}

		public static Texture2D ByteArrayToTexture2D(byte[] data) {
			using(MemoryStream stream = new MemoryStream(data)) {
				using(BinaryReader reader = new BinaryReader(stream)) {
					return ByteArrayToTexture2D(reader);
				}
			}

		}

		public static Texture2D ByteArrayToTexture2D(BinaryReader reader) {

			int width = reader.ReadInt32();
			int height = reader.ReadInt32();

			Color[] textureData = new Color[width * height];

			for(int i = 0; i < textureData.Length; i++) {
				int r = reader.ReadByte();
				int g = reader.ReadByte();
				int b = reader.ReadByte();
				int a = reader.ReadByte();
				textureData[i] = new Color(r, g, b, a);
			}

			Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
			texture.SetData(textureData);
			return texture;

		}

	}

}
