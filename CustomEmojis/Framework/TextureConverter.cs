
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SystemDrawing = System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

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

		public static Texture2D BitmapToTexture(GraphicsDevice device, SystemDrawing.Bitmap bitmap) {

			Texture2D texture = new Texture2D(device, bitmap.Width, bitmap.Height, true, SurfaceFormat.Color);

			BitmapData data = bitmap.LockBits(new SystemDrawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			int bufferSize = data.Height * data.Stride;

			//create data buffer 
			byte[] bytes = new byte[bufferSize];

			// copy bitmap data into buffer
			Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

			// copy our buffer to the texture
			texture.SetData(bytes);

			// unlock the bitmap data
			bitmap.UnlockBits(data);

			return texture;
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
			return ByteArrayToTexture2D(reader, width, height);
		}

		public static Texture2D ByteArrayToTexture2D(BinaryReader reader, int width, int height) {

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
