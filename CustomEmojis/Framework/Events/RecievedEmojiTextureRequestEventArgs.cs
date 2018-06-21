
using StardewValley;
using System;

namespace CustomEmojis.Framework.Events {
	public class RecievedEmojiTextureRequestEventArgs : EventArgs {
		public Farmer SourceFarmer { get; set; }
	}
}
