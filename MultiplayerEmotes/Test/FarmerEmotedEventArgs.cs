using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Test {

	public class FarmerEmotedEventArgs : EventArgs {

		//public Farmer EmoteAuthor { get; set; }
		public bool IsEmoting { get; set; }
		public int Emote { get; set; }

		public FarmerEmotedEventArgs(bool isEmoting, int whichEmote) {
			IsEmoting = isEmoting;
			Emote = whichEmote;
		}
	}

	public delegate void FarmerEmotedEventHandler(System.Object sender, FarmerEmotedEventArgs e);

}
