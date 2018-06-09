
using StardewValley.Menus;

public class ModConfig {
	
	/// <summary>
	/// Animate icon within the emote button 
	/// </summary>
	public bool AnimateEmoteButtonIcon { get; set; }

	/// <summary>
	/// Show or hide tooltip on hover
	/// </summary>
	public bool ShowTooltipOnHover { get; set; }	

	public ModConfig() {
		this.AnimateEmoteButtonIcon = true;
		this.ShowTooltipOnHover = true;
	}
}
