
public class ModConfig {

	//public int EmojisSize { get; set; }
	public string[] ImageExtensions { get; set; }
	public string InputFolder { get; set; }
	public string OutputFolder { get; set; }

	public ModConfig() {
		//this.EmojisSize = EmojiMenu.EMOJI_SIZE;
		this.ImageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".gif"};
		this.InputFolder = "sprites";
		this.OutputFolder = "mergedSprite";
	}
}
