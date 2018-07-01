
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
using CustomEmojis.Framework.Utilities;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace CustomEmojis.Framework {

	public class EmojiAssetsLoader : IAssetLoader {

		public Texture2D VanillaTexture { get; set; }
		public Texture2D CustomTexture { get; set; }

		public Texture2D CurrentTexture { get; set; }
		public List<TextureData> LoadedImages { get; set; }

		public int EmojisSize { get; set; }
		public bool CustomTextureAdded { get; set; }
		public int NumberCustomEmojisAdded { get; set; }
		public int TotalNumberEmojis { get; private set; }
		public bool SaveGeneratedTexture { get; set; }
		public bool SaveCustomEmojiTexture { get; set; }
		public bool ShouldGenerateTexture { get; set; }

		private readonly IModHelper modHelper;
		private readonly ModData modData;
		private readonly string[] imageExtensions;
		private readonly bool saveCreatedTexture;

		public EmojiAssetsLoader(IModHelper modHelper, ModData modData, int emojisSize, string[] imageExtensions, bool saveCreatedTexture = true) {

			this.modHelper = modHelper;
			this.modData = modData;

			VanillaTexture = modHelper.Content.Load<Texture2D>("LooseSprites\\emojis", ContentSource.GameContent);

			this.NumberCustomEmojisAdded = 0;
			this.CustomTextureAdded = false;
			this.EmojisSize = emojisSize;
			this.imageExtensions = imageExtensions;
			this.saveCreatedTexture = saveCreatedTexture;

		}

		public EmojiAssetsLoader(IModHelper modHelper, ModData modData, int emojisSize, string[] imageExtensions, bool generateTexture = true, bool saveCreatedTexture = true) : this(modHelper, modData, emojisSize, imageExtensions, saveCreatedTexture) {
			this.ShouldGenerateTexture = generateTexture;
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
			swTotal.Start();

			string outputFolderPath = Path.Combine(modHelper.DirectoryPath, Assets.OutputFolder);
			Directory.CreateDirectory(outputFolderPath);

			string inputFolderPath = Path.Combine(modHelper.DirectoryPath, Assets.InputFolder);

			// If file changes are detected, make again the texture
			if(!Directory.Exists(inputFolderPath)) {
				Directory.CreateDirectory(inputFolderPath);
			} else if(modData.ShouldGenerateTexture()) {

				//if(!File.Exists(Path.Combine(modHelper.DirectoryPath, "vanillaEmojis.png"))) {
				//	SaveTextureToPng(this.VanillaEmojisTexture, Path.Combine(modHelper.DirectoryPath, "vanillaEmojis.png"));
				//}

				//List<Image> images = new List<Image> {
				//	Image.FromFile(Path.Combine(modHelper.DirectoryPath, "vanillaEmojis.png"))
				//};
				Stopwatch sw = new Stopwatch();
				ModEntry.ModMonitor.Log($"[EmojiAssetsLoader MergeEmojiImages] Timer started!");
				sw.Start();
				CustomTexture = MergeEmojiImages(inputFolderPath);
				if(CustomTexture != null) {
					SaveTextureToPng(this.CustomTexture, Path.Combine(modHelper.DirectoryPath, Assets.OutputFolder, Assets.OutputFile));
				}
				sw.Stop();
				ModEntry.ModMonitor.Log($"[EmojiAssetsLoader MergeEmojiImages] Timer Stoped! Elapsed Time: {sw.Elapsed}");

			} else if(File.Exists(Path.Combine(modHelper.DirectoryPath, Assets.OutputFolder, Assets.OutputFile)) ) {
				Stopwatch sw = new Stopwatch();
				ModEntry.ModMonitor.Log($"[EmojiAssetsLoader loadTexture] Timer started!");
				sw.Start();
				CustomTexture = modHelper.Content.Load<Texture2D>(Path.Combine(Assets.OutputFolder, Assets.OutputFile), ContentSource.ModFolder);
				sw.Stop();
				ModEntry.ModMonitor.Log($"[EmojiAssetsLoader loadTexture] Timer Stoped! Elapsed Time: {sw.Elapsed}");
				swTotal.Stop();
				ModEntry.ModMonitor.Log($"[AfterTextureCreated/Loaded] Time elapsed: {swTotal.Elapsed}");
			}

			if(CustomTexture != null) {
				this.CustomTextureAdded = true;
				this.CurrentTexture = this.CustomTexture;
			} else {
				this.CustomTextureAdded = false;
				this.CurrentTexture = this.VanillaTexture;
			}
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

			//string inputPath = Path.Combine(modHelper.DirectoryPath, inputFolder);
			//string outputPath = Path.Combine(modHelper.DirectoryPath, outputFolder);

			//if(!Directory.Exists(outputPath)) {
			//	Directory.CreateDirectory(outputPath);
			//}

			//if(!Directory.Exists(inputPath)) {
			//	Directory.CreateDirectory(inputPath);
			//} else {

			//List<Image> images = new List<Image>() {
			//	//Image.FromFile(Path.Combine(modHelper.DirectoryPath, "vanillaEmojis.png"))
			//	TextureToImage(VanillaEmojisTexture)
			//};

			var images = ModUtilities.GetFiles(inputPath, imageExtensions, SearchOption.AllDirectories);
			//var images = foundFiles.Where(imageAbsolutePath => modData.FilesChecksums.FirstKeys.Any(x => x == ModUtilities.GetRelativePath(modHelper.DirectoryPath, imageAbsolutePath)));

			//if(foundFiles.Count() != images.Count()) {
			//	ModEntry.ModMonitor.Log($"Found different images with same content, those will not be added to the generated texture.", LogLevel.Info);
			//	ModEntry.ModMonitor.Log($"Skipped duplicated files:", LogLevel.Trace);
			//	foreach(var item in foundFiles.Except(images)) {
			//		ModEntry.ModMonitor.Log($"{item}");
			//	}
			//}

			//var files = ModUtilities.GetFiles(inputPath, imageExtensions, SearchOption.AllDirectories);

			NumberCustomEmojisAdded = images.Count();

			if(NumberCustomEmojisAdded > 0) {

				//images.AddRange(GetImagesList(files));

				outputTexture = MergeTextures(VanillaTexture, new List<Image>(GetImagesList(images)));
				//foreach(string filePath in files) {
				//	Image imageToAdd = Image.FromFile(filePath);
				//	images.Add(ResizeImage(imageToAdd, EmojisSize, EmojisSize));
				//}
				/*
				Bitmap outputImage = new Bitmap(images[0].Width, images[0].Height + ((int)Math.Ceiling((double)NumberCustomEmojisAdded / 14) * EmojisSize), PixelFormat.Format32bppArgb);
				using(Graphics graphics = Graphics.FromImage(outputImage)) {
					graphics.DrawImage(images[0], new Rectangle(new Point(), images[0].Size), new Rectangle(new Point(), images[0].Size), GraphicsUnit.Pixel);
				}
				int heightAcum = 0;
				for(int i = 1; i < images.Count(); i++) {
					if((i - 1) % 14 == 0) {
						heightAcum = (((i - 1) / 14) * EmojisSize) + images[0].Height;
					}
					using(Graphics graphics = (Graphics.FromImage(outputImage))) {
						graphics.DrawImage(images[i], new Rectangle(new Point(((i - 1) % 14) * EmojisSize, heightAcum), images[i].Size), new Rectangle(new Point(), images[i].Size), GraphicsUnit.Pixel);
					}
				}

				outputImage.Save(Path.Combine(outputPath, outputFile), ImageFormat.Png);

				outputTexture = modHelper.Content.Load<Texture2D>(Path.Combine(outputFolder, outputFile), ContentSource.ModFolder);
				*/
				//}

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

		private Texture2D MergeTextures(List<Image> images, bool appendTofirst = false) {

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
				Image imageToAdd = Image.FromFile(filePath);
				images.Add(ResizeImage(imageToAdd, EmojisSize, EmojisSize));
			}
			return images;
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
