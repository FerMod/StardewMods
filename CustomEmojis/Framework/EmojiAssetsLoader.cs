
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CustomEmojis.Framework.Constants;
using CustomEmojis.Framework.Network;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerEmojis;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace CustomEmojis.Framework {

	public class EmojiAssetsLoader : IAssetLoader {

		public Texture2D VanillaTexture { get; set; }
		public Texture2D CustomTexture { get; set; }

		public Texture2D CurrentTexture { get; set; }
		public List<TextureData> LoadedTextureData { get; set; } = new List<TextureData>();

		public int EmojisSize { get; set; }
		public bool CustomTextureAdded { get; set; }
		public int NumberCustomEmojisAdded => LoadedTextureData.Count();
		public int TotalNumberEmojis { get; private set; }
		public bool SaveGeneratedTexture { get; set; }
		public bool SaveCustomEmojiTexture { get; set; }
		public bool ShouldGenerateTexture { get; set; }

		private readonly IModHelper modHelper;
		private readonly ModData modData;
		private readonly string[] imageExtensions;
		private readonly bool saveCreatedTexture;

		public EmojiAssetsLoader(IModHelper modHelper, ModData modData, int emojisSize, string[] imageExtensions, bool generateTexture = true, bool saveCreatedTexture = true) {

			this.modHelper = modHelper;
			this.modData = modData;

			VanillaTexture = modHelper.Content.Load<Texture2D>("LooseSprites\\emojis", ContentSource.GameContent);

			this.CustomTextureAdded = false;
			this.EmojisSize = emojisSize;
			this.imageExtensions = imageExtensions;

			this.ShouldGenerateTexture = generateTexture;
			this.saveCreatedTexture = saveCreatedTexture;

		}

		/// <summary>Get whether this instance can load the initial version of the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanLoad<T>(IAssetInfo asset) {
			return asset.AssetNameEquals(@"LooseSprites\emojis");
		}

		/// <summary>Load a matched asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public T Load<T>(IAssetInfo asset) {

			Logger.Log($"Generate Texture? {modData.ShouldGenerateTexture()}");
			Logger.Log($"Save Checksum Data? {modData.ShouldSaveData()}");

			Stopwatch swTotal = new Stopwatch();
			ModEntry.ModMonitor.Log($"[EmojiAssetsLoader TextureCreated/Loaded] Timer Started!");
			Logger.Log("[Start Asset Load] Timer Started!");
			swTotal.Start();

			string outputFolderPath = Path.Combine(modHelper.DirectoryPath, Assets.OutputFolder);
			Directory.CreateDirectory(outputFolderPath);

			string inputFolderPath = Path.Combine(modHelper.DirectoryPath, Assets.InputFolder);

			// If file changes are detected, make again the texture
			if(!Directory.Exists(inputFolderPath)) {
				Directory.CreateDirectory(inputFolderPath);
				Logger.Log("[In Assed Load] No directory", $"Total time Elapsed: {swTotal.Elapsed}");
			} else if(modData.Checksum()) {

				Logger.Log("[In Assed Load] [Generate Texture] Timer Started!", $"Total Time Elapsed: {swTotal.Elapsed}");

				Stopwatch sw = new Stopwatch();
				sw.Start();

				Logger.Log("[In Assed Load] [Merging Images]");

				CustomTexture = MergeEmojiImages(inputFolderPath);

				Logger.Log("[In Assed Load] [After Merging Images]", $"Time Elapsed: {sw.Elapsed}");

				Logger.Log("[In Assed Load] [Saving Merged Image]");
				sw.Restart();

				if(CustomTexture != null) {
					SaveTextureToPng(this.CustomTexture, Path.Combine(modHelper.DirectoryPath, Assets.OutputFolder, Assets.OutputFile));
				}

				sw.Stop();
				Logger.Log("[In Assed Load] [After Saving Merged Image]", $"Time Elapsed: {sw.Elapsed}");

			} else if(File.Exists(Path.Combine(modHelper.DirectoryPath, Assets.OutputFolder, Assets.OutputFile))) {

				Logger.Log("[In Assed Load] [Load Texture]");

				Stopwatch sw = new Stopwatch();
				sw.Start();
				CustomTexture = modHelper.Content.Load<Texture2D>(Path.Combine(Assets.OutputFolder, Assets.OutputFile), ContentSource.ModFolder);

				sw.Stop();
				Logger.Log("[In Assed Load] [After Load Texture]", $"Time Elapsed: {sw.Elapsed}");

			}

			if(CustomTexture != null) {
				this.CustomTextureAdded = true;
				this.CurrentTexture = this.CustomTexture;
			} else {
				this.CustomTextureAdded = false;
				this.CurrentTexture = this.VanillaTexture;
			}
			swTotal.Stop();
			Logger.Log($"[End Asset Load]", $"Total Time Elapsed: {swTotal.Elapsed}");
			// (T)(object) is a trick to cast anything to T if we know it's compatible
			return (T)(object)this.CurrentTexture;
		}

		public int UpdateTotalEmojis() {
			if(TotalNumberEmojis != EmojiMenu.totalEmojis) {
				EmojiMenu.totalEmojis += NumberCustomEmojisAdded - ((VanillaTexture.Width / EmojisSize) * (int)Math.Ceiling((double)NumberCustomEmojisAdded / 14));
				TotalNumberEmojis = EmojiMenu.totalEmojis;
			}
			return TotalNumberEmojis;
		}

		private void SaveTextureToPng(Texture2D texture, string path) {
			using(FileStream stream = File.Create(path)) {
				texture.SaveAsPng(stream, texture.Width, texture.Height);
			}
		}

		private Texture2D MergeEmojiImages(string inputPath) {

			Texture2D outputTexture = null;

			List<Image> imagesList = modData.FilesChecksums.Values.Select(x => (Image)ResizeImage(Image.FromFile(x), EmojisSize, EmojisSize)).ToList();

			if(imagesList.Count > 0) {

				LoadedTextureData = new List<TextureData>();
				foreach(Image image in imagesList) {
					using(MemoryStream stream = new MemoryStream()) {
						image.Save(stream, ImageFormat.Png);
						LoadedTextureData.Add(new TextureData(stream));
					}
				}

				outputTexture = MergeTextures(VanillaTexture, LoadedTextureData.Select(x => x.GetTexture()).ToList());

			}

			return outputTexture;
		}

		internal void ReloadAsset() {
			ModEntry.ModMonitor.Log($"Reloading emoji assets...");

			bool cacheInvalidated = modHelper.Content.InvalidateCache("LooseSprites/emojis");
			UpdateTotalEmojis();
			ModEntry.ModMonitor.Log($"CacheInvalidated: {cacheInvalidated}");
			//modHelper.Reflection.GetField<Texture2D>(Game1.chatBox.emojiMenu, "emojiTexture").SetValue(modHelper.Content.Load<Texture2D>($"{Assets.OutputFolder}/{Assets.OutputFile}"));
			//ChatBox.emojiTexture = modHelper.Content.Load<Texture2D>($"{Assets.OutputFolder}/{Assets.OutputFile}");
		}

		private Texture2D MergeTextures(Texture2D vanillaTexture, List<Texture2D> textureList) {

			List<Image> textureImages = new List<Image>();
			foreach(Texture2D texture in textureList) {
				textureImages.Add(TextureToImage(texture));
			}

			return MergeTextures(vanillaTexture, textureImages);
		}

		private Texture2D MergeTextures(Texture2D vanillaTexture, List<Image> images) {
			return MergeTextures(TextureToImage(vanillaTexture), images);
		}

		private Texture2D MergeTextures(Image vanillaTexture, List<Image> images) {
			images.Insert(0, vanillaTexture);
			return MergeTextures(images);
		}

		private Texture2D MergeTextures(List<Image> images) {

			Bitmap outputImage = new Bitmap(images[0].Width, images[0].Height + ((int)Math.Ceiling((double)NumberCustomEmojisAdded / 14) * EmojisSize), PixelFormat.Format32bppArgb);
			using(Graphics graphics = Graphics.FromImage(outputImage)) {
				graphics.DrawImage(images[0], new Rectangle(new Point(), images[0].Size), new Rectangle(new Point(), images[0].Size), GraphicsUnit.Pixel);
			}

			int xPosition = 0;
			int yPosition = 0;
			for(int i = 1; i < images.Count(); i++) {
				xPosition = ((i - 1) % 14) * EmojisSize;
				if((i - 1) % 14 == 0) {
					yPosition = (((i - 1) / 14) * EmojisSize) + images[0].Height;
				}
				using(Graphics graphics = Graphics.FromImage(outputImage)) {
					graphics.DrawImage(images[i], new Rectangle(new Point(xPosition, yPosition), images[i].Size), new Rectangle(new Point(), images[i].Size), GraphicsUnit.Pixel);
				}
			}

			using(MemoryStream memoryStream = new MemoryStream()) {
				outputImage.Save(memoryStream, ImageFormat.Png);
				return Texture2D.FromStream(Game1.graphics.GraphicsDevice, memoryStream);
			}
		}

		private List<Image> GetImagesList(IEnumerable<string> filePathsList) {
			List<Image> images = new List<Image>();
			foreach(string filePath in filePathsList) {
				images.Add(ResizeImage(Image.FromFile(filePath), EmojisSize, EmojisSize));
			}
			return images;
		}

		/// <summary>Resize the image to the specified width and height.</summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height) {

			var destRect = new Rectangle(0, 0, width, height);
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

		public static Image TextureToImage(Texture2D texture) {
			Image image;
			using(MemoryStream memoryStream = new MemoryStream()) {
				texture.SaveAsPng(memoryStream, texture.Width, texture.Height);
				memoryStream.Seek(0, SeekOrigin.Begin);
				image = Image.FromStream(memoryStream);
			}
			return image;
		}

		public byte[] ToByteArray(Texture2D texture) {
			return ToByteArray(TextureToImage(texture));
		}

		public byte[] ToByteArray(Image image) {
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, image.RawFormat);
				return ms.ToArray();
			}
		}
	}

}
