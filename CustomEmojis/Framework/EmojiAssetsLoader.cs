
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CustomEmojis.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;

namespace CustomEmojis.Framework {

	public class EmojiAssetsLoader : IAssetLoader {

		public Texture2D VanillaEmojisTexture { get; set; }
		public Texture2D CustomEmojisTexture { get; set; }

		public int EmojisSize { get; set; }
		public bool CustomEmojisAdded { get; set; } = false;
		public int NumberCustomEmojisAdded { get; set; } = 0;
		public string OutputFolder { get; set; } = "mergedSprite";
		public string OutputFile { get; set; } = "emojis.png";
		public string InputFolder { get; set; } = "sprites";

		private readonly IModHelper modHelper;
		private readonly string[] imageExtensions;
		private readonly bool createTexture = true;

		public EmojiAssetsLoader(IModHelper modHelper) {
			this.modHelper = modHelper;
		}

		public EmojiAssetsLoader(IModHelper modHelper, int emojisSize, string[] imageExtensions) : this(modHelper) {
			this.EmojisSize = emojisSize;
			this.imageExtensions = imageExtensions;
		}

		public EmojiAssetsLoader(IModHelper modHelper, int emojisSize, string[] imageExtensions, bool createTexture) : this(modHelper, emojisSize, imageExtensions) {
			this.createTexture = createTexture;
		}

		/// <summary>Get whether this instance can load the initial version of the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanLoad<T>(IAssetInfo asset) {
			return asset.AssetNameEquals(@"LooseSprites\emojis");
		}

		/// <summary>Load a matched asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public T Load<T>(IAssetInfo asset) {

			// If file changes are detected, make again the texture
			if(createTexture) {

				if(!File.Exists(Path.Combine(modHelper.DirectoryPath, "vanillaEmojis.png"))) {
					SaveTextureToPng(this.VanillaEmojisTexture, Path.Combine(modHelper.DirectoryPath, "vanillaEmojis.png"));
				}

				CustomEmojisTexture = MergeImages(InputFolder, OutputFolder);

			} else if(File.Exists(Path.Combine(OutputFolder, OutputFile))) {

				CustomEmojisTexture = modHelper.Content.Load<Texture2D>(Path.Combine(OutputFolder, OutputFile), ContentSource.ModFolder);

			}

			if(CustomEmojisTexture != null) {
				CustomEmojisAdded = true;
				return (T)(object)this.CustomEmojisTexture;
			} else {
				CustomEmojisAdded = false;
			}

			// (T)(object) is a trick to cast anything to T if we know it's compatible
			return (T)(object)this.VanillaEmojisTexture;
		}

		private void SaveTextureToPng(Texture2D texture, string path) {
			using(FileStream stream = File.Create(path)) {
				texture.SaveAsPng(stream, texture.Width, texture.Height);
			}
		}

		private Texture2D MergeImages(string inputFolder, string outputFolder) {

			Texture2D outputTexture = null;

			string inputPath = Path.Combine(modHelper.DirectoryPath, inputFolder);
			string outputPath = Path.Combine(modHelper.DirectoryPath, outputFolder);

			if(!Directory.Exists(outputPath)) {
				Directory.CreateDirectory(outputPath);
			}

			if(!Directory.Exists(inputPath)) {
				Directory.CreateDirectory(inputPath);
			} else {

				List<Image> images = new List<Image> {
					Image.FromFile(Path.Combine(modHelper.DirectoryPath, "vanillaEmojis.png"))
				};

				var files = ModUtilities.GetFiles(inputPath, imageExtensions, SearchOption.AllDirectories);

				NumberCustomEmojisAdded = files.Count();

				if(NumberCustomEmojisAdded > 0) {

					foreach(string filePath in files) {
						Image imageToAdd = Image.FromFile(filePath);
						images.Add(ResizeImage(imageToAdd, EmojisSize, EmojisSize));
					}

					Bitmap outputImage = new Bitmap(images[0].Width, images[0].Height + ((int)Math.Ceiling((double)NumberCustomEmojisAdded / 14) * EmojisSize), PixelFormat.Format32bppArgb);
					using(Graphics graphics = (Graphics.FromImage(outputImage))) {
						graphics.DrawImage(images[0], new System.Drawing.Rectangle(new System.Drawing.Point(), images[0].Size), new System.Drawing.Rectangle(new System.Drawing.Point(), images[0].Size), GraphicsUnit.Pixel);
					}
					int heightAcum = 0;
					for(int i = 1; i < images.Count(); i++) {
						if((i - 1) % 14 == 0) {
							heightAcum = (((i - 1) / 14) * EmojisSize) + images[0].Height;
						}
						using(Graphics graphics = (Graphics.FromImage(outputImage))) {
							graphics.DrawImage(images[i], new System.Drawing.Rectangle(new System.Drawing.Point(((i - 1) % 14) * EmojisSize, heightAcum), images[i].Size), new System.Drawing.Rectangle(new System.Drawing.Point(), images[i].Size), GraphicsUnit.Pixel);
						}
					}

					outputImage.Save(Path.Combine(outputPath, OutputFile), ImageFormat.Png);

					outputTexture = modHelper.Content.Load<Texture2D>(Path.Combine(outputFolder, OutputFile), ContentSource.ModFolder);

				}

			}

			return outputTexture;
		}

		/*
		//TODO: Remove
		public static Texture2D ResizeTexture(Texture2D texture, int width, int height) {
			Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
			texture.GetData(data);
			Texture2D resizedTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
			resizedTexture.SetData(data);
			return resizedTexture;
		}
		*/

		/// <summary>Resize the image to the specified width and height.</summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height) {

			var destRect = new System.Drawing.Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			//if (image.HorizontalResolution != width || image.VerticalResolution != height) {

			using(var graphics = Graphics.FromImage(destImage)) {
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using(var wrapMode = new ImageAttributes()) {
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}

			}
			//}

			return destImage;
		}

		public Image ToImage(Texture2D texture) {
			using(MemoryStream ms = new MemoryStream()) {
				texture.SaveAsPng(ms, texture.Width, texture.Height);
				//Go To the  beginning of the stream.
				ms.Seek(0, SeekOrigin.Begin);
				//Create the image based on the stream.
				return Bitmap.FromStream(ms);
			}
		}

		public byte[] ToByteArray(Texture2D texture) {
			return ToByteArray(ToImage(texture));
		}

		public byte[] ToByteArray(Image image) {
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, image.RawFormat);
				return ms.ToArray();
			}
		}

	}

}
